using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Prodejna
    {
        [Key]
        public int IdProdejny { get; set; }

        [Required]
        [StringLength(15)]
        public string Kontaktnicislo { get; set; }

        [Required]
        public int Plocha { get; set; }

        public int? PokladnaIdPokladny { get; set; }

        [Required]
        public DateTime DatumVytvoreni { get; set; }

        [Required]
        public DateTime DatumAktualizace { get; set; }
    }
}