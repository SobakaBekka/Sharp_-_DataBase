using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Pozice
    {
        [Key]
        public int IdPozice { get; set; }

        [Required]
        [StringLength(20)]
        public string Nazev { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Mzda musí být nezáporná.")]
        public decimal Mzda { get; set; }

    }

}
