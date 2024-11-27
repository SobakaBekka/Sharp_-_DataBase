using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Prodejna
    {
        [Key]
        public int IdProdejny { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "Kontaktní číslo musí mít mezi 9 a 15 znaky.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Kontaktní číslo musí obsahovat pouze číslice.")]
        public string KontaktniCislo { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Plocha musí být kladná.")]
        public decimal Plocha { get; set; }

        public int? PokladnaIdPokladny { get; set; } // Nullable foreign key
    }
}
