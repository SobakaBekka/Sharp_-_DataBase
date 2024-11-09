using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Pult
    {
        [Key]
        public int IdPultu { get; set; }  // відповідає колонці "IDPULTU"

        [Required]
        public int Cislo { get; set; }  // відповідає колонці "CISLO"

        [Required]
        public int PocetPoicek { get; set; }  // відповідає колонці "POCETPOICEK"

        public int? ProdejnaIdProdejny { get; set; }  // відповідає колонці "PRODEJNA_IDPRODEJNY"

        // Навігаційна властивість для зв’язку з таблицею "PRODEJNA"
        [ForeignKey("ProdejnaIdProdejny")]
        public virtual Prodejna? Prodejna { get; set; }
    }
}
