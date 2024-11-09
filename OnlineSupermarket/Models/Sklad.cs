using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Sklad
    {
        [Key]
        public int IdSkladu { get; set; }  // відповідає колонці "IDSKLADU"

        [Required]
        public int PocetPolicek { get; set; }  // відповідає колонці "POCETPOLICEK"

        public int? Plocha { get; set; }  // відповідає колонці "PLOCHA"
    }
}
