using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP_Project.Models
{
    [Table("motori")]
    public class Motori
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Unesite proizvođača")]
        [StringLength(100)]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Unesite model")]
        [StringLength(100)]
        [Column("Model")]
        public string NazivModela { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Snaga mora biti veća od 0")]
        public int Power { get; set; }

        [Range(1900, 2100, ErrorMessage = "Unesite ispravnu godinu")]
        public int Year { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unesite cijenu")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}