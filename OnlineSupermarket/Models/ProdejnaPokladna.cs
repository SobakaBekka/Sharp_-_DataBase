using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class ProdejnaPokladna
    {
        [Key]
        public int ProdejnaIdProdejny { get; set; }
        public int PokladnaIdPokladny { get; set; }
    }
}
