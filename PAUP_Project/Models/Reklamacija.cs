using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP_Project.Models
{
    [Table("reklamacije")]
    public class Reklamacija
    {
        [Key]
        public int ReklamacijaID { get; set; }

        public int RacunStavkaID { get; set; }

        public string KorisnickoIme { get; set; }

        public string Opis { get; set; }

        public DateTime DatumReklamacije { get; set; }

        [ForeignKey("RacunStavkaID")]
        public virtual RacunStavka RacunStavka { get; set; }
    }
}