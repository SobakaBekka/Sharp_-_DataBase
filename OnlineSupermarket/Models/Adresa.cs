using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Adresa
    {
        [Key]
        public int IdAdresy { get; set; }  // відповідає колонці "IDADRESY"

        [Required]
        [StringLength(20)]
        public string Mesto { get; set; }  // відповідає колонці "MESTO"

        [Required]
        [StringLength(20)]
        public string Ulice { get; set; }  // відповідає колонці "ULICE"

        public int? ProdejnaIdProdejny { get; set; }  // відповідає колонці "PRODEJNA_IDPRODEJNY"
        public int? ZamestnanecIdZamestance { get; set; }  // відповідає колонці "ZAMESTANEC_IDZAMESTANCE"

        [Required]
        public int SkladIdSkladu { get; set; }  // відповідає колонці "SKLAD_IDSKLADU"

        // Навігаційні властивості для зв'язків з іншими таблицями
        [ForeignKey("ProdejnaIdProdejny")]
        public virtual Prodejna? Prodejna { get; set; }

        [ForeignKey("ZamestnanecIdZamestance")]
        public virtual Zamestnanec? Zamestnanec { get; set; }

        [ForeignKey("SkladIdSkladu")]
        public virtual Sklad Sklad { get; set; }
    }
}
