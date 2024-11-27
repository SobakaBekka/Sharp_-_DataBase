using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Karta
    {
        [Key]
        public int IdTranzakce { get; set; }

        [Required]
        public int AutorizacniKod { get; set; }

        [Required]
        public int Cislo { get; set; }
    }
}
