using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Platba
    {
        [Key]
        public int IdTranzakce { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "CelkovaCena musí být kladná.")]
        public decimal CelkovaCena { get; set; }

        [Required]
        [StringLength(10)]
        public string Typ { get; set; }

        [Required]
        public int ZboziIdZbozi { get; set; }

        [Required]
        public int ProdejnaIdProdejny { get; set; }

        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
    }
}