using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class RegisUzivatel
    {
        [Key]
        public int IdRegisUzivatele { get; set; }  // відповідає колонці "IDREGISUZIVATELU"

        [Required]
        [StringLength(30)]
        public string Jmeno { get; set; }  // відповідає колонці "JMENO"

        [Required]
        [StringLength(30)]
        public string Prijmeni { get; set; }  // відповідає колонці "PRIJMENI"

        [Required]
        [StringLength(100)]
        public string HesloHash { get; set; }  // відповідає колонці "HESLOHASH"

        [Required]
        [StringLength(100)]
        public string HesloSul { get; set; }  // відповідає колонці "HESLOSUL"

        [Required]
        public int RoleIdRole { get; set; }  // відповідає колонці "ROLE_IDROLE"

        [Required]
        public int SouborIdSouboru { get; set; }  // відповідає колонці "SOUBOR_IDSOUBORU"

        [Required]
        public int IdSouboru { get; set; }  // відповідає колонці "IDSOUBORU"

        // Навігаційні властивості
        [ForeignKey("RoleIdRole")]
        public virtual Role Role { get; set; }

        [ForeignKey("SouborIdSouboru")]
        public virtual Soubor Soubor { get; set; }
    }
}
