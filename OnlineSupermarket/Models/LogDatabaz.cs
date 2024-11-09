using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSupermarket.Models
{
    public class LogDatabaz
    {
        [Key]
        public int IdLogDatabaz { get; set; }  // відповідає колонці "IDLOGDATABAZ"

        [Required]
        [StringLength(50)]
        public string Tabulka { get; set; }  // відповідає колонці "TABULKA"

        [Required]
        [StringLength(100)]
        public string Operace { get; set; }  // відповідає колонці "OPERACE"

        [Required]
        public DateTime Datum { get; set; }  // відповідає колонці "DATUM"

        [Required]
        [StringLength(50)]
        public string Uzivatel { get; set; }  // відповідає колонці "UZIVATEL"

        [Required]
        [StringLength(50)]
        public string Zmeny { get; set; }  // відповідає колонці "ZMENY"
    }
}
