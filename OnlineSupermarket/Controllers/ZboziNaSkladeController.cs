using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using OnlineSupermarket.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineSupermarket.Controllers
{
    public class ZboziNaSkladeController : Controller
    {
        private readonly string _connectionString;

        public ZboziNaSkladeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
        }

        public IActionResult Index()
        {
            List<ZboziNaSklade> zboziNaSkladeList = new List<ZboziNaSklade>();

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("ZOBRAZ_ZBOZINASKLADE", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            zboziNaSkladeList.Add(new ZboziNaSklade
                            {
                                Pocet = reader.GetInt32(0),
                                ZboziIdZbozi = reader.GetInt32(1),
                                SkladIdSkladu = reader.GetInt32(2),
                                AdresaIdAdresy = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                KategorieIdKategorii = reader.GetInt32(4),
                                DatumVytvoreni = reader.GetDateTime(5),
                                DatumAktualizace = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }

            return View(zboziNaSkladeList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ZboziNaSklade zboziNaSklade)
        {
            if (ModelState.IsValid)
            {
                // Перевірка числових значень
                if (!int.TryParse(zboziNaSklade.ZboziIdZbozi.ToString(), out int zboziIdZbozi) ||
                    !int.TryParse(zboziNaSklade.SkladIdSkladu.ToString(), out int skladIdSkladu) ||
                    !int.TryParse(zboziNaSklade.Pocet.ToString(), out int pocet) ||
                    !int.TryParse(zboziNaSklade.KategorieIdKategorii.ToString(), out int kategorieIdKategorii))
                {
                    ModelState.AddModelError("", "Всі значення повинні бути числовими.");
                    return View(zboziNaSklade);
                }

                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("VLOZ_ZBOZINASKLADE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziIdZbozi;
                        command.Parameters.Add("p_sklad_idskladu", OracleDbType.Int32).Value = skladIdSkladu;
                        command.Parameters.Add("p_pocet", OracleDbType.Int32).Value = pocet;
                        command.Parameters.Add("p_adresa_idadresy", OracleDbType.Int32).Value = zboziNaSklade.AdresaIdAdresy ?? (object)DBNull.Value;
                        command.Parameters.Add("p_kategorie_idkategorii", OracleDbType.Int32).Value = kategorieIdKategorii;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(zboziNaSklade);
        }

        [HttpGet]
        public IActionResult Edit(int zboziIdZbozi, int skladIdSkladu)
        {
            ZboziNaSklade zboziNaSklade = null;

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("ZOBRAZ_ZBOZINASKLADE", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetInt32(1) == zboziIdZbozi && reader.GetInt32(2) == skladIdSkladu)
                            {
                                zboziNaSklade = new ZboziNaSklade
                                {
                                    Pocet = reader.GetInt32(0),
                                    ZboziIdZbozi = reader.GetInt32(1),
                                    SkladIdSkladu = reader.GetInt32(2),
                                    AdresaIdAdresy = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                    KategorieIdKategorii = reader.GetInt32(4),
                                    DatumVytvoreni = reader.GetDateTime(5),
                                    DatumAktualizace = reader.GetDateTime(6)
                                };
                                break;
                            }
                        }
                    }
                }
            }

            if (zboziNaSklade == null)
            {
                return NotFound();
            }

            return View(zboziNaSklade);
        }

        [HttpPost]
        public IActionResult Edit(ZboziNaSklade zboziNaSklade)
        {
            if (ModelState.IsValid)
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("AKTUALIZUJ_ZBOZINASKLADE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziNaSklade.ZboziIdZbozi;
                        command.Parameters.Add("p_sklad_idskladu", OracleDbType.Int32).Value = zboziNaSklade.SkladIdSkladu;
                        command.Parameters.Add("p_pocet", OracleDbType.Int32).Value = zboziNaSklade.Pocet;
                        command.Parameters.Add("p_adresa_idadresy", OracleDbType.Int32).Value = zboziNaSklade.AdresaIdAdresy ?? (object)DBNull.Value;
                        command.Parameters.Add("p_kategorie_idkategorii", OracleDbType.Int32).Value = zboziNaSklade.KategorieIdKategorii;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(zboziNaSklade);
        }

        [HttpGet]
        public IActionResult Delete(int zboziIdZbozi, int skladIdSkladu)
        {
            ZboziNaSklade zboziNaSklade = null;

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("ZOBRAZ_ZBOZINASKLADE", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetInt32(1) == zboziIdZbozi && reader.GetInt32(2) == skladIdSkladu)
                            {
                                zboziNaSklade = new ZboziNaSklade
                                {
                                    Pocet = reader.GetInt32(0),
                                    ZboziIdZbozi = reader.GetInt32(1),
                                    SkladIdSkladu = reader.GetInt32(2),
                                    AdresaIdAdresy = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                    KategorieIdKategorii = reader.GetInt32(4),
                                    DatumVytvoreni = reader.GetDateTime(5),
                                    DatumAktualizace = reader.GetDateTime(6)
                                };
                                break;
                            }
                        }
                    }
                }
            }

            if (zboziNaSklade == null)
            {
                return NotFound();
            }

            return View(zboziNaSklade);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int zboziIdZbozi, int skladIdSkladu)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("SMAZ_ZBOZINASKLADE", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziIdZbozi;
                    command.Parameters.Add("p_sklad_idskladu", OracleDbType.Int32).Value = skladIdSkladu;

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }
    }
}