using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace OnlineSupermarket.Models
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Method to execute a non-query SQL command (INSERT, UPDATE, DELETE)
        public int ExecuteNonQuery(string sql, OracleParameter[] parameters = null)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                using (var command = new OracleCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    // Если есть выходные параметры, обновите их значения
                    foreach (OracleParameter param in command.Parameters)
                    {
                        if (param.Direction == ParameterDirection.Output)
                        {
                            param.Value = command.Parameters[param.ParameterName].Value;
                        }
                    }

                    return result;
                }
            }
            catch (OracleException ex)
            {
                // Логирование ошибки
                throw; // Повторно выбросить исключение для обработки в контроллере
            }
        }


        // Method to execute a scalar SQL command (e.g., SELECT COUNT(*))
        public object ExecuteScalar(string sql, OracleParameter[] parameters = null)
        {
            using (var connection = new OracleConnection(_connectionString))
            using (var command = new OracleCommand(sql, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                connection.Open();
                return command.ExecuteScalar();
            }
        }

        // Method to execute a SQL query and return a DataTable
        public DataTable ExecuteQuery(string sql, OracleParameter[] parameters = null)
        {
            using (var connection = new OracleConnection(_connectionString))
            using (var command = new OracleCommand(sql, connection))
            using (var adapter = new OracleDataAdapter(command))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        // **MapAdresa Method**
        public List<Adresa> MapAdresa(DataTable dataTable)
        {
            var adresaList = new List<Adresa>();

            foreach (DataRow row in dataTable.Rows)
            {
                var adresa = new Adresa
                {
                    IdAdresy = Convert.ToInt32(row["IDADRESY"]),
                    Mesto = row["MESTO"].ToString(),
                    Ulice = row["ULICE"].ToString(),
                    ProdejnaIdProdejny = row["PRODEJNA_IDPRODEJNY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["PRODEJNA_IDPRODEJNY"]),
                    ZamestnanecIdZamestance = row["ZAMESTANEC_IDZAMESTANCE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ZAMESTANEC_IDZAMESTANCE"]),
                    SkladIdSkladu = row["SKLAD_IDSKLADU"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["SKLAD_IDSKLADU"])
                };

                adresaList.Add(adresa);
            }

            return adresaList;
        }

        public List<Hotove> MapHotove(DataTable dataTable)
        {
            var hotoveList = new List<Hotove>();

            foreach (DataRow row in dataTable.Rows)
            {
                var hotove = new Hotove
                {
                    IdTranzakce = Convert.ToInt32(row["IDTRANZAKCE"]),
                    Vraceni = Convert.ToDecimal(row["VRACENI"])
                };

                hotoveList.Add(hotove);
            }

            return hotoveList;
        }
        public List<Karta> MapKarta(DataTable dataTable)
        {
            var kartaList = new List<Karta>();

            foreach (DataRow row in dataTable.Rows)
            {
                var karta = new Karta
                {
                    IdTranzakce = Convert.ToInt32(row["IDTRANZAKCE"]),
                    AutorizacniKod = Convert.ToInt32(row["AUTORIZACNIKOD"]),
                    Cislo = Convert.ToInt32(row["CISLO"])
                };

                kartaList.Add(karta);
            }

            return kartaList;
        }
        public List<Kategorie> MapKategorie(DataTable dataTable)
        {
            var kategorieList = new List<Kategorie>();

            foreach (DataRow row in dataTable.Rows)
            {
                var kategorie = new Kategorie
                {
                    IdKategorii = Convert.ToInt32(row["IDKATEGORII"]),
                    Nazev = row["NAZEV"].ToString()
                };

                kategorieList.Add(kategorie);
            }

            return kategorieList;
        }
        public List<Kupon> MapKupon(DataTable dataTable)
        {
            var kuponList = new List<Kupon>();

            foreach (DataRow row in dataTable.Rows)
            {
                var kupon = new Kupon
                {
                    IdTranzakce = Convert.ToInt32(row["IDTRANZAKCE"]),
                    Cislo = row["CISLO"].ToString()
                };

                kuponList.Add(kupon);
            }

            return kuponList;
        }
        public List<LogDatabaz> MapLogDatabaz(DataTable dataTable)
        {
            var logList = new List<LogDatabaz>();

            foreach (DataRow row in dataTable.Rows)
            {
                var log = new LogDatabaz
                {
                    IdLogDatabaz = Convert.ToInt32(row["IDLOGDATABAZ"]),
                    Tabulka = row["TABULKA"].ToString(),
                    Operace = row["OPERACE"].ToString(),
                    Datum = Convert.ToDateTime(row["DATUM"]),
                    Uzivatel = row["UZIVATEL"].ToString(),
                    Zmeny = row["ZMENY"].ToString()
                };

                logList.Add(log);
            }

            return logList;
        }
        public List<Platba> MapPlatba(DataTable dataTable)
        {
            var platbaList = new List<Platba>();

            foreach (DataRow row in dataTable.Rows)
            {
                var platba = new Platba
                {
                    IdTranzakce = Convert.ToInt32(row["IDTRANZAKCE"]),
                    CelkovaCena = Convert.ToDecimal(row["CELKOVACENA"]),
                    ProdejIdProdeje = Convert.ToInt32(row["PRODEJ_IDPRODEJE"]),
                    ProdejZboziIdZbozi = Convert.ToInt32(row["PRODEJ_ZBOZI_IDZBOZI"]),
                    Typ = row["TYP"].ToString()
                };

                platbaList.Add(platba);
            }

            return platbaList;
        }
        public List<Pokladna> MapPokladna(DataTable dataTable)
        {
            var pokladnaList = new List<Pokladna>();

            foreach (DataRow row in dataTable.Rows)
            {
                var pokladna = new Pokladna
                {
                    IdPokladny = Convert.ToInt32(row["IDPOKLADNY"]),
                    Samoobsluzna = Convert.ToInt32(row["SAMOOBSLUZNA"]) == 1
                };

                pokladnaList.Add(pokladna);
            }

            return pokladnaList;
        }
        public List<Pozice> MapPozice(DataTable dataTable)
        {
            var poziceList = new List<Pozice>();

            foreach (DataRow row in dataTable.Rows)
            {
                var pozice = new Pozice
                {
                    IdPozice = Convert.ToInt32(row["IDPOZICE"]),
                    Nazev = row["NAZEV"].ToString(),
                    Mzda = Convert.ToDecimal(row["MZDA"])
                };

                poziceList.Add(pozice);
            }

            return poziceList;
        }
        public List<ProdaneZbozi> MapProdaneZbozi(DataTable dataTable)
        {
            var prodaneZboziList = new List<ProdaneZbozi>();

            foreach (DataRow row in dataTable.Rows)
            {
                var prodaneZbozi = new ProdaneZbozi
                {
                    Pocet = Convert.ToInt32(row["POCET"]),
                    ProdejniCena = Convert.ToDecimal(row["PRODEJNICENA"]),
                    ZboziIdZbozi = Convert.ToInt32(row["ZBOZI_IDZBOZI"]),
                    ProdejIdProdeje = Convert.ToInt32(row["PRODEJ_IDPRODEJE"]),
                    ProdejZboziIdZbozi = Convert.ToInt32(row["PRODEJ_ZBOZI_IDZBOZI"]),
                    IdTranzakce = Convert.ToInt32(row["IDTRANZAKCE"])
                };

                prodaneZboziList.Add(prodaneZbozi);
            }

            return prodaneZboziList;
        }
        public List<Prodej> MapProdej(DataTable dataTable)
        {
            var prodejList = new List<Prodej>();

            foreach (DataRow row in dataTable.Rows)
            {
                var prodej = new Prodej
                {
                    IdProdeje = Convert.ToInt32(row["IDPRODEJE"]),
                    Datum = Convert.ToDateTime(row["DATUM"]),
                    CelkovaCena = Convert.ToDecimal(row["CELKOVACENA"]),
                    ZboziIdZbozi = Convert.ToInt32(row["ZBOZI_IDZBOZI"]),
                    PlatbaIdTranzakce = row["PLATBA_IDTRANZAKCE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["PLATBA_IDTRANZAKCE"])
                };

                prodejList.Add(prodej);
            }

            return prodejList;
        }
        public List<Prodejna> MapProdejna(DataTable dataTable)
        {
            var prodejnaList = new List<Prodejna>();

            foreach (DataRow row in dataTable.Rows)
            {
                var prodejna = new Prodejna
                {
                    IdProdejny = Convert.ToInt32(row["IDPRODEJNY"]),
                    KontaktniCislo = row["KONTAKTNICISLO"].ToString(), // Convert int to string
                    Plocha = Convert.ToDecimal(row["PLOCHA"]),
                    PokladnaIdPokladny = row["POKLADNA_IDPOKLADNY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["POKLADNA_IDPOKLADNY"])
                };

                prodejnaList.Add(prodejna);
            }

            return prodejnaList;
        }
        public List<Pult> MapPult(DataTable dataTable)
        {
            var pultList = new List<Pult>();

            foreach (DataRow row in dataTable.Rows)
            {
                var pult = new Pult
                {
                    IdPultu = Convert.ToInt32(row["IDPULTU"]),
                    Cislo = row["CISLO"].ToString(),
                    PocetPoicek = Convert.ToInt32(row["POCETPOICEK"]),
                    ProdejnaIdProdejny = row["PRODEJNA_IDPRODEJNY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["PRODEJNA_IDPRODEJNY"]),
                    IdKategorii = row["IDKATEGORII"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["IDKATEGORII"])
                };

                pultList.Add(pult);
            }

            return pultList;
        }
        public List<RegisUzivatel> MapRegisUzivatel(DataTable dataTable)
        {
            var regisUzivatelList = new List<RegisUzivatel>();

            foreach (DataRow row in dataTable.Rows)
            {
                var regisUzivatel = new RegisUzivatel
                {
                    IdRegisUzivatele = Convert.ToInt32(row["IDREGISUZIVATELU"]),
                    Jmeno = row["JMENO"].ToString(),
                    Prijmeni = row["PRIJMENI"].ToString(),
                    HesloHash = row["HESLOHASH"].ToString(),
                    HesloSul = row["HESLOSUL"].ToString(),
                    RoleIdRole = Convert.ToInt32(row["ROLE_IDROLE"]),
                    SouborIdSouboru = Convert.ToInt32(row["SOUBOR_IDSOUBORU"]),
                    IdSouboru = Convert.ToInt32(row["IDSOUBORU"])
                };

                regisUzivatelList.Add(regisUzivatel);
            }

            return regisUzivatelList;
        }

        public List<Role> MapRole(DataTable dataTable)
        {
            var roleList = new List<Role>();

            foreach (DataRow row in dataTable.Rows)
            {
                var role = new Role
                {
                    IdRole = Convert.ToInt32(row["IDROLE"]),
                    Nazev = row["NAZEV"].ToString()
                };

                roleList.Add(role);
            }

            return roleList;
        }
        public List<Sklad> MapSklad(DataTable dataTable)
        {
            var skladList = new List<Sklad>();

            foreach (DataRow row in dataTable.Rows)
            {
                var sklad = new Sklad
                {
                    IdSkladu = Convert.ToInt32(row["IDSKLADU"]),
                    PocetPolicek = Convert.ToInt32(row["POCETPOLICEK"]),
                    Plocha = row["PLOCHA"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["PLOCHA"])
                };

                skladList.Add(sklad);
            }

            return skladList;
        }
        public List<Soubor> MapSoubor(DataTable dataTable)
        {
            var souborList = new List<Soubor>();

            foreach (DataRow row in dataTable.Rows)
            {
                var soubor = new Soubor
                {
                    IdSouboru = Convert.ToInt32(row["IDSOUBORU"]),
                    Nazev = row["NAZEV"].ToString(),
                    SouborContent = (byte[])row["SOUBOR"],
                    Pripona = row["PRIPONA"].ToString(),
                    UploadDate = Convert.ToDateTime(row["UPLOADDATE"]),
                    ModifyDate = Convert.ToDateTime(row["MODIFYDATE"]),
                    TypSouboru = row["TYPSOUBORU"].ToString(),
                    RegisUzivatelId = Convert.ToInt32(row["REGISUZIVATEL_IDREGISUZIVATELU"])
                };

                souborList.Add(soubor);
            }

            return souborList;
        }
        public List<Zamestnanec> MapZamestnanec(DataTable dataTable)
        {
            var zamestnanecList = new List<Zamestnanec>();

            foreach (DataRow row in dataTable.Rows)
            {
                var zamestnanec = new Zamestnanec
                {
                    IdZamestance = Convert.ToInt32(row["IDZAMESTANCE"]),
                    Jmeno = row["JMENO"].ToString(),
                    Prijmeni = row["PRIJMENI"].ToString(),
                    RodneCislo = row["RODNECISLO"].ToString(),
                    TelefonniCislo = row["TELEFONICISLO"].ToString(),
                    PoziceIdPozice = Convert.ToInt32(row["POZICE_IDPOZICE"]),
                    ProdejnaIdProdejny = Convert.ToInt32(row["PRODEJNA_IDPRODEJNY"]),
                    IdNadrezene = row["IDNADREZENE"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["IDNADREZENE"])
                };

                zamestnanecList.Add(zamestnanec);
            }

            return zamestnanecList;
        }
        public List<Zbozi> MapZbozi(DataTable dataTable)
        {
            var zboziList = new List<Zbozi>();

            foreach (DataRow row in dataTable.Rows)
            {
                var zbozi = new Zbozi
                {
                    IdZbozi = Convert.ToInt32(row["IDZBOZI"]),
                    Nazev = row["NAZEV"].ToString(),
                    AktualniCena = Convert.ToDecimal(row["AKTUALNICENA"]),
                    CenaZeKlubKartou = row["CENAZEKLUBKARTOU"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["CENAZEKLUBKARTOU"]),
                    Hmotnost = Convert.ToDecimal(row["HMOTNOST"]),
                    Slozeni = row["SLOZENI"].ToString(),
                    KategorieIdKategorii = Convert.ToInt32(row["KATEGORIE_IDKATEGORII"])
                };

                zboziList.Add(zbozi);
            }

            return zboziList;
        }
        public List<ZboziNaPulte> MapZboziNaPulte(DataTable dataTable)
        {
            var zboziNaPulteList = new List<ZboziNaPulte>();

            foreach (DataRow row in dataTable.Rows)
            {
                var zboziNaPulte = new ZboziNaPulte
                {
                    PultIdPultu = Convert.ToInt32(row["PULT_IDPULTU"]),
                    ZboziIdZbozi = Convert.ToInt32(row["ZBOZI_IDZBOZI"]),
                    Pocet = Convert.ToInt32(row["POCET"])
                };

                zboziNaPulteList.Add(zboziNaPulte);
            }

            return zboziNaPulteList;
        }
        public List<ZboziNaSklade> MapZboziNaSklade(DataTable dataTable)
        {
            var zboziNaSkladeList = new List<ZboziNaSklade>();

            foreach (DataRow row in dataTable.Rows)
            {
                var zboziNaSklade = new ZboziNaSklade
                {
                    ZboziIdZbozi = Convert.ToInt32(row["ZBOZI_IDZBOZI"]),
                    SkladIdSkladu = Convert.ToInt32(row["SKLAD_IDSKLADU"]),
                    Pocet = Convert.ToInt32(row["POCET"]),
                    AdresaIdAdresy = row["ADRESA_IDADRESY"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ADRESA_IDADRESY"]),
                    KategorieIdKategorii = Convert.ToInt32(row["KATEGORIE_IDKATEGORII"])
                };

                zboziNaSkladeList.Add(zboziNaSklade);
            }

            return zboziNaSkladeList;
        }
    }
}