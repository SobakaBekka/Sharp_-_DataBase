namespace OnlineSupermarket.Models
{
    public class RegisUzivatel
    {
        public int IdRegisUzivatelu { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string HesloHash { get; set; }
        public string HesloSul { get; set; }
        public int IdRole { get; set; }
        public int IdSouboru { get; set; }

        // Навігаційна властивість
        public Role Role { get; set; }
    }
}
