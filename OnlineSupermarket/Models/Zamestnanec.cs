using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Zamestnanec
    {
        [Key]
        public int IdZamestance { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Jméno musí mít maximálně 20 znaků.")]
        public string Jmeno { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Příjmení musí mít maximálně 30 znaků.")]
        public string Prijmeni { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Rodné číslo musí mít maximálně 20 znaků.")]
        public string RodneCislo { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Telefonní číslo musí mít maximálně 20 znaků.")]
        [RegularExpression(@"^\+?[0-9\s\-]+$", ErrorMessage = "Telefonní číslo může obsahovat pouze číslice, mezery, pomlčky a +.")]
        public string TelefonniCislo { get; set; }

        [Required]
        public int PoziceIdPozice { get; set; }

        [Required]
        public int ProdejnaIdProdejny { get; set; }

        public int? IdNadrezene { get; set; }
    }
}
