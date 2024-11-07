using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Kategorie
    {
        [Key]
        public int IdKategorii { get; set; }
        public string Nazev { get; set; }
    }
}
