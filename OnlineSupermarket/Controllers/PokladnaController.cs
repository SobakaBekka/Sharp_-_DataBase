using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace OnlineSupermarket.Controllers
{
    public class PokladnaController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<PokladnaController> _logger;

        public PokladnaController(IConfiguration configuration, ILogger<PokladnaController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Pokladna> pokladnaList = new List<Pokladna>();

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("ZOBRAZ_POKLADNU", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pokladnaList.Add(new Pokladna
                            {
                                IdPokladny = reader.GetInt32(0),
                                Samoobsluzna = reader.GetInt32(1),
                                Datum_Vytvoreni = reader.GetDateTime(2),
                                Datum_Aktualizace = reader.IsDBNull(3) ? default(DateTime) : reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }

            return View(pokladnaList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Pokladna pokladna)
        {
            if (ModelState.IsValid)
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("VLOZ_POKLADNU", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_SAMOOBSLUZNA", OracleDbType.Int32).Value = pokladna.Samoobsluzna;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(pokladna);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Pokladna? pokladna = null;

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("ZOBRAZ_POKLADNU_BY_ID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pokladna = new Pokladna
                            {
                                IdPokladny = reader.GetInt32(0),
                                Samoobsluzna = reader.GetInt32(1),
                                Datum_Vytvoreni = reader.GetDateTime(2),
                                Datum_Aktualizace = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3)
                            };
                        }
                    }
                }
            }

            if (pokladna == null)
            {
                return NotFound();
            }

            return View(pokladna);
        }

        [HttpPost]
        public IActionResult Edit(Pokladna pokladna)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (OracleConnection connection = new OracleConnection(_connectionString))
                    {
                        connection.Open();
                        using (OracleCommand command = new OracleCommand("AKTUALIZUJ_POKLADNU", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_IDPOKLADNY", OracleDbType.Int32).Value = pokladna.IdPokladny;
                            command.Parameters.Add("p_SAMOOBSLUZNA", OracleDbType.Int32).Value = pokladna.Samoobsluzna;

                            command.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (OracleException ex)
                {
                    if (ex.Number == 20001)
                    {
                        ModelState.AddModelError(string.Empty, "Pokladna nebyla nalezena.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while updating the record.");
                    }
                }
            }

            return View(pokladna);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_POKLADNU", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idpokladny", OracleDbType.Int32).Value = id;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении записи с IdPokladny: {IdPokladny}", id);
                return Json(new { success = false, message = $"Произошла ошибка при удалении записи: {ex.Message}" });
            }
        }

    }
}

