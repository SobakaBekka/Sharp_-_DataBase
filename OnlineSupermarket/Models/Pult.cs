using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Pult
    {
        [Key]
        public int IdPultu { get; set; }
        public int Cislo { get; set; }
        public int PocetPoicek { get; set; }
        public int ProdejnaIdProdejny { get; set; }
    }
}
