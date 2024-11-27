using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class ProdaneZbozi
    {
        [Required]
        public int Pocet { get; set; }

        [Required]
        public decimal ProdejniCena { get; set; }

        [Required]
        public int ZboziIdZbozi { get; set; }

        [Required]
        public int ProdejIdProdeje { get; set; }

        [Required]
        public int ProdejZboziIdZbozi { get; set; }

        [Required]
        public int IdTranzakce { get; set; }
    }
}
