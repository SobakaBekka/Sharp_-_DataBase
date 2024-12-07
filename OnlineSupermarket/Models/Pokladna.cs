namespace OnlineSupermarket.Models
{
    public class Pokladna
    {
        public int IdPokladny { get; set; }
        public int Samoobsluzna { get; set; }
        public DateTime Datum_Vytvoreni { get; set; }
        public DateTime? Datum_Aktualizace { get; set; }
    }
}