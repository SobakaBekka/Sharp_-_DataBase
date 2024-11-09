using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Kupon
    {
        [Key]
        public int IdTranzakce { get; set; }  // відповідає колонці "IDTRANZAKCE"

        [Required]
        public int Cislo { get; set; }  // відповідає колонці "CISLO"

        // Навігаційна властивість для зв’язку з таблицею "PLATBA"
        [ForeignKey("IdTranzakce")]
        public virtual Platba Platba { get; set; }
    }
}
