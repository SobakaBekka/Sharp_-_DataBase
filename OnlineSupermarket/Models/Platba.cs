using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Platba
    {
        [Key]
        public int IdTranzakce { get; set; }  // відповідає колонці "IDTRANZAKCE"

        [Required]
        public decimal CelkovaCena { get; set; }  // відповідає колонці "CELKOVACENA"

        [Required]
        public int ProdejIdProdeje { get; set; }  // відповідає колонці "PRODEJ_IDPRODEJE"

        [Required]
        public int ProdejZboziIdZbozi { get; set; }  // відповідає колонці "PRODEJ_ZBOZI_IDZBOZI"

        [Required]
        [StringLength(10)]
        public string Typ { get; set; }  // відповідає колонці "TYP"

        // Навігаційна властивість для зв’язку з таблицею "PRODEJ"
        [ForeignKey("ProdejIdProdeje, ProdejZboziIdZbozi")]
        public virtual Prodej Prodej { get; set; }
    }
}
