using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Prodej
    {
        [Key]
        [Column(Order = 1)]
        public int IdProdeje { get; set; }  // відповідає колонці "IDPRODEJE"

        [Required]
        public DateTime Datum { get; set; }  // відповідає колонці "DATUM"

        [Required]
        public decimal CelkovaCena { get; set; }  // відповідає колонці "CELKOVACENA"

        [Key]
        [Column(Order = 2)]
        public int ZboziIdZbozi { get; set; }  // відповідає колонці "ZBOZI_IDZBOZI"

        [Required]
        public int PlatbaIdTranzakce { get; set; }  // відповідає колонці "PLATBA_IDTRANZAKCE"

        // Навігаційні властивості
        [ForeignKey("ZboziIdZbozi")]
        public virtual Zbozi Zbozi { get; set; }

        [ForeignKey("PlatbaIdTranzakce")]
        public virtual Platba Platba { get; set; }
    }
}
