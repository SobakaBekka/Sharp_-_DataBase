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
    public class ProdaneZboziController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<ProdaneZboziController> _logger;

        public ProdaneZboziController(IConfiguration configuration, ILogger<ProdaneZboziController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Метод для отображения списка проданных товаров
        public async Task<IActionResult> Index()
        {
            var prodaneZboziList = new List<ProdaneZbozi>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_PRODANEZBOZI", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                prodaneZboziList.Add(new ProdaneZbozi
                                {
                                    Pocet = reader.GetInt32(0),
                                    Prodejnicena = reader.GetDecimal(1),
                                    ZboziIdZbozi = reader.GetInt32(2),
                                    IdTranzakce = reader.GetInt32(3),
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
                _logger.LogError(ex, "Произошла ошибка при получении списка проданных товаров");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка проданных товаров: {ex.Message}");
            }

            return View(prodaneZboziList);
        }

        // Метод для отображения формы создания проданного товара
        public IActionResult Create()
        {
            return View();
        }

        // Метод для обработки создания проданного товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdaneZbozi prodaneZbozi)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Установим текущее время для полей DatumVytvoreni и DatumAktualizace перед сохранением в базу
                    prodaneZbozi.DatumVytvoreni = DateTime.UtcNow;
                    prodaneZbozi.DatumAktualizace = DateTime.UtcNow;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_PRODANEZBOZI", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_pocet", OracleDbType.Int32).Value = prodaneZbozi.Pocet;
                            command.Parameters.Add("p_prodejnicena", OracleDbType.Decimal).Value = prodaneZbozi.Prodejnicena;
                            command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = prodaneZbozi.ZboziIdZbozi;
                            command.Parameters.Add("p_idtranzakce", OracleDbType.Int32).Value = prodaneZbozi.IdTranzakce;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении проданного товара");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении проданного товара: {ex.Message}");
                }
            }

            return View(prodaneZbozi);
        }

        // Метод для отображения формы редактирования проданного товара
        public async Task<IActionResult> Edit(int zboziIdZbozi, int idTranzakce)
        {
            ProdaneZbozi? prodaneZbozi = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_PRODANEZBOZI_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Входные параметры - ID товара и ID транзакции
                        command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziIdZbozi;
                        command.Parameters.Add("p_idtranzakce", OracleDbType.Int32).Value = idTranzakce;

                        // Выходной параметр - курсор
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                prodaneZbozi = new ProdaneZbozi
                                {
                                    Pocet = reader.GetInt32(0),
                                    Prodejnicena = reader.GetDecimal(1),
                                    ZboziIdZbozi = reader.GetInt32(2),
                                    IdTranzakce = reader.GetInt32(3),
                                    DatumVytvoreni = reader.GetDateTime(4),
                                    DatumAktualizace = reader.GetDateTime(5)
                                };
                            }
                        }
                    }
                }

                if (prodaneZbozi == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении проданного товара для редактирования");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении проданного товара для редактирования: {ex.Message}");
                return RedirectToAction("Index");
            }

            return View(prodaneZbozi);
        }

        // Метод для обработки редактирования проданного товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int zboziIdZbozi, int idTranzakce, ProdaneZbozi prodaneZbozi)
        {
            if (zboziIdZbozi != prodaneZbozi.ZboziIdZbozi || idTranzakce != prodaneZbozi.IdTranzakce)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Обновим время обновления
                    prodaneZbozi.DatumAktualizace = DateTime.UtcNow;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        // Логгирование для проверки значений перед вызовом процедуры
                        _logger.LogInformation("Попытка обновления проданного товара с ZboziIdZbozi: {ZboziIdZbozi}, IdTranzakce: {IdTranzakce}, Pocet: {Pocet}, Prodejnicena: {Prodejnicena}",
                            prodaneZbozi.ZboziIdZbozi, prodaneZbozi.IdTranzakce, prodaneZbozi.Pocet, prodaneZbozi.Prodejnicena);

                        using (var command = new OracleCommand("AKTUALIZUJ_PRODANEZBOZI", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = prodaneZbozi.ZboziIdZbozi;
                            command.Parameters.Add("p_idtranzakce", OracleDbType.Int32).Value = prodaneZbozi.IdTranzakce;
                            command.Parameters.Add("p_pocet", OracleDbType.Int32).Value = prodaneZbozi.Pocet;
                            command.Parameters.Add("p_prodejnicena", OracleDbType.Decimal).Value = prodaneZbozi.Prodejnicena;

                            var affectedRows = await command.ExecuteNonQueryAsync();

                            // Логгирование количества затронутых строк
                            _logger.LogInformation("Обновлено строк: {AffectedRows}", affectedRows);
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении проданного товара");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении проданного товара: {ex.Message}");
                }
            }

            return View(prodaneZbozi);
        }

        // Метод для обработки удаления проданного товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int zboziIdZbozi, int idTranzakce)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_PRODANEZBOZI", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziIdZbozi;
                        command.Parameters.Add("p_idtranzakce", OracleDbType.Int32).Value = idTranzakce;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении проданного товара");
                return Json(new { success = false, message = $"Произошла ошибка при удалении проданного товара: {ex.Message}" });
            }
        }
    }
}





