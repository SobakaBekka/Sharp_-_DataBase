using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class ProdaneZbozi
    {
        [Required]
        public int Pocet { get; set; }  // відповідає колонці "POCET"

        [Required]
        public decimal ProdejniCena { get; set; }  // відповідає колонці "PRODEJNICENA"

        [Key]
        [Column(Order = 1)]
        public int ZboziIdZbozi { get; set; }  // відповідає колонці "ZBOZI_IDZBOZI"

        [Key]
        [Column(Order = 2)]
        public int ProdejIdProdeje { get; set; }  // відповідає колонці "PRODEJ_IDPRODEJE"

        [Required]
        public int ProdejZboziIdZbozi { get; set; }  // відповідає колонці "PRODEJ_ZBOZI_IDZBOZI"

        [Required]
        public int IdTranzakce { get; set; }  // відповідає колонці "IDTRANZAKCE"

        // Навігаційні властивості
        [ForeignKey("ZboziIdZbozi")]
        public virtual Zbozi Zbozi { get; set; }

        [ForeignKey("ProdejIdProdeje, ProdejZboziIdZbozi")]
        public virtual Prodej Prodej { get; set; }

        [ForeignKey("IdTranzakce")]
        public virtual Platba Platba { get; set; }
    }
}
