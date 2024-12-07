using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;

namespace OnlineSupermarket.Controllers
{
    public class ZamestanecController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<ZamestanecController> _logger;

        public ZamestanecController(IConfiguration configuration, ILogger<ZamestanecController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;

        }
        // Метод для отображения списка сотрудников
        public async Task<IActionResult> Index()
        {
            var zamestanecList = new List<Zamestanec>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ZAMESTANCE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                zamestanecList.Add(new Zamestanec
                                {
                                    IdZamestance = reader.GetInt32(0),
                                    Jmeno = reader.GetString(1),
                                    Prijmeni = reader.GetString(2),
                                    RodneCislo = reader.GetString(3),
                                    TelefonniCislo = reader.GetString(4),
                                    PoziceIdPozice = reader.GetInt32(5),
                                    ProdejnaIdProdejny = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                    IdNadrezene = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                                    DatumVytvoreni = reader.GetDateTime(8),
                                    DatumAktualizace = reader.GetDateTime(9)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении списка сотрудников");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка сотрудников: {ex.Message}");
            }

            return View(zamestanecList);
        }



        // Метод для отображения формы создания сотрудника
        public IActionResult Create()
        {
            return View();
        }

        // Метод для обработки создания сотрудника
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Zamestanec zamestanec)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    zamestanec.DatumVytvoreni = DateTime.UtcNow;
                    zamestanec.DatumAktualizace = DateTime.UtcNow;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_ZAMESTANCE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = zamestanec.Jmeno;
                            command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = zamestanec.Prijmeni;
                            command.Parameters.Add("p_rodnecislo", OracleDbType.Varchar2).Value = zamestanec.RodneCislo;
                            command.Parameters.Add("p_telefonicislo", OracleDbType.Varchar2).Value = zamestanec.TelefonniCislo;
                            command.Parameters.Add("p_pozice_idpozice", OracleDbType.Int32).Value = zamestanec.PoziceIdPozice;
                            command.Parameters.Add("p_prodejna_idprodejny", OracleDbType.Int32).Value = (object)zamestanec.ProdejnaIdProdejny ?? DBNull.Value;
                            command.Parameters.Add("p_idnadrezene", OracleDbType.Int32).Value = (object)zamestanec.IdNadrezene ?? DBNull.Value;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении сотрудника");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении сотрудника: {ex.Message}");
                }
            }

            return View(zamestanec);
        }

        // Метод для отображения формы редактирования сотрудника
        public async Task<IActionResult> Edit(int idZamestance)
        {
            Zamestanec? zamestanec = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ZAMESTANCE_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idzamestance", OracleDbType.Int32).Value = idZamestance;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                zamestanec = new Zamestanec
                                {
                                    IdZamestance = reader.GetInt32(0),
                                    Jmeno = reader.GetString(1),
                                    Prijmeni = reader.GetString(2),
                                    RodneCislo = reader.GetString(3),
                                    TelefonniCislo = reader.GetString(4),
                                    PoziceIdPozice = reader.GetInt32(5),
                                    ProdejnaIdProdejny = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                    IdNadrezene = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                                    DatumVytvoreni = reader.GetDateTime(8),
                                    DatumAktualizace = reader.GetDateTime(9)
                                };
                            }
                        }
                    }
                }

                if (zamestanec == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении сотрудника для редактирования");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении сотрудника для редактирования: {ex.Message}");
                return RedirectToAction("Index");
            }

            return View(zamestanec);
        }

        // Метод для обработки редактирования сотрудника
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idZamestance, Zamestanec zamestanec)
        {
            if (idZamestance != zamestanec.IdZamestance)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    zamestanec.DatumAktualizace = DateTime.UtcNow;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("AKTUALIZUJ_ZAMESTANCE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idzamestance", OracleDbType.Int32).Value = zamestanec.IdZamestance;
                            command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = zamestanec.Jmeno;
                            command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = zamestanec.Prijmeni;
                            command.Parameters.Add("p_rodnecislo", OracleDbType.Varchar2).Value = zamestanec.RodneCislo;
                            command.Parameters.Add("p_telefonicislo", OracleDbType.Varchar2).Value = zamestanec.TelefonniCislo;
                            command.Parameters.Add("p_pozice_idpozice", OracleDbType.Int32).Value = zamestanec.PoziceIdPozice;
                            command.Parameters.Add("p_prodejna_idprodejny", OracleDbType.Int32).Value = (object)zamestanec.ProdejnaIdProdejny ?? DBNull.Value;
                            command.Parameters.Add("p_idnadrezene", OracleDbType.Int32).Value = (object)zamestanec.IdNadrezene ?? DBNull.Value;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении сотрудника");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении сотрудника: {ex.Message}");
                }
            }

            return View(zamestanec);
        }

        // Метод для обработки удаления сотрудника
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int idZamestance)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_ZAMESTANCE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idzamestance", OracleDbType.Int32).Value = idZamestance;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении сотрудника");
                return Json(new { success = false, message = $"Произошла ошибка при удалении сотрудника: {ex.Message}" });
            }
        }


    }
}
