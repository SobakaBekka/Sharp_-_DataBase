using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Sklad
    {
        [Key]
        public int IdSkladu { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Počet políček musí být kladný.")]
        public int PocetPolicek { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Plocha musí být kladná nebo nula.")]
        public decimal? Plocha { get; set; }
    }
}
