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
    public class PlatbaController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<PlatbaController> _logger;

        public PlatbaController(IConfiguration configuration, ILogger<PlatbaController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Отображение списка платежей
        public async Task<IActionResult> Index()
        {
            var platbaList = new List<Platba>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_PLATBU", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                platbaList.Add(new Platba
                                {
                                    IdTranzakce = reader.GetInt32(0),
                                    CelkovaCena = reader.GetDecimal(1),
                                    Typ = reader.GetString(2),
                                    DatumVytvoreni = reader.GetDateTime(3),
                                    DatumAktualizace = reader.GetDateTime(4),
                                    ZboziIdZbozi = reader.GetInt32(5),
                                    ProdejnaIdProdejny = reader.GetInt32(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении списка платежей");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка платежей: {ex.Message}");
            }

            return View(platbaList);
        }

        // Форма создания платежа
        public IActionResult Create()
        {
            return View();
        }

        // Обработка отправки формы создания платежа
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Platba platba)
        {
            // Логирование значений модели перед проверкой состояния модели
            _logger.LogInformation("Значения модели: CelkovaCena={CelkovaCena}, Typ={Typ}, ZboziIdZbozi={ZboziIdZbozi}, ProdejnaIdProdejny={ProdejnaIdProdejny}",
                platba.CelkovaCena, platba.Typ, platba.ZboziIdZbozi, platba.ProdejnaIdProdejny);

            // Даты устанавливает база, но если нужно, можно их не трогать тут
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Начало создания платежа");

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        _logger.LogInformation("Открытие соединения с базой данных");
                        await connection.OpenAsync();

                        using (var command = new OracleCommand("VLOZ_PLATBU", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_celkovaCena", OracleDbType.Decimal).Value = platba.CelkovaCena;
                            command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = platba.Typ;
                            command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = platba.ZboziIdZbozi;
                            command.Parameters.Add("p_prodejna_idprodejny", OracleDbType.Int32).Value = platba.ProdejnaIdProdejny;

                            _logger.LogInformation("Параметры команды: p_celkovaCena={CelkovaCena}, p_typ={Typ}, p_zbozi_idzbozi={ZboziIdZbozi}, p_prodejna_idprodejny={ProdejnaIdProdejny}",
                                platba.CelkovaCena, platba.Typ, platba.ZboziIdZbozi, platba.ProdejnaIdProdejny);

                            _logger.LogInformation("Выполнение команды для добавления платежа");
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    _logger.LogInformation("Платеж успешно создан");
                    return RedirectToAction(nameof(Index));
                }
                catch (OracleException ex)
                {
                    _logger.LogError(ex, "Произошла ошибка Oracle при добавлении платежа: {Message}", ex.Message);
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка Oracle при добавлении платежа: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении платежа: {Message}", ex.Message);
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении платежа: {ex.Message}");
                }
            }
            else
            {
                _logger.LogWarning("Модель состояния недействительна: {ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            return View(platba);
        }





        // Форма редактирования
        public async Task<IActionResult> Edit(int idTranzakce)
        {
            Platba? platba = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_PLATBU_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idtranzakce", OracleDbType.Int32).Value = idTranzakce;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                platba = new Platba
                                {
                                    IdTranzakce = reader.GetInt32(0),
                                    CelkovaCena = reader.GetDecimal(1),
                                    Typ = reader.GetString(2),
                                    DatumVytvoreni = reader.GetDateTime(3),
                                    DatumAktualizace = reader.GetDateTime(4),
                                    ZboziIdZbozi = reader.GetInt32(5),
                                    ProdejnaIdProdejny = reader.GetInt32(6)
                                };
                            }
                            else
                            {
                                _logger.LogWarning("Запись с ID {IdTranzakce} не найдена", idTranzakce);
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Произошла ошибка Oracle при получении платежа для редактирования");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении платежа для редактирования: {ex.Message}");
                return RedirectToAction("Index");
            }

            return View(platba);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idTranzakce, Platba platba)
        {
            if (idTranzakce != platba.IdTranzakce)
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
                        _logger.LogInformation("Попытка обновления platba с IdTranzakce: {IdTranzakce}, CelkovaCena: {CelkovaCena}, Typ: {Typ}, ZboziIdZbozi: {ZboziIdZbozi}, ProdejnaIdProdejny: {ProdejnaIdProdejny}",
                            platba.IdTranzakce, platba.CelkovaCena, platba.Typ, platba.ZboziIdZbozi, platba.ProdejnaIdProdejny);

                        // Исправляем имя процедуры
                        using (var command = new OracleCommand("AKTUALIZUJ_PLATBU", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idtranzakce", OracleDbType.Int32).Value = platba.IdTranzakce;
                            command.Parameters.Add("p_celkovaCena", OracleDbType.Decimal).Value = platba.CelkovaCena;
                            command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = platba.Typ;
                            command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = platba.ZboziIdZbozi;
                            command.Parameters.Add("p_prodejna_idprodejny", OracleDbType.Int32).Value = platba.ProdejnaIdProdejny;

                            var affectedRows = await command.ExecuteNonQueryAsync();

                            // Логгирование количества затронутых строк
                            _logger.LogInformation("Обновлено строк: {AffectedRows}", affectedRows);
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении платежа");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении платежа: {ex.Message}");
                }
            }

            return View(platba);
        }

        // Удаление платежа
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int idTranzakce)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_PLATBU", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idtranzakce", OracleDbType.Int32).Value = idTranzakce;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении платежа");
                return Json(new { success = false, message = $"Произошла ошибка при удалении платежа: {ex.Message}" });
            }
        }
    }
}
