using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Zamestnanec
    {
        [Key]
        public int IdZamestnance { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public DateTime RodneCislo { get; set; }
        public int TelefonniCislo { get; set; }
        public int PoziceIdPozice { get; set; }
        public int ProdejnaIdProdejny { get; set; }
        public int IdNadrezene { get; set; }
    }
}
