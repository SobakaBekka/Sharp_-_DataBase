using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Role
    {
        [Key]
        public int IdRole { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Název role musí mít maximálně 50 znaků.")]
        public string Nazev { get; set; }
    }
}
