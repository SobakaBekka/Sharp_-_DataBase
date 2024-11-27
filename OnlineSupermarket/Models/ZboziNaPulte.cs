using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class ZboziNaPulte
    {
        [Required]
        public int PultIdPultu { get; set; }

        [Required]
        public int ZboziIdZbozi { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Počet musí být kladné číslo.")]
        public int Pocet { get; set; }
    }
}
