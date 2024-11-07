using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Zbozi
    {
        [Key]
        public int IdZbozi { get; set; }
        public string Nazev { get; set; }
        public decimal AktualniCena { get; set; }
        public decimal CenaZeKlubKartou { get; set; }
        public decimal Hmotnost { get; set; }
        public string Slozeni { get; set; }
        public int KategorieIdKategorii { get; set; }
        // Додана властивість Ціна
        public decimal Cena { get; set; }
        // Ідентифікатор файлу зображення
        public int? IdSoubor { get; set; }

        // Навігаційна властивість
        public Soubor Soubor { get; set; }
    }
}
