using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace OnlineSupermarket.Controllers
{
    public class RegisUzivatelController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<RegisUzivatelController> _logger;

        public RegisUzivatelController(IConfiguration configuration, ILogger<RegisUzivatelController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }




       




        public IActionResult DetailyUzivatele(int id)
        {
            RegisUzivatel? uzivatel = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new OracleCommand("ZOBRAZ_REGISUZIVATELE_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Vstupní parametr - ID uživatele
                        command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

                        // Výstupní parametr - kurzor
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                uzivatel = new RegisUzivatel
                                {
                                    IdRegisUzivatelu = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Jmeno = reader.GetString(3),
                                    Prijmeni = reader.GetString(4),
                                    Heslo = reader.GetString(5),
                                    DatumVytvoreni = reader.GetDateTime(6),
                                    DatumAktualizace = reader.GetDateTime(7),
                                    PosledniPrihlaseni = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                                    NeuspesnePrihlaseni = reader.GetInt32(9),
                                    RoleIdRole = reader.GetInt32(10),
                                    Rolenazev = reader.GetString(11)
                                };
                            }
                        }
                    }
                }

                if (uzivatel == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Došlo k chybě při získávání uživatele pro zobrazení");
                ModelState.AddModelError(string.Empty, $"Došlo k chybě při získávání uživatele pro zobrazení: {ex.Message}");
                return RedirectToAction("Index");
            }

            // Установите данные сессии для пользователя
            HttpContext.Session.SetString("Username", uzivatel.Username);
            HttpContext.Session.SetString("Role", uzivatel.Rolenazev);

            return RedirectToAction("Index", "Home");
        }







        // Метод для отображения списка пользователей
        public async Task<IActionResult> Index()
        {
            var uzivatelList = new List<RegisUzivatel>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_REGISUZIVATELE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                uzivatelList.Add(new RegisUzivatel
                                {
                                    IdRegisUzivatelu = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Jmeno = reader.GetString(3),
                                    Prijmeni = reader.GetString(4),
                                    Heslo = reader.GetString(5),
                                    DatumVytvoreni = reader.GetDateTime(6),
                                    DatumAktualizace = reader.GetDateTime(7),
                                    PosledniPrihlaseni = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                                    NeuspesnePrihlaseni = reader.GetInt32(9),
                                    RoleIdRole = reader.GetInt32(10),
                                    Rolenazev = reader.GetString(11)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении списка пользователей");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении списка пользователей: {ex.Message}");
            }

            return View(uzivatelList);
        }

        // Метод для отображения формы создания пользователя
        public IActionResult Create()
        {
            ViewBag.Roles = GetRoles();
            return View();
        }

        // Метод для обработки создания пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisUzivatel uzivatel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Генерация соли для хэширования
                    byte[] salt = new byte[128 / 8]; // 16 байт
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(salt);
                    }

                    // Хэширование пароля
                    string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: uzivatel.Heslo,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 10000, // количество итераций (можно настроить)
                        numBytesRequested: 256 / 8)); // длина хэша 32 байта

                    // В идеальном случае вы сохраняете соль и хэш вместе, например, в формате "salt:hash"
                    // или храните соль в отдельном поле. Для примера объединим их через двоеточие.
                    string saltBase64 = Convert.ToBase64String(salt);
                    string passwordWithSalt = $"{saltBase64}:{hashedPassword}";

                    // Устанавливаем поля даты
                    uzivatel.DatumVytvoreni = DateTime.UtcNow;
                    uzivatel.DatumAktualizace = DateTime.UtcNow;

                    // Вместо исходного пароля сохраняем хэшированный
                    uzivatel.Heslo = passwordWithSalt;

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_REGISUZIVATELE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = uzivatel.Username;
                            command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = uzivatel.Email;
                            command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = uzivatel.Jmeno;
                            command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = uzivatel.Prijmeni;
                            // Записываем вместо исходного пароля хэшированный
                            command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = uzivatel.Heslo;
                            command.Parameters.Add("p_role_idrole", OracleDbType.Int32).Value = uzivatel.RoleIdRole;
                            command.Parameters.Add("p_rolenazev", OracleDbType.Varchar2).Value = uzivatel.Rolenazev;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при добавлении пользователя");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при добавлении пользователя: {ex.Message}");
                }
            }

            ViewBag.Roles = GetRoles();
            return View(uzivatel);
        }
        // Метод для отображения формы редактирования пользователя
        public async Task<IActionResult> Edit(int id)
        {
            RegisUzivatel? uzivatel = null;

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("ZOBRAZ_REGISUZIVATELE_PODLE_ID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Входной параметр - ID пользователя
                        command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

                        // Выходной параметр - курсор
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                uzivatel = new RegisUzivatel
                                {
                                    IdRegisUzivatelu = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Jmeno = reader.GetString(3),
                                    Prijmeni = reader.GetString(4),
                                    Heslo = reader.GetString(5),
                                    DatumVytvoreni = reader.GetDateTime(6),
                                    DatumAktualizace = reader.GetDateTime(7),
                                    PosledniPrihlaseni = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                                    NeuspesnePrihlaseni = reader.GetInt32(9),
                                    RoleIdRole = reader.GetInt32(10),
                                    Rolenazev = reader.GetString(11)
                                };
                            }
                        }
                    }
                }

                if (uzivatel == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при получении пользователя для редактирования");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при получении пользователя для редактирования: {ex.Message}");
                return RedirectToAction("Index");
            }

            ViewBag.Roles = GetRoles();
            return View(uzivatel);
        }

        // Метод для обработки редактирования пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegisUzivatel uzivatel)
        {
            if (id != uzivatel.IdRegisUzivatelu)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Обновим время обновления
                    uzivatel.DatumAktualizace = DateTime.UtcNow;

                    // Если поле пароля не пустое, перехэшируем пароль
                    if (!string.IsNullOrEmpty(uzivatel.Heslo))
                    {
                        byte[] salt = new byte[16]; // 16 байт
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(salt);
                        }

                        // Хэшируем пароль
                        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: uzivatel.Heslo,
                            salt: salt,
                            prf: KeyDerivationPrf.HMACSHA256,
                            iterationCount: 10000,
                            numBytesRequested: 32 // 32 байта хэша
                        ));

                        // Объединяем соль и хэш для хранения
                        string saltBase64 = Convert.ToBase64String(salt);
                        uzivatel.Heslo = $"{saltBase64}:{hashedPassword}";
                    }

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("AKTUALIZUJ_REGISUZIVATELE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_idregisuzivatelu", OracleDbType.Int32).Value = uzivatel.IdRegisUzivatelu;
                            command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = uzivatel.Username;
                            command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = uzivatel.Email;
                            command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = uzivatel.Jmeno;
                            command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = uzivatel.Prijmeni;
                            // Передаем уже захэшированный пароль (или незатронутый, если его не меняли)
                            command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = uzivatel.Heslo;
                            command.Parameters.Add("p_role_idrole", OracleDbType.Int32).Value = uzivatel.RoleIdRole;
                            command.Parameters.Add("p_rolenazev", OracleDbType.Varchar2).Value = uzivatel.Rolenazev;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении пользователя");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при обновлении пользователя: {ex.Message}");
                }
            }

            ViewBag.Roles = GetRoles();
            return View(uzivatel);
        }
        // Метод для обработки удаления пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SMAZ_REGISUZIVATELE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idregisuzivatelu", OracleDbType.Int32).Value = id;

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении пользователя");
                return Json(new { success = false, message = $"Произошла ошибка при удалении пользователя: {ex.Message}" });
            }
        }

        // Метод для получения списка ролей
        private List<SelectListItem> GetRoles()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Administrator", Text = "Administrator" },
                new SelectListItem { Value = "Regis", Text = "Regis" },
                new SelectListItem { Value = "UzivatelBezPrihlaseni", Text = "UzivatelBezPrihlaseni" }
            };
        }
    }
}




