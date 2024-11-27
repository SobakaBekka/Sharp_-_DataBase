using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Pokladna
    {
        [Key]
        public int IdPokladny { get; set; }

        [Required]
        public bool Samoobsluzna { get; set; }
    }
}
