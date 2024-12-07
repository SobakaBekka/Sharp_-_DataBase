using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OnlineSupermarket.Controllers
{
    public class PoziceController : Controller
    {
        private readonly string _connectionString;

        public PoziceController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
        }

        public IActionResult Index()
        {
            List<Pozice> poziceList = new List<Pozice>();

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("ZOBRAZ_VSE_POZICE", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            poziceList.Add(new Pozice
                            {
                                IdPozice = reader.GetInt32(0),
                                Nazev = reader.GetString(1),
                                Mzda = reader.GetDecimal(2),
                                DatumVytvoreni = reader.GetDateTime(3),
                                DatumAktualizace = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return View(poziceList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Pozice pozice)
        {
            if (ModelState.IsValid)
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("VLOZ_POZICI", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = pozice.Nazev;
                        command.Parameters.Add("p_mzda", OracleDbType.Decimal).Value = pozice.Mzda;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(pozice);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Pozice pozice = default;

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("SELECT IDPOZICE, NAZEV, MZDA, DATUM_VYTVORENI, DATUM_AKTUALIZACE FROM POZICE WHERE IDPOZICE = :id", connection))
                {
                    command.Parameters.Add("id", OracleDbType.Int32).Value = id;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pozice = new Pozice
                            {
                                IdPozice = reader.GetInt32(0),
                                Nazev = reader.GetString(1),
                                Mzda = reader.GetDecimal(2),
                                DatumVytvoreni = reader.GetDateTime(3),
                                DatumAktualizace = reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }

            if (pozice == null)
            {
                return NotFound();
            }

            return View(pozice);
        }

        [HttpPost]
        public IActionResult Edit(Pozice pozice)
        {
            if (ModelState.IsValid)
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("AKTUALIZUJ_POZICI", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = pozice.IdPozice;
                        command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = pozice.Nazev;
                        command.Parameters.Add("p_mzda", OracleDbType.Decimal).Value = pozice.Mzda;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(pozice);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("SMAZ_POZICI", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = id;

                    command.ExecuteNonQuery();
                }
            }

            return Json(new { success = true });
        }
    }
}