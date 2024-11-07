using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Role
    {
        [Key]
        public int IdRole { get; set; }
        public string Nazev { get; set; }
    }
}
