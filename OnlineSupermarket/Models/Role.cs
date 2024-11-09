using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class Role
    {
        [Key]
        public int IdRole { get; set; }  // відповідає колонці "IDROLE"

        [Required]
        [StringLength(50)]
        public string Nazev { get; set; }  // відповідає колонці "NAZEV"
    }
}
