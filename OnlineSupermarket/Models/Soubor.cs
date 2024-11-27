using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Soubor
    {
        [Key]
        public int IdSouboru { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Název musí mít maximálně 100 znaků.")]
        public string Nazev { get; set; }

        [Required]
        public byte[] SouborContent { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Přípona musí mít maximálně 10 znaků.")]
        public string Pripona { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Typ souboru musí mít maximálně 50 znaků.")]
        public string TypSouboru { get; set; }

        [Required]
        public int RegisUzivatelId { get; set; }
    }
}
