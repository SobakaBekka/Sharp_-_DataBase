using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class RegisUzivatel
    {
        [Key]
        public int IdRegisUzivatele { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Jméno musí mít maximálně 30 znaků.")]
        public string Jmeno { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Příjmení musí mít maximálně 30 znaků.")]
        public string Prijmeni { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo hash musí mít maximálně 100 znaků.")]
        public string HesloHash { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo sůl musí mít maximálně 100 znaků.")]
        public string HesloSul { get; set; }

        [Required]
        public int RoleIdRole { get; set; }

        [Required]
        public int SouborIdSouboru { get; set; }

        [Required]
        public int IdSouboru { get; set; }
    }
}
