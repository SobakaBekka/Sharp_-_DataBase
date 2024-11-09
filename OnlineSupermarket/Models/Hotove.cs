using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Hotove
    {
        [Key]
        public int IdTranzakce { get; set; }  // відповідає колонці "IDTRANZAKCE"

        [Required]
        public decimal Vraceni { get; set; }  // відповідає колонці "VRACENI"

        // Навігаційна властивість для зв’язку з таблицею "PLATBA"
        [ForeignKey("IdTranzakce")]
        public virtual Platba Platba { get; set; }
    }
}
