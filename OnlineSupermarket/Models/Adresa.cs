namespace OnlineSupermarket.Models
{
    public class Adresa
    {
        public int IdAdresy { get; set; }
        public string Mesto { get; set; }
        public string Ulice { get; set; }
        public int ProdejnaIdProdejny { get; set; }
        public int ZamestnanecIdZamestnance { get; set; }
    }
}
