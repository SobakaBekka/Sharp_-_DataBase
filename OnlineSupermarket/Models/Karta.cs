using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Karta
    {
        [Key]
        public int IdTranzakce { get; set; }  // відповідає колонці "IDTRANZAKCE"

        [Required]
        public int AutorizacniKod { get; set; }  // відповідає колонці "AUTORIZACNIKOD"

        [Required]
        public int Cislo { get; set; }  // відповідає колонці "CISLO"

        // Навігаційна властивість для зв’язку з таблицею "PLATBA"
        [ForeignKey("IdTranzakce")]
        public virtual Platba Platba { get; set; }
    }
}
