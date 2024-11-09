using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Pozice
    {
        [Key]
        public int IdPozice { get; set; }  // відповідає колонці "IDPOZICE"

        [Required]
        [StringLength(20)]
        public string Nazev { get; set; }  // відповідає колонці "NAZEV"

        [Required]
        public decimal Mzda { get; set; }  // відповідає колонці "MZDA"
    }
}
