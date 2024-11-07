using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Prodejna
    {
        [Key]
        public int IdProdejny { get; set; }
        public int KontaktniCislo { get; set; }
        public int Plocha { get; set; }
    }
}
