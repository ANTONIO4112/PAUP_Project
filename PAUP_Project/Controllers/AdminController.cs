using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PAUP_Project.Models;

namespace PAUP_Project.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Prijava()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Prijava(string korisnickoIme, string lozinka)
        {
            if (korisnickoIme == "motori" && lozinka == "motori")
            {
                HttpCookie authCookie = new HttpCookie("AdminKorisnik");
                authCookie.Value = korisnickoIme;
                Response.Cookies.Add(authCookie);

                return RedirectToAction("PopisZaAdmina", "Motori");
            }

            ModelState.AddModelError("", "Neispravno korisničko ime ili lozinka");
            return View();
        }

        [HttpPost]
        public ActionResult Odjava()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
