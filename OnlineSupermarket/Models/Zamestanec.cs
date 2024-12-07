using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Zamestanec
    {
        [Key]
        public int IdZamestance { get; set; }

        [Required]
        [StringLength(50)]
        public string Jmeno { get; set; }

        [Required]
        [StringLength(50)]
        public string Prijmeni { get; set; }

        [Required]
        [StringLength(20)]
        public string RodneCislo { get; set; }

        [Required]
        [StringLength(15)]
        public string TelefonniCislo { get; set; }

        [Required]
        public int PoziceIdPozice { get; set; }

        public int? ProdejnaIdProdejny { get; set; }

        public int? IdNadrezene { get; set; }

        [Required]
        public DateTime DatumVytvoreni { get; set; }

        [Required]
        public DateTime DatumAktualizace { get; set; }
    }
}