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
    public class ZboziNaPulteController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<ZboziNaPulteController> _logger;

        public ZboziNaPulteController(IConfiguration configuration, ILogger<ZboziNaPulteController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Метод для отображения списка товаров на полке
        public async Task<IActionResult> Index()
        {
            var zboziNaPulteList = new List<ZboziNaPulte>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ZBOZINAPULTE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                zboziNaPulteList.Add(new ZboziNaPulte
                                {
                                    Pocet = reader.GetInt32(0),
                                    ZboziIdZbozi = reader.GetInt32(1),
                                    PultIdPultu = reader.GetInt32(2),
                                    DatumVytvoreni = reader.GetDateTime(3),
                                    DatumAktualizace = reader.GetDateTime(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении списка товаров на полке");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка товаров на полке: {ex.Message}");
            }

            return View(zboziNaPulteList);
        }

        // Метод для отображения формы создания товара на полке
        public IActionResult Create()
        {
            return View();
        }

        // Метод для обработки создания товара на полке
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZboziNaPulte zboziNaPulte)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_ZBOZINAPULTE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_pocet", OracleDbType.Int32).Value = zboziNaPulte.Pocet;
                            command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziNaPulte.ZboziIdZbozi;
                            command.Parameters.Add("p_pult_idpultu", OracleDbType.Int32).Value = zboziNaPulte.PultIdPultu;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении записи");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении записи: {ex.Message}");
                }
            }

            return View(zboziNaPulte);
        }

        // Метод для отображения формы редактирования товара на полке
        public async Task<IActionResult> Edit(int zboziId, int pultId)
        {
            ZboziNaPulte? zboziNaPulte = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ZBOZINAPULTE_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Входные параметры - ID товара и ID полки
                        command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziId;
                        command.Parameters.Add("p_pult_idpultu", OracleDbType.Int32).Value = pultId;

                        // Выходной параметр - курсор
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                zboziNaPulte = new ZboziNaPulte
                                {
                                    Pocet = reader.GetInt32(0),
                                    ZboziIdZbozi = reader.GetInt32(1),
                                    PultIdPultu = reader.GetInt32(2),
                                    DatumVytvoreni = reader.GetDateTime(3),
                                    DatumAktualizace = reader.GetDateTime(4)
                                };
                            }
                        }
                    }
                }

                if (zboziNaPulte == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении записи для редактирования");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении записи для редактирования: {ex.Message}");
                return RedirectToAction("Index");
            }

            return View(zboziNaPulte);
        }

        // Метод для обработки редактирования товара на полке
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ZboziNaPulte zboziNaPulte)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("AKTUALIZUJ_ZBOZINAPULTE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_pocet", OracleDbType.Int32).Value = zboziNaPulte.Pocet;
                            command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziNaPulte.ZboziIdZbozi;
                            command.Parameters.Add("p_pult_idpultu", OracleDbType.Int32).Value = zboziNaPulte.PultIdPultu;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении записи");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении записи: {ex.Message}");
                }
            }

            return View(zboziNaPulte);
        }




        // Метод для обработки удаления товара на полке
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int zboziId, int pultId)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_ZBOZINAPULTE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_zbozi_idzbozi", OracleDbType.Int32).Value = zboziId;
                        command.Parameters.Add("p_pult_idpultu", OracleDbType.Int32).Value = pultId;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении записи");
                return Json(new { success = false, message = $"Произошла ошибка при удалении записи: {ex.Message}" });
            }
        }
    }
}


