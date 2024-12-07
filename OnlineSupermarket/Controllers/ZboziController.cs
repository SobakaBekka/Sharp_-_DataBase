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
    public class ZboziController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<ZboziController> _logger;

        public ZboziController(IConfiguration configuration, ILogger<ZboziController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Метод для отображения списка товаров
        public async Task<IActionResult> Index()
        {
            var zboziList = new List<Zbozi>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ZBOZI", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                zboziList.Add(new Zbozi
                                {
                                    IdZbozi = reader.GetInt32(0),
                                    Nazev = reader.GetString(1),
                                    AktualniCena = reader.GetDecimal(2),
                                    CenaZeKlubKartou = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3),
                                    Hmotnost = reader.GetDecimal(4),
                                    Slozeni = reader.GetString(5),
                                    KategorieIdKategorie = reader.GetInt32(6),
                                    DatumVytvoreni = reader.GetDateTime(7),
                                    DatumAktualizace = reader.GetDateTime(8)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении списка товаров");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка товаров: {ex.Message}");
            }

            return View(zboziList);
        }

        // Метод для отображения формы создания товара
        public IActionResult Create()
        {
            return View();
        }

        // Метод для обработки создания товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Zbozi zbozi)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Установим текущее время для полей DatumVytvoreni и DatumAktualizace перед сохранением в базу
                    zbozi.DatumVytvoreni = DateTime.UtcNow;
                    zbozi.DatumAktualizace = DateTime.UtcNow;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_ZBOZI", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = zbozi.Nazev;
                            command.Parameters.Add("p_aktualni_cena", OracleDbType.Decimal).Value = zbozi.AktualniCena;
                            command.Parameters.Add("p_cena_ze_klub_kartou", OracleDbType.Decimal).Value = zbozi.CenaZeKlubKartou ?? (object)DBNull.Value;
                            command.Parameters.Add("p_hmotnost", OracleDbType.Decimal).Value = zbozi.Hmotnost;
                            command.Parameters.Add("p_slozeni", OracleDbType.Varchar2).Value = zbozi.Slozeni;
                            command.Parameters.Add("p_kategorie_idkategorii", OracleDbType.Int32).Value = zbozi.KategorieIdKategorie;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении записи");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении записи: {ex.Message}");
                }
            }

            return View(zbozi);
        }

        // Метод для отображения формы редактирования товара
        public async Task<IActionResult> Edit(int id)
        {
            Zbozi? zbozi = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (OracleCommand command = new OracleCommand("ZOBRAZ_ZBOZI_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Входной параметр - ID товара
                        command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

                        // Выходной параметр - курсор
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                zbozi = new Zbozi
                                {
                                    IdZbozi = reader.GetInt32(0),
                                    Nazev = reader.GetString(1),
                                    AktualniCena = reader.GetDecimal(2),
                                    CenaZeKlubKartou = reader.IsDBNull(3) ? (decimal?)null : reader.GetDecimal(3),
                                    Hmotnost = reader.GetDecimal(4),
                                    Slozeni = reader.GetString(5),
                                    KategorieIdKategorie = reader.GetInt32(6),
                                    DatumVytvoreni = reader.GetDateTime(7),
                                    DatumAktualizace = reader.GetDateTime(8)
                                };
                            }
                        }
                    }
                }

                if (zbozi == null)
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

            return View(zbozi);
        }

        // Метод для обработки редактирования товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Zbozi zbozi)
        {
            if (id != zbozi.IdZbozi)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Обновим время обновления
                    zbozi.DatumAktualizace = DateTime.UtcNow;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("AKTUALIZUJ_ZBOZI", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idzbozi", OracleDbType.Int32).Value = zbozi.IdZbozi;
                            command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = zbozi.Nazev;
                            command.Parameters.Add("p_aktualni_cena", OracleDbType.Decimal).Value = zbozi.AktualniCena;
                            command.Parameters.Add("p_cena_ze_klub_kartou", OracleDbType.Decimal).Value = zbozi.CenaZeKlubKartou ?? (object)DBNull.Value;
                            command.Parameters.Add("p_hmotnost", OracleDbType.Decimal).Value = zbozi.Hmotnost;
                            command.Parameters.Add("p_slozeni", OracleDbType.Varchar2).Value = zbozi.Slozeni;
                            command.Parameters.Add("p_kategorie_idkategorii", OracleDbType.Int32).Value = zbozi.KategorieIdKategorie;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении записи");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении записи: {ex.Message}");
                }
            }

            return View(zbozi);
        }

        // Метод для обработки удаления товара
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_ZBOZI", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idzbozi", OracleDbType.Int32).Value = id;

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



