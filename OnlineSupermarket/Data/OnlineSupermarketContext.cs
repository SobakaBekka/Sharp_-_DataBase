using Microsoft.EntityFrameworkCore;
using OnlineSupermarket.Models;

namespace OnlineSupermarket.Data
{
    public class OnlineSupermarketContext : DbContext
    {
        public OnlineSupermarketContext(DbContextOptions<OnlineSupermarketContext> options)
            : base(options)
        {
        }

        public DbSet<Adresa> Adresy { get; set; }
        public DbSet<Hotove> Hotove { get; set; }
        public DbSet<Karta> Karty { get; set; }
        public DbSet<Kategorie> Kategorie { get; set; }
        public DbSet<Kupon> Kupony { get; set; }
        public DbSet<LogDatabaz> LogDatabaze { get; set; }
        public DbSet<Platba> Platby { get; set; }
        public DbSet<Pokladna> Pokladny { get; set; }
        public DbSet<Pozice> Pozice { get; set; }
        public DbSet<ProdaneZbozi> ProdaneZbozi { get; set; }
        public DbSet<Prodej> Prodeje { get; set; }
        public DbSet<Prodejna> Prodejny { get; set; }
        public DbSet<ProdejnaPokladna> ProdejnaPokladny { get; set; }
        public DbSet<Pult> Pulty { get; set; }
        public DbSet<RegisUzivatel> RegisUzivatele { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Sklad> Sklady { get; set; }
        public DbSet<Soubor> Soubory { get; set; } // Використовуємо клас Soubor
        public DbSet<Zamestnanec> Zamestnanci { get; set; }
        public DbSet<Zbozi> Zbozi { get; set; }
        public DbSet<ZboziNaPulte> ZboziNaPulte { get; set; }
        public DbSet<ZboziNaSklade> ZboziNaSklade { get; set; }
        public DbSet<UserAuditLog> UserAuditLogs { get; set; }
        public DbSet<PasswordChangeLog> PasswordChangeLogs { get; set; }
        public DbSet<SalaryResult> SalaryResults { get; set; }
    }
}