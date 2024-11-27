using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Kupon
    {
        [Key]
        public int IdTranzakce { get; set; }

        [Required]
        [StringLength(20)]
        public string Cislo { get; set; }
    }
}
