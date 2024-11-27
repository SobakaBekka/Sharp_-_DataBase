using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class ZboziNaSklade
    {
        [Required]
        public int ZboziIdZbozi { get; set; }

        [Required]
        public int SkladIdSkladu { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Počet musí být kladné číslo.")]
        public int Pocet { get; set; }

        public int? AdresaIdAdresy { get; set; }

        [Required]
        public int KategorieIdKategorii { get; set; }
    }
}
