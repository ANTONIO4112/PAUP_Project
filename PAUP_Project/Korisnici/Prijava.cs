using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP_Project.Models
{
    [Table("PrijavaKorisnika")]
    public class PrijavaKorisnika
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Unesite korisničko ime")]
        public string KorisnickoIme { get; set; }

        [Required(ErrorMessage = "Unesite lozinku")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; }
    }
}
