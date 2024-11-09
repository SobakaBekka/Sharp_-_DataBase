using Microsoft.EntityFrameworkCore;

namespace OnlineSupermarket.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Встановлення назв таблиць
            modelBuilder.Entity<Adresa>().ToTable("ADRESA");
            modelBuilder.Entity<Hotove>().ToTable("HOTOVE");
            modelBuilder.Entity<Karta>().ToTable("KARTA");
            modelBuilder.Entity<Kategorie>().ToTable("KATEGORIE");
            modelBuilder.Entity<Kupon>().ToTable("KUPON");
            modelBuilder.Entity<LogDatabaz>().ToTable("LOGDATABAZ");
            modelBuilder.Entity<Platba>().ToTable("PLATBA");
            modelBuilder.Entity<Pokladna>().ToTable("POKLADNA");
            modelBuilder.Entity<Pozice>().ToTable("POZICE");
            modelBuilder.Entity<ProdaneZbozi>().ToTable("PRODANEZBOZI");
            modelBuilder.Entity<Prodej>().ToTable("PRODEJ");
            modelBuilder.Entity<Prodejna>().ToTable("PRODEJNA");
            modelBuilder.Entity<Pult>().ToTable("PULT");
            modelBuilder.Entity<RegisUzivatel>().ToTable("REGISUZIVATEL");
            modelBuilder.Entity<Role>().ToTable("ROLE");
            modelBuilder.Entity<Sklad>().ToTable("SKLAD");
            modelBuilder.Entity<Soubor>().ToTable("SOUBOR");
            modelBuilder.Entity<Zamestnanec>().ToTable("ZAMESTANEC");
            modelBuilder.Entity<Zbozi>().ToTable("ZBOZI");
            modelBuilder.Entity<ZboziNaPulte>().ToTable("ZBOZINAPULTE");
            modelBuilder.Entity<ZboziNaSklade>().ToTable("ZBOZINASKLADE");

            // Ваші налаштування зв’язків та комбінованих ключів

            modelBuilder.Entity<Adresa>()
                .HasOne(a => a.Prodejna)
                .WithMany()
                .HasForeignKey(a => a.ProdejnaIdProdejny)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Adresa>()
                .HasOne(a => a.Zamestnanec)
                .WithMany()
                .HasForeignKey(a => a.ZamestnanecIdZamestance)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Adresa>()
                .HasOne(a => a.Sklad)
                .WithMany()
                .HasForeignKey(a => a.SkladIdSkladu)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Platba>()
                .HasOne(p => p.Prodej)
                .WithMany()
                .HasForeignKey(p => new { p.ProdejIdProdeje, p.ProdejZboziIdZbozi })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdaneZbozi>()
                .HasKey(p => new { p.ZboziIdZbozi, p.ProdejIdProdeje });

            modelBuilder.Entity<ProdaneZbozi>()
                .HasOne(p => p.Zbozi)
                .WithMany()
                .HasForeignKey(p => p.ZboziIdZbozi)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdaneZbozi>()
                .HasOne(p => p.Prodej)
                .WithMany()
                .HasForeignKey(p => new { p.ProdejIdProdeje, p.ProdejZboziIdZbozi })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdaneZbozi>()
                .HasOne(p => p.Platba)
                .WithMany()
                .HasForeignKey(p => p.IdTranzakce)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prodej>()
                .HasKey(p => new { p.IdProdeje, p.ZboziIdZbozi });

            modelBuilder.Entity<Prodej>()
                .HasOne(p => p.Zbozi)
                .WithMany()
                .HasForeignKey(p => p.ZboziIdZbozi)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prodej>()
                .HasOne(p => p.Platba)
                .WithMany()
                .HasForeignKey(p => p.PlatbaIdTranzakce)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prodejna>()
                .HasOne(p => p.Pokladna)
                .WithMany()
                .HasForeignKey(p => p.PokladnaIdPokladny)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pult>()
                .HasOne(p => p.Prodejna)
                .WithMany()
                .HasForeignKey(p => p.ProdejnaIdProdejny)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RegisUzivatel>()
                .HasOne(r => r.Role)
                .WithMany()
                .HasForeignKey(r => r.RoleIdRole)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RegisUzivatel>()
                .HasOne(r => r.Soubor)
                .WithMany()
                .HasForeignKey(r => r.SouborIdSouboru)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Soubor>()
                .HasOne(s => s.RegisUzivatel)
                .WithMany()
                .HasForeignKey(s => s.RegisUzivatelIdRegisUzivatele)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zamestnanec>()
                .HasOne(z => z.Pozice)
                .WithMany()
                .HasForeignKey(z => z.PoziceIdPozice)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zamestnanec>()
                .HasOne(z => z.Prodejna)
                .WithMany()
                .HasForeignKey(z => z.ProdejnaIdProdejny)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zamestnanec>()
                .HasOne(z => z.Nadrazeny)
                .WithMany()
                .HasForeignKey(z => z.IdNadrezene)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zamestnanec>()
                .HasOne(z => z.Podrizeny)
                .WithMany()
                .HasForeignKey(z => z.ZamestnanecIdZamestance)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zamestnanec>()
                .HasOne(z => z.Podrizeny1)
                .WithMany()
                .HasForeignKey(z => z.ZamestnanecIdZamestance1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zbozi>()
                .HasOne(z => z.Kategorie)
                .WithMany()
                .HasForeignKey(z => z.KategorieIdKategorii)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZboziNaPulte>()
                .HasKey(zp => new { zp.ZboziIdZbozi, zp.PultIdPultu });

            modelBuilder.Entity<ZboziNaPulte>()
                .HasOne(zp => zp.Zbozi)
                .WithMany()
                .HasForeignKey(zp => zp.ZboziIdZbozi)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZboziNaPulte>()
                .HasOne(zp => zp.Pult)
                .WithMany()
                .HasForeignKey(zp => zp.PultIdPultu)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZboziNaSklade>()
                .HasKey(zs => new { zs.ZboziIdZbozi, zs.SkladIdSkladu });

            modelBuilder.Entity<ZboziNaSklade>()
                .HasOne(zs => zs.Zbozi)
                .WithMany()
                .HasForeignKey(zs => zs.ZboziIdZbozi)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZboziNaSklade>()
                .HasOne(zs => zs.Sklad)
                .WithMany()
                .HasForeignKey(zs => zs.SkladIdSkladu)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZboziNaSklade>()
                .HasOne(zs => zs.Adresa)
                .WithMany()
                .HasForeignKey(zs => zs.AdresaIdAdresy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ZboziNaSklade>()
                .HasOne(zs => zs.Kategorie)
                .WithMany()
                .HasForeignKey(zs => zs.KategorieIdKategorii)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // Ваші DbSet-и
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
        public DbSet<Pult> Pulty { get; set; }
        public DbSet<RegisUzivatel> RegisUzivatele { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Sklad> Sklady { get; set; }
        public DbSet<Soubor> Soubory { get; set; }
        public DbSet<Zamestnanec> Zamestanci { get; set; }
        public DbSet<Zbozi> Zbozi { get; set; }
        public DbSet<ZboziNaPulte> ZboziNaPulte { get; set; }
        public DbSet<ZboziNaSklade> ZboziNaSklade { get; set; }
    }
}
