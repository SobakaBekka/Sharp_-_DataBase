using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class ZboziNaPulte
    {
        [Required]
        public int Pocet { get; set; }  // відповідає колонці "POCET"

        [Key]
        [Column(Order = 1)]
        public int ZboziIdZbozi { get; set; }  // відповідає колонці "ZBOZI_IDZBOZI"

        [Key]
        [Column(Order = 2)]
        public int PultIdPultu { get; set; }  // відповідає колонці "PULT_IDPULTU"

        [Required]
        public int IdPultu { get; set; }  // відповідає колонці "IDPULTU"

        // Навігаційні властивості
        [ForeignKey("ZboziIdZbozi")]
        public virtual Zbozi Zbozi { get; set; }

        [ForeignKey("PultIdPultu")]
        public virtual Pult Pult { get; set; }
    }
}
