using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Adresa
    {
        [Key]
        public int IdAdresy { get; set; }

        [Required]
        [StringLength(20)]
        public string Mesto { get; set; }

        [Required]
        [StringLength(25)]
        public string Ulice { get; set; }

        public int? ProdejnaIdProdejny { get; set; }

        public int? ZamestnanecIdZamestance { get; set; }

        public int? SkladIdSkladu { get; set; }
    }
}
