namespace OnlineSupermarket.Models
{
    public class ZboziKategorieViewModel
    {
        public int IDZbozi { get; set; }
        public string ZboziNazev { get; set; }
        public decimal AktualniCena { get; set; }
        public decimal? CenaZeKlubKartou { get; set; }
        public decimal Hmotnost { get; set; }
        public string Slozeni { get; set; }
        public string KategorieNazev { get; set; }
        public string ImageFileName { get; set; }
    }
}