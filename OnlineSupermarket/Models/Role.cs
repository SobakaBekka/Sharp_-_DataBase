using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public enum RoleType
    {
        Administrator,
        RegistrovanyUzivatel,
        UzivatelBezPrav
    }

    public class Role
    {
        [Key]
        public int IdRole { get; set; }

        [Required]
        [StringLength(50)]
        public string Nazev { get; set; }

        [Required]
        public RoleType RoleType { get; set; }

        [StringLength(255)]
        public string? Popis { get; set; }

        [Required]
        public DateTime DatumVytvoreni { get; set; }

        [Required]
        public DateTime DatumAktualizace { get; set; }
    }
}