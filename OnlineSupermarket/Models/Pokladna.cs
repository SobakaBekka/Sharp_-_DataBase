using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Pokladna
    {
        [Key]
        public int IdPokladny { get; set; }  // відповідає колонці "IDPOKLADNY"

        [Required]
        public int Cislo { get; set; }  // відповідає колонці "CISLO"

        [Required]
        public int Samoobsluzna { get; set; }  // відповідає колонці "SAMOOBSLUZNA"
    }
}
