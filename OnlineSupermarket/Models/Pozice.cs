namespace OnlineSupermarket.Models
{
    public class Pozice
    {
        public int IdPozice { get; set; }
        public string Nazev { get; set; }
        public decimal Mzda { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
    }
}