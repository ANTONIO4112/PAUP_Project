using System.Linq;
using System.Web.Mvc;
using PAUP_Project.Models;
using System.Web.Security;
using System.Collections.Generic;
using System;
using System.Data.Entity.Validation;

namespace PAUP_Project.Controllers
{
    public class KorisniciController : Controller
    {
        private readonly BazaDbContext _context;

        public KorisniciController()
        {
            _context = new BazaDbContext();
        }

        [HttpGet]
        public ActionResult Prijava()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Prijava(PrijavaKorisnika prijava)
        {
            if (ModelState.IsValid)
            {
                var korisnik = _context.RegistracijaKorisnika
                    .FirstOrDefault(k => k.KorisnickoIme == prijava.KorisnickoIme && k.Lozinka == prijava.Lozinka);

                if (korisnik != null)
                {
                    FormsAuthentication.SetAuthCookie(korisnik.KorisnickoIme, false);
                    Session["KorisnickoIme"] = korisnik.KorisnickoIme; // Postavljanje KorisnickoIme u sesiju

                    // Postavljanje KorisnickogIme u ViewBag
                    ViewBag.KorisnickoIme = korisnik.KorisnickoIme;

                    return RedirectToAction("PopisZaKorisnika", "Motori");
                }
                else
                {
                    ModelState.AddModelError("", "Pogrešno korisničko ime ili lozinka");
                }
            }
            return View(prijava);
        }



        [HttpGet]
        public ActionResult Registracija()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registracija(RegistracijaKorisnika model)
        {
            if (ModelState.IsValid)
            {
                var korisnik = new RegistracijaKorisnika
                {
                    KorisnickoIme = model.KorisnickoIme,
                    Lozinka = model.Lozinka,
                    Email = model.Email,
                    PrezimeIme = model.PrezimeIme,
                    OIB = model.OIB
                };

                try
                {
                    _context.RegistracijaKorisnika.Add(korisnik);
                    _context.SaveChanges();

                    return RedirectToAction("Prijava");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult PromjenaLozinke()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PromijeniLozinku(PromjenaLozinkeModel model)
        {
            if (ModelState.IsValid)
            {
                var korisnik = _context.RegistracijaKorisnika
                    .FirstOrDefault(k => k.KorisnickoIme == model.KorisnickoIme && k.Lozinka == model.StaraLozinka);

                if (korisnik != null)
                {
                    korisnik.Lozinka = model.NovaLozinka;
                    _context.SaveChanges();
                    ViewBag.Message = "Lozinka je uspješno promijenjena.";
                }
                else
                {
                    ModelState.AddModelError("", "Pogrešno korisničko ime ili stara lozinka.");
                }
            }

            return View("PromjenaLozinke", model);
        }

        public ActionResult PopisKupaca()
        {
            try
            {
                var kupci = _context.RegistracijaKorisnika.ToList();

                foreach (var korisnik in kupci)
                {
                    System.Diagnostics.Debug.WriteLine($"Korisnik ID: {korisnik.Id}, Korisničko ime: {korisnik.KorisnickoIme}");
                }

                return View(kupci);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Došlo je do pogreške prilikom dohvaćanja podataka: " + ex.Message;
                return View(new List<RegistracijaKorisnika>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BrisiKorisnika(int id)
        {
            try
            {
                var korisnik = _context.RegistracijaKorisnika.FirstOrDefault(k => k.Id == id);
                if (korisnik == null)
                {
                    return HttpNotFound();
                }

                _context.RegistracijaKorisnika.Remove(korisnik);
                _context.SaveChanges();

                return RedirectToAction("PopisKupaca");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Došlo je do pogreške prilikom brisanja korisnika: " + ex.Message;
                return View("PopisKupaca", _context.RegistracijaKorisnika.ToList());
            }
        }

        public ActionResult PotvrdaBrisanja(int id)
        {
            var korisnik = _context.RegistracijaKorisnika.Find(id);
            if (korisnik == null)
            {
                return HttpNotFound();
            }

            return View(korisnik);
        }
    }
}
