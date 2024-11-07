using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class LogDatabaz
    {
        [Key]
        public int IdLogDatabaz { get; set; }
        public string Tabulka { get; set; }
        public string Operace { get; set; }
        public DateTime Datum { get; set; }
        public string Uzivatel { get; set; }
        public string Zmeny { get; set; }
    }
}
