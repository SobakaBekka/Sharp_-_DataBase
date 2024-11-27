using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Zbozi
    {
        [Key]
        public int IdZbozi { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Název musí mít maximálně 20 znaků.")]
        public string Nazev { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Cena musí být kladná.")]
        public decimal AktualniCena { get; set; }

        public decimal? CenaZeKlubKartou { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Hmotnost musí být kladná.")]
        public decimal Hmotnost { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Složení musí mít maximálně 100 znaků.")]
        public string Slozeni { get; set; }

        [Required]
        public int KategorieIdKategorii { get; set; }
    }
}
