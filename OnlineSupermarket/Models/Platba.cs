using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Platba
    {
        [Key]
        public int IdTranzakce { get; set; }

        [Required]
        public decimal CelkovaCena { get; set; }

        [Required]
        [StringLength(10)]
        public string Typ { get; set; }

        [Required]
        public DateTime DatumVytvoreni { get; set; }

        [Required]
        public DateTime DatumAktualizace { get; set; }

        [Required]
        public int ZboziIdZbozi { get; set; }

        [ForeignKey("ZboziIdZbozi")]
        public Zbozi Zbozi { get; set; }

        public int? ProdejnaIdProdejny { get; set; }

        [ForeignKey("ProdejnaIdProdejny")]
        public Prodejna Prodejna { get; set; }
    }
}