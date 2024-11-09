using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Zamestnanec
    {
        [Key]
        public int IdZamestance { get; set; }  // відповідає колонці "IDZAMESTANCE"

        [Required]
        [StringLength(20)]
        public string Jmeno { get; set; }  // відповідає колонці "JMENO"

        [Required]
        [StringLength(30)]
        public string Prijmeni { get; set; }  // відповідає колонці "PRIJMENI"

        [Required]
        public DateTime RodneCislo { get; set; }  // відповідає колонці "RODNECISLO"

        [Required]
        public long TelefonniCislo { get; set; }  // відповідає колонці "TELEFONICISLO"

        [Required]
        public int PoziceIdPozice { get; set; }  // відповідає колонці "POZICE_IDPOZICE"

        [Required]
        public int ProdejnaIdProdejny { get; set; }  // відповідає колонці "PRODEJNA_IDPRODEJNY"

        public int? IdNadrezene { get; set; }  // відповідає колонці "IDNADREZENE"

        public int ZamestnanecIdZamestance { get; set; }  // відповідає колонці "ZAMESTANEC_IDZAMESTANCE"

        public int ZamestnanecIdZamestance1 { get; set; }  // відповідає колонці "ZAMESTANEC_IDZAMESTANCE1"

        // Навігаційні властивості
        [ForeignKey("PoziceIdPozice")]
        public virtual Pozice Pozice { get; set; }

        [ForeignKey("ProdejnaIdProdejny")]
        public virtual Prodejna Prodejna { get; set; }

        [ForeignKey("IdNadrezene")]
        public virtual Zamestnanec Nadrazeny { get; set; }

        [ForeignKey("ZamestnanecIdZamestance")]
        public virtual Zamestnanec Podrizeny { get; set; }

        [ForeignKey("ZamestnanecIdZamestance1")]
        public virtual Zamestnanec Podrizeny1 { get; set; }
    }
}
