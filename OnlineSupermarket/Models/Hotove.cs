using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Hotove
    {
        [Key]
        public int IdTranzakce { get; set; }

        [Required]
        public decimal Vraceni { get; set; }
    }
}
