using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Pult
    {
        [Key]
        public int IdPultu { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Číslo musí mít maximálně 10 znaků.")]
        public string Cislo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Počet poiček musí být kladný.")]
        public int PocetPoicek { get; set; }

        public int? ProdejnaIdProdejny { get; set; } // Nullable foreign key

        public int? IdKategorii { get; set; } // Nullable foreign key
    }
}
