using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Zbozi
    {
        [Key]
        public int IdZbozi { get; set; }  // відповідає колонці "IDZBOZI"

        [Required]
        [StringLength(20)]
        public string Nazev { get; set; }  // відповідає колонці "NAZEV"

        [Required]
        public decimal AktualniCena { get; set; }  // відповідає колонці "AKTUALNICENA"

        public decimal? CenaZeKlubKartou { get; set; }  // відповідає колонці "CENAZEKLUBKARTOU"

        [Required]
        public decimal Hmotnost { get; set; }  // відповідає колонці "HMOTNOST"

        [Required]
        [StringLength(100)]
        public string Slozeni { get; set; }  // відповідає колонці "SLOZENI"

        [Required]
        public int KategorieIdKategorii { get; set; }  // відповідає колонці "KATEGORIE_IDKATEGORII"

        // Навігаційна властивість для зв’язку з таблицею "KATEGORIE"
        [ForeignKey("KategorieIdKategorii")]
        public virtual Kategorie Kategorie { get; set; }
    }
}
