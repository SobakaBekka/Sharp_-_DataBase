namespace OnlineSupermarket.Models
{
    public class Pult
    {
        public int IdPultu { get; set; }
        public string Cislo { get; set; }
        public int PocetPolicek { get; set; }
        public int? ProdejnaIdProdejny { get; set; }
        public int? IdKategorie { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
    }
}