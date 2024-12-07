using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Adresa
    {
        [Key]
        public int IdAdresy { get; set; }

        [Required]
        [StringLength(50)]
        public string Mesto { get; set; }

        [Required]
        [StringLength(100)]
        public string Ulice { get; set; }

        [Required]
        [StringLength(10)]
        public string Psc { get; set; }

        public int? ProdejnaIdProdejny { get; set; }
        public int? ZamestnanecIdZamestance { get; set; }
        public int? SkladIdSkladu { get; set; }

        [Required]
        public DateTime DatumVytvoreni { get; set; }

        [Required]
        public DateTime DatumAktualizace { get; set; }
    }
}