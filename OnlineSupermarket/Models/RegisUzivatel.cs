using System;

namespace OnlineSupermarket.Models
{
    public class RegisUzivatel
    {
        public int IdRegisUzivatelu { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string Heslo { get; set; }
        public DateTime DatumVytvoreni { get; set; }
        public DateTime DatumAktualizace { get; set; }
        public DateTime? PosledniPrihlaseni { get; set; }
        public int NeuspesnePrihlaseni { get; set; }
        public int RoleIdRole { get; set; }
        public string Rolenazev { get; set; }
    }
}