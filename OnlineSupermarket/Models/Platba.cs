using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Platba
    {
        [Key]
        public int IdTranzakce { get; set; }

        [Required]
        public decimal CelkovaCena { get; set; }

        [Required]
        public int ProdejIdProdeje { get; set; }

        [Required]
        public int ProdejZboziIdZbozi { get; set; }

        [Required]
        [StringLength(10)]
        public string Typ { get; set; }
    }
}
