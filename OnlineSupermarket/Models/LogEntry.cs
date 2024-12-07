namespace OnlineSupermarket.Models
{
    public class LogEntry
    {
        public int IdLogDatabaz { get; set; }
        public required string Tabulka { get; set; }
        public required string Operace { get; set; }
        public DateTime Datum { get; set; }
        public required string Uzivatel { get; set; }
        public required string Zmeny { get; set; }
    }
}