using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Platba
    {
        [Key]
        public int IdTranzakce { get; set; }
        public decimal CelkovaCena { get; set; }
        public int ProdejIdProdeje { get; set; }
        public int ProdejZboziIdZbozi { get; set; }
        public string Typ { get; set; }
    }
}
