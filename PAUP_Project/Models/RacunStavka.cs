using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP_Project.Models
{
    [Table("racunstavke")]
    public class RacunStavka
    {
        [Key]
        [Column("RacunStavkaID")]
        public int Id { get; set; }

        public int RacunID { get; set; }

        public int MotorId { get; set; }

        public int Kolicina { get; set; }

        public decimal Iznos { get; set; }

        public decimal UkupanIznos { get; set; }

        [ForeignKey("RacunID")]
        public virtual Racun Racun { get; set; }

        [ForeignKey("MotorId")]
        public virtual Motori Motor { get; set; }
    }
}