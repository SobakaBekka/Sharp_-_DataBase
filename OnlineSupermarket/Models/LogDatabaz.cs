using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class LogDatabaz
    {
        [Key]
        public int IdLogDatabaz { get; set; }

        [Required]
        [StringLength(50)]
        public string Tabulka { get; set; }

        [Required]
        [StringLength(100)]
        public string Operace { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        [Required]
        [StringLength(50)]
        public string Uzivatel { get; set; }

        [Required]
        [StringLength(200)]
        public string Zmeny { get; set; }
    }
}
