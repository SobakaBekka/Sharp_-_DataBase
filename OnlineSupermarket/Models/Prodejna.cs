using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Prodejna
    {
        [Key]
        public int IdProdejny { get; set; }  // відповідає колонці "IDPRODEJNY"

        [Required]
        public int KontaktniCislo { get; set; }  // відповідає колонці "KONTAKTNICISLO"

        [Required]
        public int Plocha { get; set; }  // відповідає колонці "PLOCHA"

        public int? PokladnaIdPokladny { get; set; }  // відповідає колонці "POKLADNA_IDPOKLADNY"

        // Навігаційна властивість для зв’язку з таблицею "POKLADNA"
        [ForeignKey("PokladnaIdPokladny")]
        public virtual Pokladna? Pokladna { get; set; }
    }
}
