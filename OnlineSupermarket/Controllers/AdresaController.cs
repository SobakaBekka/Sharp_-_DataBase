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
    public class AdresaController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<AdresaController> _logger;

        public AdresaController(IConfiguration configuration, ILogger<AdresaController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Метод для отображения списка адресов
        public async Task<IActionResult> Index()
        {
            var adresaList = new List<Adresa>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ADRESY", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                adresaList.Add(new Adresa
                                {
                                    IdAdresy = reader.GetInt32(0),
                                    Mesto = reader.GetString(1),
                                    Ulice = reader.GetString(2),
                                    Psc = reader.GetString(3),
                                    ProdejnaIdProdejny = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                    ZamestnanecIdZamestance = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                    SkladIdSkladu = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
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
                _logger.LogError(ex, "Произошла ошибка при получении списка адресов");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка адресов: {ex.Message}");
            }

            return View(adresaList);
        }

        // Метод для отображения формы создания адреса
        public IActionResult Create()
        {
            return View();
        }

        // Метод для обработки создания адреса
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Adresa adresa)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Установим текущее время для полей DatumVytvoreni и DatumAktualizace перед сохранением в базу
                    adresa.DatumVytvoreni = DateTime.UtcNow;
                    adresa.DatumAktualizace = DateTime.UtcNow;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_ADRESU", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = adresa.Mesto;
                            command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = adresa.Ulice;
                            command.Parameters.Add("p_psc", OracleDbType.Varchar2).Value = adresa.Psc;
                            command.Parameters.Add("p_prodejna_idprodejny", OracleDbType.Int32).Value = (object)adresa.ProdejnaIdProdejny ?? DBNull.Value;
                            command.Parameters.Add("p_zamestnanec_idzamestance", OracleDbType.Int32).Value = (object)adresa.ZamestnanecIdZamestance ?? DBNull.Value;
                            command.Parameters.Add("p_sklad_idskladu", OracleDbType.Int32).Value = (object)adresa.SkladIdSkladu ?? DBNull.Value;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении адреса");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении адреса: {ex.Message}");
                }
            }

            return View(adresa);
        }

        // Метод для отображения формы редактирования адреса
        public async Task<IActionResult> Edit(int idAdresy)
        {
            Adresa? adresa = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ADRESU_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Входной параметр - ID адреса
                        command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = idAdresy;

                        // Выходной параметр - курсор для получения данных
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                adresa = new Adresa
                                {
                                    IdAdresy = reader.GetInt32(0),
                                    Mesto = reader.GetString(1),
                                    Ulice = reader.GetString(2),
                                    Psc = reader.GetString(3),
                                    ProdejnaIdProdejny = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                    ZamestnanecIdZamestance = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                    SkladIdSkladu = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                    DatumVytvoreni = reader.GetDateTime(7),
                                    DatumAktualizace = reader.GetDateTime(8)
                                };
                            }
                            else
                            {
                                // Обработка, если данные не найдены
                                _logger.LogWarning("Запись с ID {IdAdresy} не найдена", idAdresy);
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // Обработка ошибок Oracle
                _logger.LogError(ex, "Произошла ошибка Oracle при получении адреса для редактирования");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении адреса для редактирования: {ex.Message}");
                return RedirectToAction("Index");
            }

            return View(adresa);
        }





        // Метод для обработки редактирования адреса
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idAdresy, Adresa adresa)
        {
            if (idAdresy != adresa.IdAdresy)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    adresa.DatumAktualizace = DateTime.UtcNow;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("AKTUALIZUJ_ADRESU", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = adresa.IdAdresy;
                            command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = adresa.Mesto;
                            command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = adresa.Ulice;
                            command.Parameters.Add("p_psc", OracleDbType.Varchar2).Value = adresa.Psc;
                            command.Parameters.Add("p_prodejna_idprodejny", OracleDbType.Int32).Value = (object)adresa.ProdejnaIdProdejny ?? DBNull.Value;
                            command.Parameters.Add("p_zamestnanec_idzamestance", OracleDbType.Int32).Value = (object)adresa.ZamestnanecIdZamestance ?? DBNull.Value;
                            command.Parameters.Add("p_sklad_idskladu", OracleDbType.Int32).Value = (object)adresa.SkladIdSkladu ?? DBNull.Value;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении адреса");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении адреса: {ex.Message}");
                }
            }

            return View(adresa);
        }

        // Метод для обработки удаления адреса
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int idAdresy)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_ADRESU", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = idAdresy;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении адреса");
                return Json(new { success = false, message = $"Произошла ошибка при удалении адреса: {ex.Message}" });
            }
        }
    }
}
