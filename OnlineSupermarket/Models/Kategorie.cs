using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Kategorie
    {
        [Key]
        public int IdKategorii { get; set; }

        [Required]
        [StringLength(20)]
        public string Nazev { get; set; }
    }
}
