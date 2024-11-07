using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Kupon
    {
        [Key]
        public int IdTranzakce { get; set; }
        public int Cislo { get; set; }
    }
}
