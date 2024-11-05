namespace OnlineSupermarket.Models
{
    public class Prodej
    {
        public int IdProdeje { get; set; }
        public DateTime Datum { get; set; }
        public decimal CelkovaCena { get; set; }
        public int ZboziIdZbozi { get; set; }
    }
}
