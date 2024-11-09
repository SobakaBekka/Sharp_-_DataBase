using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class ZboziNaSklade
    {
        [Required]
        public int Pocet { get; set; }  // відповідає колонці "POCET"

        [Key]
        [Column(Order = 1)]
        public int ZboziIdZbozi { get; set; }  // відповідає колонці "ZBOZI_IDZBOZI"

        [Key]
        [Column(Order = 2)]
        public int SkladIdSkladu { get; set; }  // відповідає колонці "SKLAD_IDSKLADU"

        public int? AdresaIdAdresy { get; set; }  // відповідає колонці "ADRESA_IDADRESY"

        [Required]
        public int KategorieIdKategorii { get; set; }  // відповідає колонці "KATEGORIE_IDKATEGORII"

        // Навігаційні властивості
        [ForeignKey("ZboziIdZbozi")]
        public virtual Zbozi Zbozi { get; set; }

        [ForeignKey("SkladIdSkladu")]
        public virtual Sklad Sklad { get; set; }

        [ForeignKey("AdresaIdAdresy")]
        public virtual Adresa Adresa { get; set; }

        [ForeignKey("KategorieIdKategorii")]
        public virtual Kategorie Kategorie { get; set; }
    }
}
