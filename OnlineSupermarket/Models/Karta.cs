using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Karta
    {
        [Key]
        public int IdTranzakce { get; set; }
        public int AutorizacniKod { get; set; }
        public int Cislo { get; set; }
    }
}
