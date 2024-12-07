using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace OnlineSupermarket.Controllers
{
    public class SkladController : Controller
    {
        private readonly string _connectionString;

        public SkladController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            List<Sklad> sklady = new List<Sklad>();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand("ZOBRAZ_SKLAD", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sklady.Add(new Sklad
                            {
                                IdSkladu = reader.GetInt32(0),
                                NazevSkladu = reader.GetString(1),
                                PocetPolicek = reader.GetInt32(2),
                                Plocha = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                DatumVytvoreni = reader.GetDateTime(4),
                                DatumAktualizace = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }

            return View(sklady);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Sklad sklad)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new OracleCommand("VLOZ_SKLAD", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_nazevSkladu", OracleDbType.Varchar2).Value = sklad.NazevSkladu;
                        command.Parameters.Add("p_pocetPolicek", OracleDbType.Int32).Value = sklad.PocetPolicek;
                        command.Parameters.Add("p_plocha", OracleDbType.Int32).Value = sklad.Plocha;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(sklad);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Sklad sklad = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand("SELECT * FROM ST67136.SKLAD WHERE IDSKLADU = :id", connection))
                {
                    command.Parameters.Add("id", OracleDbType.Int32).Value = id;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sklad = new Sklad
                            {
                                IdSkladu = reader.GetInt32(0),
                                NazevSkladu = reader.GetString(1),
                                PocetPolicek = reader.GetInt32(2),
                                Plocha = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                DatumVytvoreni = reader.GetDateTime(4),
                                DatumAktualizace = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }

            if (sklad == null)
            {
                return NotFound();
            }

            return View(sklad);
        }

        [HttpPost]
        public IActionResult Edit(Sklad sklad)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new OracleCommand("AKTUALIZUJ_SKLAD", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idSkladu", OracleDbType.Int32).Value = sklad.IdSkladu;
                        command.Parameters.Add("p_nazevSkladu", OracleDbType.Varchar2).Value = sklad.NazevSkladu;
                        command.Parameters.Add("p_pocetPolicek", OracleDbType.Int32).Value = sklad.PocetPolicek;
                        command.Parameters.Add("p_plocha", OracleDbType.Int32).Value = sklad.Plocha;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(sklad);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (var command = new OracleCommand("SMAZ_SKLAD", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_idSkladu", OracleDbType.Int32).Value = id;

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}