using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace OnlineSupermarket.Controllers
{
    public class PultController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<PultController> _logger;

        public PultController(IConfiguration configuration, ILogger<PultController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Метод для отображения списка пультов
        public async Task<IActionResult> Index()
        {
            var pultList = new List<Pult>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_PULT", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                pultList.Add(new Pult
                                {
                                    IdPultu = reader.GetInt32(0),
                                    Cislo = reader.GetString(1),
                                    PocetPolicek = reader.GetInt32(2),
                                    ProdejnaIdProdejny = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                    IdKategorie = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                    DatumVytvoreni = reader.GetDateTime(5),
                                    DatumAktualizace = reader.GetDateTime(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении списка пультов");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка пультов: {ex.Message}");
            }

            return View(pultList);
        }

        // Метод для отображения формы создания пульта
        public IActionResult Create()
        {
            return View();
        }

        // Метод для обработки создания пульта
        [HttpPost]
        public async Task<IActionResult> Create(Pult pult)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_PULT", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_cislo", OracleDbType.Varchar2).Value = pult.Cislo;
                            command.Parameters.Add("p_pocetpolicek", OracleDbType.Int32).Value = pult.PocetPolicek;
                            command.Parameters.Add("p_prodejna_idprodejny", OracleDbType.Int32).Value = pult.ProdejnaIdProdejny ?? (object)DBNull.Value;
                            command.Parameters.Add("p_idkategorie", OracleDbType.Int32).Value = pult.IdKategorie ?? (object)DBNull.Value;

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

            return View(pult);
        }

        // Асинхронный метод для отображения формы редактирования пульта
        public async Task<IActionResult> Edit(int id)
        {
            Pult? pult = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (OracleCommand command = new OracleCommand("ZOBRAZ_PULT_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Входной параметр - ID пульта
                        command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

                        // Выходной параметр - курсор
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                pult = new Pult
                                {
                                    IdPultu = reader.GetInt32(0),
                                    Cislo = reader.GetString(1),
                                    PocetPolicek = reader.GetInt32(2),
                                    ProdejnaIdProdejny = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                    IdKategorie = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                    DatumVytvoreni = reader.GetDateTime(5),
                                    DatumAktualizace = reader.GetDateTime(6)
                                };
                            }
                        }
                    }
                }

                if (pult == null)
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

            return View(pult);
        }

        // Метод для обработки редактирования пульта
        [HttpPost]
        public async Task<IActionResult> Edit(Pult pult)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("AKTUALIZUJ_PULT", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idpultu", OracleDbType.Int32).Value = pult.IdPultu;
                            command.Parameters.Add("p_cislo", OracleDbType.Varchar2).Value = pult.Cislo;
                            command.Parameters.Add("p_pocetpolicek", OracleDbType.Int32).Value = pult.PocetPolicek;
                            command.Parameters.Add("p_prodejna_idprodejny", OracleDbType.Int32).Value = pult.ProdejnaIdProdejny ?? (object)DBNull.Value;
                            command.Parameters.Add("p_idkategorie", OracleDbType.Int32).Value = pult.IdKategorie ?? (object)DBNull.Value;

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

            return View(pult);
        }

        // Метод для обработки удаления пульта
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int idPultu)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_PULT", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idpultu", OracleDbType.Int32).Value = idPultu;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении пульта");
                return Json(new { success = false, message = $"Произошла ошибка при удалении пульта: {ex.Message}" });
            }
        }



    }
}

