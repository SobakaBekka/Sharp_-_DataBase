using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineSupermarket.Controllers
{
    public class KategorieController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<KategorieController> _logger;

        public KategorieController(IConfiguration configuration, ILogger<KategorieController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Kategorie> kategorie = new List<Kategorie>();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("ZOBRAZ_VSE_KATEGORIE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                kategorie.Add(new Kategorie
                                {
                                    IdKategorie = reader.GetInt32(0),
                                    Nazev = reader.GetString(1),
                                    DatumVytvoreni = reader.GetDateTime(2),
                                    DatumAktualizace = reader.GetDateTime(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving categories: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving categories.");
            }

            return View(kategorie);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Kategorie kategorie)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (OracleConnection connection = new OracleConnection(_connectionString))
                    {
                        connection.Open();
                        using (OracleCommand command = new OracleCommand("VLOZ_KATEGORIE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = kategorie.Nazev;

                            command.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating category: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the category.");
                }
            }

            return View(kategorie);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Kategorie kategorie = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("SELECT IDKATEGORII, NAZEV, DATUM_VYTVORENI, DATUM_AKTUALIZACE FROM KATEGORIE WHERE IDKATEGORII = :id", connection))
                    {
                        command.Parameters.Add("id", OracleDbType.Int32).Value = id;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                kategorie = new Kategorie
                                {
                                    IdKategorie = reader.GetInt32(0),
                                    Nazev = reader.GetString(1),
                                    DatumVytvoreni = reader.GetDateTime(2),
                                    DatumAktualizace = reader.GetDateTime(3)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving category for edit: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving the category for edit.");
            }

            if (kategorie == null)
            {
                return NotFound();
            }

            return View(kategorie);
        }

        [HttpPost]
        public IActionResult Edit(Kategorie kategorie)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation($"Attempting to update category with ID {kategorie.IdKategorie} and name {kategorie.Nazev}.");

                    using (OracleConnection connection = new OracleConnection(_connectionString))
                    {
                        connection.Open();
                        using (OracleCommand command = new OracleCommand("AKTUALIZUJ_KATEGORIE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idkategorie", OracleDbType.Int32).Value = kategorie.IdKategorie;
                            command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = kategorie.Nazev;

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                _logger.LogInformation($"Category with ID {kategorie.IdKategorie} updated successfully.");
                            }
                            else
                            {
                                _logger.LogWarning($"No rows affected when updating category with ID {kategorie.IdKategorie}.");
                            }
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (OracleException ex)
                {
                    _logger.LogError($"Oracle error code: {ex.Number}, message: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An Oracle error occurred while updating the category.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating category: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the category.");
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid.");
            }

            return View(kategorie);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Kategorie kategorie = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("SELECT IDKATEGORII, NAZEV, DATUM_VYTVORENI, DATUM_AKTUALIZACE FROM KATEGORIE WHERE IDKATEGORII = :id", connection))
                    {
                        command.Parameters.Add("id", OracleDbType.Int32).Value = id;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                kategorie = new Kategorie
                                {
                                    IdKategorie = reader.GetInt32(0),
                                    Nazev = reader.GetString(1),
                                    DatumVytvoreni = reader.GetDateTime(2),
                                    DatumAktualizace = reader.GetDateTime(3)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving category for delete: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving the category for delete.");
            }

            if (kategorie == null)
            {
                return NotFound();
            }

            return View(kategorie);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("SMAZ_KATEGORIE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idkategorie", OracleDbType.Int32).Value = id;

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting category: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the category.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}