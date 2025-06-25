using System.ComponentModel.DataAnnotations;

namespace PAUP_Project.Models
{
    public class AdminPrijava
    {
        [Required(ErrorMessage = "Molimo unesite korisničko ime")]
        [Display(Name = "Korisničko ime")]
        public string KorisnickoIme { get; set; }

        [Required(ErrorMessage = "Molimo unesite lozinku")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Lozinka { get; set; }
    }
}
