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
    public class RoleController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IConfiguration configuration, ILogger<RoleController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Метод для отображения списка ролей
        public async Task<IActionResult> Index()
        {
            var roleList = new List<Role>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ROLE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                roleList.Add(new Role
                                {
                                    IdRole = reader.GetInt32(0),
                                    Nazev = reader.GetString(1),
                                    Popis = reader.IsDBNull(2) ? null : reader.GetString(2),
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
                _logger.LogError(ex, "Произошла ошибка при получении списка ролей");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка ролей: {ex.Message}");
            }

            return View(roleList);
        }

        // Метод для отображения формы создания роли
        public IActionResult Create()
        {
            return View();
        }

        // Метод для обработки создания роли
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_ROLE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = role.Nazev;
                            command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = role.Popis;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении роли");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении роли: {ex.Message}");
                }
            }

            return View(role);
        }

        // Метод для отображения формы редактирования роли
        public async Task<IActionResult> Edit(int idRole)
        {
            Role? role = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_ROLE_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Входной параметр - ID роли
                        command.Parameters.Add("p_idrole", OracleDbType.Int32).Value = idRole;

                        // Выходной параметр - курсор
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                role = new Role
                                {
                                    IdRole = reader.GetInt32(0),
                                    Nazev = reader.GetString(1),
                                    Popis = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    DatumVytvoreni = reader.GetDateTime(3),
                                    DatumAktualizace = reader.GetDateTime(4)
                                };
                            }
                            else
                            {
                                // Обработка, если данные не найдены
                                _logger.LogWarning("Запись с ID {IdRole} не найдена", idRole);
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                // Обработка ошибок Oracle
                _logger.LogError(ex, "Произошла ошибка Oracle при получении роли для редактирования");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении роли для редактирования: {ex.Message}");
                return RedirectToAction("Index");
            }

            return View(role);
        }

        // Метод для обработки редактирования роли
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idRole, Role role)
        {
            if (idRole != role.IdRole)
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
                        _logger.LogInformation("Попытка обновления role с IdRole: {IdRole}, Nazev: {Nazev}, Popis: {Popis}",
                            role.IdRole, role.Nazev, role.Popis);

                        using (var command = new OracleCommand("AKTUALIZUJ_ROLE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idrole", OracleDbType.Int32).Value = role.IdRole;
                            command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = role.Nazev;
                            command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = role.Popis;

                            var affectedRows = await command.ExecuteNonQueryAsync();

                            // Логгирование количества затронутых строк
                            _logger.LogInformation("Обновлено строк: {AffectedRows}", affectedRows);
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении role");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении role: {ex.Message}");
                }
            }

            return View(role);
        }

        // Метод для обработки удаления роли
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int idRole)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_ROLE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idrole", OracleDbType.Int32).Value = idRole;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении роли");
                return Json(new { success = false, message = $"Произошла ошибка при удалении роли: {ex.Message}" });
            }
        }
    }
}


