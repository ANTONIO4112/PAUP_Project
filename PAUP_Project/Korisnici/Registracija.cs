using System.ComponentModel.DataAnnotations;

namespace PAUP_Project.Models
{
    public class RegistracijaKorisnika
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Unesite korisničko ime")]
        [StringLength(50)]
        public string KorisnickoIme { get; set; }

        [Required(ErrorMessage = "Unesite lozinku")]
        [DataType(DataType.Password)]
        [StringLength(50)]
        public string Lozinka { get; set; }

        [Required(ErrorMessage = "Unesite email adresu")]
        [EmailAddress(ErrorMessage = "Unesite valjanu email adresu")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Unesite svoje puno ime")]
        [StringLength(100)]
        public string PrezimeIme { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati točno 11 znamenki")]
        public string OIB { get; set; }
    }
}
