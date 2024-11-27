using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class RegisUzivatel
    {
        public int IdRegisUzivatele { get; set; }

        [Required]
        [MaxLength(30)]
        public string Jmeno { get; set; }

        [Required]
        [MaxLength(30)]
        public string Prijmeni { get; set; }

        [Required]
        [MaxLength(100)]
        public string HesloHash { get; set; }

        [Required]
        [MaxLength(100)]
        public string HesloSul { get; set; }

        [Required]
        public int RoleIdRole { get; set; }

        [Required]
        public int SouborIdSouboru { get; set; }

        [Required]
        public int IdSouboru { get; set; }
    }
}