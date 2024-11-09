using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Kategorie
    {
        [Key]
        public int IdKategorii { get; set; }  // відповідає колонці "IDKATEGORII"

        [Required]
        [StringLength(20)]
        public string Nazev { get; set; }  // відповідає колонці "NAZEV"
    }
}
