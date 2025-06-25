using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP_Project.Models
{
    public class Racun
    {
        [Key]
        public int RacunID { get; set; }

        public DateTime DatumKupnje { get; set; }

        public decimal UkupanIznos { get; set; }

        public int KorisnikID { get; set; }

        [ForeignKey("KorisnikID")]
        public virtual RegistracijaKorisnika Korisnik { get; set; }

        public virtual ICollection<RacunStavka> RacunStavke { get; set; }
    }
}