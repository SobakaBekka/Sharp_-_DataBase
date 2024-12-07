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
    public class ProdejnaController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<ProdejnaController> _logger;

        public ProdejnaController(IConfiguration configuration, ILogger<ProdejnaController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Метод для отображения списка магазинов
        public async Task<IActionResult> Index()
        {
            var prodejnaList = new List<Prodejna>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_PRODEJNU", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                prodejnaList.Add(new Prodejna
                                {
                                    IdProdejny = reader.GetInt32(0),
                                    Kontaktnicislo = reader.GetString(1),
                                    Plocha = reader.GetInt32(2),
                                    PokladnaIdPokladny = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                    DatumVytvoreni = reader.GetDateTime(4),
                                    DatumAktualizace = reader.GetDateTime(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении списка магазинов");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка магазинов: {ex.Message}");
            }

            return View(prodejnaList);
        }




        // Метод для отображения формы создания магазина
        public IActionResult Create()
        {
            return View();
        }

        // Метод для обработки отправки формы создания магазина
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prodejna prodejna)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_PRODEJNU", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_kontaktnicislo", OracleDbType.Varchar2).Value = prodejna.Kontaktnicislo;
                            command.Parameters.Add("p_plocha", OracleDbType.Decimal).Value = prodejna.Plocha;
                            command.Parameters.Add("p_pokladna_idpokladny", OracleDbType.Int32).Value = prodejna.PokladnaIdPokladny ?? (object)DBNull.Value;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении магазина");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении магазина: {ex.Message}");
                }
            }

            return View(prodejna);
        }







        // Метод для отображения формы редактирования магазина
        public async Task<IActionResult> Edit(int idProdejny)
        {
            Prodejna? prodejna = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_PRODEJNU_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Входной параметр - ID магазина
                        command.Parameters.Add("p_idprodejny", OracleDbType.Int32).Value = idProdejny;

                        // Выходной параметр - курсор для получения данных
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                prodejna = new Prodejna
                                {
                                    IdProdejny = reader.GetInt32(0),
                                    Kontaktnicislo = reader.GetString(1),
                                    Plocha = reader.GetInt32(2),
                                    PokladnaIdPokladny = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                    DatumVytvoreni = reader.GetDateTime(4),
                                    DatumAktualizace = reader.GetDateTime(5)
                                };
                            }
                            else
                            {
                                // Обработка, если данные не найдены
                                _logger.LogWarning("Запись с ID {IdProdejny} не найдена", idProdejny);
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // Обработка ошибок Oracle
                _logger.LogError(ex, "Произошла ошибка Oracle при получении магазина для редактирования");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении магазина для редактирования: {ex.Message}");
                return RedirectToAction("Index");
            }

            return View(prodejna);
        }


        // Метод для обработки редактирования магазина




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idProdejny, Prodejna prodejna)
        {
            if (idProdejny != prodejna.IdProdejny)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        // Логгирование для проверки значений перед вызовом процедуры
                        _logger.LogInformation("Попытка обновления prodejna с IdProdejny: {IdProdejny}, Kontaktnicislo: {Kontaktnicislo}, Plocha: {Plocha}, PokladnaIdPokladny: {PokladnaIdPokladny}",
                            prodejna.IdProdejny, prodejna.Kontaktnicislo, prodejna.Plocha, prodejna.PokladnaIdPokladny);

                        using (var command = new OracleCommand("AKTUALIZUJ_PRODEJNU", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idprodejny", OracleDbType.Int32).Value = prodejna.IdProdejny;
                            command.Parameters.Add("p_kontaktnicislo", OracleDbType.Varchar2).Value = prodejna.Kontaktnicislo;
                            command.Parameters.Add("p_plocha", OracleDbType.Decimal).Value = prodejna.Plocha;
                            command.Parameters.Add("p_pokladna_idpokladny", OracleDbType.Int32).Value = prodejna.PokladnaIdPokladny;

                            var affectedRows = await command.ExecuteNonQueryAsync();

                            // Логгирование количества затронутых строк
                            _logger.LogInformation("Обновлено строк: {AffectedRows}", affectedRows);
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении prodejna");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении prodejna: {ex.Message}");
                }
            }

            return View(prodejna);
        }






        // Метод для обработки удаления магазина
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int idProdejny)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_PRODEJNU", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idprodejny", OracleDbType.Int32).Value = idProdejny;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении магазина");
                return Json(new { success = false, message = $"Произошла ошибка при удалении магазина: {ex.Message}" });
            }
        }
    }
}
