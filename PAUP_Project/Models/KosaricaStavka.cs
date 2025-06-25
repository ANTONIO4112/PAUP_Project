using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP_Project.Models
{
    [Table("kosaricastavke")]
    public class KosaricaStavka
    {
        [Key]
        public int Id { get; set; }

        public string KorisnickoIme { get; set; }

        public int MotorId { get; set; }

        public int Kolicina { get; set; }

        [ForeignKey("MotorId")]
        public virtual Motori Motor { get; set; }
    }
}