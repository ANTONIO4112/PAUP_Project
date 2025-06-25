using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using PAUP_Project.Models;

namespace PAUP_Project.Controllers
{
    public class MotoriController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        public ActionResult PopisZaKorisnika(string model, string brand, int? page)
        {
            var motori = db.Motori.AsQueryable();

            if (!string.IsNullOrEmpty(model))
                motori = motori.Where(m => m.NazivModela.Contains(model));
            if (!string.IsNullOrEmpty(brand))
                motori = motori.Where(m => m.Brand.Contains(brand));

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.KorisnickoIme = Session["KorisnickoIme"];
            return View(motori.OrderBy(m => m.Id).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DetaljiZaKorisnika(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Motori motor = db.Motori.Find(id);
            if (motor == null)
                return HttpNotFound();

            return View(motor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DodajUKosaricu(int motorId, int kolicina)
        {
            string korisnik = Session["KorisnickoIme"]?.ToString();
            if (string.IsNullOrEmpty(korisnik))
                return RedirectToAction("Login", "Home");

            var stavka = db.KosaricaStavke.FirstOrDefault(s => s.KorisnickoIme == korisnik && s.MotorId == motorId);

            if (stavka != null)
            {
                stavka.Kolicina += kolicina;
                db.Entry(stavka).State = EntityState.Modified;
            }
            else
            {
                db.KosaricaStavke.Add(new KosaricaStavka
                {
                    KorisnickoIme = korisnik,
                    MotorId = motorId,
                    Kolicina = kolicina
                });
            }

            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Kosarica()
        {
            string korisnik = Session["KorisnickoIme"]?.ToString();
            if (string.IsNullOrEmpty(korisnik))
                return RedirectToAction("Login", "Home");

            var stavke = db.KosaricaStavke
                           .Include(s => s.Motor)
                           .Where(s => s.KorisnickoIme == korisnik)
                           .ToList();

            ViewBag.KorisnickoIme = korisnik;
            return View(stavke);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ObrisiIzKosarice(int id)
        {
            var korisnik = Session["KorisnickoIme"]?.ToString();

            var stavka = db.KosaricaStavke.FirstOrDefault(s => s.Id == id && s.KorisnickoIme == korisnik);

            if (stavka != null)
            {
                db.KosaricaStavke.Remove(stavka);
                db.SaveChanges();
            }

            return RedirectToAction("Kosarica");
        }

        public ActionResult MojeKupnje()
        {
            string korisnik = Session["KorisnickoIme"]?.ToString();
            var racuni = db.Racuni
                           .Include(r => r.RacunStavke.Select(rs => rs.Motor))
                           .Where(r => r.Korisnik.KorisnickoIme == korisnik)
                           .ToList();

            return View(racuni);
        }
        public ActionResult DetaljiRacunaZaKupca(int id)
        {
            var racun = db.Racuni
                .Include(r => r.RacunStavke.Select(s => s.Motor))
                .FirstOrDefault(r => r.RacunID == id);

            if (racun == null)
            {
                return HttpNotFound();
            }

            ViewBag.KorisnickoIme = racun.Korisnik.KorisnickoIme;
            return View("DetaljiRacunaZaKupca", racun);
        }

        public ActionResult PrikaziRacun()
        {
            string korisnickoIme = Session["KorisnickoIme"] as string;
            if (string.IsNullOrEmpty(korisnickoIme))
                return RedirectToAction("Login", "Home");

            using (var db = new BazaDbContext())
            {
                var korisnik = db.RegistracijaKorisnika.FirstOrDefault(k => k.KorisnickoIme == korisnickoIme);

                var stavke = db.KosaricaStavke
                               .Include(s => s.Motor)
                               .Where(s => s.KorisnickoIme == korisnickoIme)
                               .ToList();

                if (!stavke.Any())
                {
                    TempData["Message"] = "Košarica je prazna.";
                    return RedirectToAction("Kosarica");
                }

                var racun = new Racun
                {
                    DatumKupnje = DateTime.Now,
                    KorisnikID = korisnik.Id, // Ili postavi Korisnik = korisnik
                    RacunStavke = stavke.Select(s => new RacunStavka
                    {
                        MotorId = s.MotorId,
                        Motor = s.Motor,
                        Kolicina = s.Kolicina,
                        Iznos = s.Motor.Price,
                        UkupanIznos = s.Motor.Price * s.Kolicina
                    }).ToList()
                };

                racun.UkupanIznos = racun.RacunStavke.Sum(rs => rs.UkupanIznos);

                db.Racuni.Add(racun);
                db.KosaricaStavke.RemoveRange(stavke);
                db.SaveChanges();

                ViewBag.KorisnickoIme = korisnickoIme;
                return View(racun);
            }
        }

        [HttpGet]
        public ActionResult PrijavaReklamacije(int racunStavkaId)
        {
            var stavka = db.RacunStavke.Include("Racun").Include("Motor")
                           .FirstOrDefault(r => r.Id == racunStavkaId);

            if (stavka == null)
                return HttpNotFound();

            var reklamacija = new Reklamacija
            {
                RacunStavkaID = stavka.Id,
                KorisnickoIme = stavka.Racun.Korisnik.KorisnickoIme,
                DatumReklamacije = DateTime.Now
            };

            return View("PrijavaReklamacije", reklamacija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrijaviReklamaciju(Reklamacija reklamacija)
        {
            if (ModelState.IsValid)
            {
                db.Reklamacije.Add(reklamacija);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Reklamacija je uspješno prijavljena.";
                return RedirectToAction("MojeKupnje");
            }

            return View("PrijavaReklamacije", reklamacija);
        }

        // ========================== ADMIN I CRUD ==========================

        public ActionResult Index()
        {
            return View(db.Motori.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Motori motori = db.Motori.Find(id);
            if (motori == null)
                return HttpNotFound();
            return View(motori);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Brand,Model,Power,Year,Price")] Motori motori)
        {
            if (ModelState.IsValid)
            {
                db.Motori.Add(motori);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(motori);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Motori motori = db.Motori.Find(id);
            if (motori == null)
                return HttpNotFound();
            return View(motori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Brand,Model,Power,Year,Price")] Motori motori)
        {
            if (ModelState.IsValid)
            {
                db.Entry(motori).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(motori);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Motori motori = db.Motori.Find(id);
            if (motori == null)
                return HttpNotFound();
            return View(motori);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Motori motori = db.Motori.Find(id);
            db.Motori.Remove(motori);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult PregledReklamacija()
        {
            using (var db = new BazaDbContext())
            {
                var reklamacije = db.Reklamacije
                    .Include("RacunStavka.Motor")
                    .ToList();

                if (!reklamacije.Any())
                {
                    ViewBag.Message = "Trenutno nema prijavljenih reklamacija.";
                }

                return View(reklamacije);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrihvatiReklamaciju(int id)
        {
            var reklamacija = db.Reklamacije
                .Include(r => r.RacunStavka)
                .FirstOrDefault(r => r.ReklamacijaID == id);

            if (reklamacija != null)
            {
                var stavka = reklamacija.RacunStavka;
                var racunId = stavka.RacunID;

                db.RacunStavke.Remove(stavka);

                var preostaleStavke = db.RacunStavke.Where(rs => rs.RacunID == racunId).ToList();
                if (!preostaleStavke.Any())
                {
                    var racun = db.Racuni.Find(racunId);
                    if (racun != null)
                    {
                        db.Racuni.Remove(racun);
                    }
                }

                db.Reklamacije.Remove(reklamacija);

                db.SaveChanges();
            }

            return RedirectToAction("PregledReklamacija");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OdbijReklamaciju(int id)
        {
            var reklamacija = db.Reklamacije.Find(id);
            if (reklamacija != null)
            {
                db.Reklamacije.Remove(reklamacija);
                db.SaveChanges();
            }
            return RedirectToAction("PregledReklamacija");
        }

        [HttpGet]
        public ActionResult DodajNoviMotor()
        {
            return View();
        }

        public ActionResult PopisZaAdmina(string model, string brand, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var motori = db.Motori.AsQueryable();

            if (!string.IsNullOrEmpty(model))
            {
                motori = motori.Where(m => m.NazivModela.Contains(model));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                motori = motori.Where(m => m.Brand.Contains(brand));
            }

            ViewBag.ModelFilter = model;
            ViewBag.BrandFilter = brand;

            var pagedList = motori.OrderBy(m => m.Id).ToPagedList(pageNumber, pageSize);

            if (!pagedList.Any())
            {
                ViewBag.Message = "Nema rezultata za tražene kriterije.";
            }

            return View(pagedList);
        }

        public ActionResult DetaljiZaAdmina(int id)
        {
            var motor = db.Motori.Find(id);
            if (motor == null)
            {
                return HttpNotFound();
            }

            return View(motor);
        }

        [HttpGet]
        public ActionResult Brisi(int id)
        {
            var motor = db.Motori.Find(id);
            if (motor == null)
            {
                return HttpNotFound();
            }

            return View(motor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Brisi(Motori model)
        {
            var motor = db.Motori.Find(model.Id);
            if (motor == null)
            {
                return HttpNotFound();
            }

            db.Motori.Remove(motor);
            db.SaveChanges();

            return RedirectToAction("PopisZaAdmina");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DodajNoviMotor(Motori motor)
        {
            if (ModelState.IsValid)
            {
                db.Motori.Add(motor);
                db.SaveChanges();
                return RedirectToAction("PopisZaAdmina");
            }

            ViewBag.Novi = true;
            return View("Azuriraj", motor);
        }

        [HttpGet]
        public ActionResult Azuriraj(int id)
        {
            var motor = db.Motori.Find(id);
            if (motor == null)
            {
                return HttpNotFound();
            }

            ViewBag.Novi = false;
            return View("Azuriraj", motor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Spremi(Motori model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Novi = model != null && model.Id == 0;
                return View("Azuriraj", model ?? new Motori());
            }


            if (model.Id == 0)
            {
                db.Motori.Add(model);
            }
            else
            {
                var motor = db.Motori.Find(model.Id);
                if (motor == null)
                {
                    return HttpNotFound();
                }

                motor.Brand = model.Brand;
                motor.NazivModela = model.NazivModela;
                motor.Power = model.Power;
                motor.Year = model.Year;
                motor.Price = model.Price;
            }

            db.SaveChanges();
            return RedirectToAction("PopisZaAdmina");
        }

        [HttpGet]
        public ActionResult PopisSvihRacuna()
        {
            var racuni = db.Racuni
                .Include("RacunStavke.Motor")
                .Include("Korisnik")
                .ToList();

            return View("PopisSvihRacuna", racuni);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}