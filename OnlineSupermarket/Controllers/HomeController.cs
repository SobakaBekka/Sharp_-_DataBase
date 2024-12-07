using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;

namespace OnlineSupermarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Username = username;
            ViewBag.Role = role;

            // Проверка наличия аккаунта
            if (string.IsNullOrEmpty(username))
            {
                TempData["AccountCheckMessage"] = "У вас есть аккаунт?";
                return RedirectToAction("KontrolaAkaunta", "Home");
            }

            return View();
        }


        public IActionResult KontrolaAkaunta()
        {
            return View();
        }





        [HttpPost]
        public async Task<IActionResult> KontrolaAkaunta(string hasAccount)
        {
            if (hasAccount == "yes")
            {
                return RedirectToAction("Autorizace", "Home");
            }
            else
            {
                // Создание нового пользователя с данными "Host"
                var username = "Host";
                var email = "host@example.com";
                var jmeno = "Host";
                var prijmeni = "Host";
                var heslo = "Host";
                var rolenazev = "UzivatelBezPrihlaseni";
                var roleidrole = 3;

                try
                {
                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (var command = new OracleCommand("VLOZ_REGISUZIVATELE_AUTOMATIC_ROLE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = username;
                            command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
                            command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = jmeno;
                            command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = prijmeni;
                            command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = heslo;
                            command.Parameters.Add("p_rolenazev", OracleDbType.Varchar2).Value = rolenazev;
                            command.Parameters.Add("p_roleidrole", OracleDbType.Int32).Value = roleidrole;

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    // Установите данные сессии для пользователя
                    HttpContext.Session.SetString("Username", username);
                    HttpContext.Session.SetString("Role", rolenazev);

                    TempData["SuccessMessage"] = "Пользователь Host успешно создан.";
                    return RedirectToAction("Host", "Home");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при создании пользователя Host");
                    TempData["ErrorMessage"] = $"Произошла ошибка при создании пользователя Host: {ex.Message}";
                    return RedirectToAction("Host", "Home");
                }
            }
        }







        public async Task SmazatHostUzivatele()
        {
            var username = "Host";

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("DELETE FROM RegisUzivatel WHERE Username = :username", connection))
                    {
                        command.Parameters.Add("username", OracleDbType.Varchar2).Value = username;
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при удалении пользователя Host");
            }
        }






        public IActionResult Host()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Username = username;
            ViewBag.Role = role;

            return View();
        }





        public IActionResult AdminIndex()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Username = username;
            ViewBag.Role = role;
            return View();
        }

        public IActionResult AdminHost()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Username = username;
            ViewBag.Role = role;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Метод для отображения формы регистрации
        public IActionResult Registrace()
        {
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrace(RegisUzivatel regisUzivatel)
        {
            // Удаление пользователя "Host" перед регистрацией нового пользователя
            await SmazatHostUzivatele();

            // Устанавливаем значения для роли
            regisUzivatel.Rolenazev = "Regis";
            regisUzivatel.RoleIdRole = 2; // значение по умолчанию

            // Удаляем ошибку валидации для Rolenazev, так как задаём его программно
            ModelState.Remove(nameof(regisUzivatel.Rolenazev));

            if (ModelState.IsValid)
            {
                try
                {
                    // Проверяем, что пароль не пуст. Если нужно условие, что пользователь обязательно должен ввести пароль,
                    // то это может быть предусмотрено валидацией модели.
                    if (!string.IsNullOrEmpty(regisUzivatel.Heslo))
                    {
                        // Генерируем соль для пароля
                        byte[] salt = new byte[16];
                        using (var rng = RandomNumberGenerator.Create())
                        {
                            rng.GetBytes(salt);
                        }

                        // Хэшируем пароль с использованием PBKDF2
                        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: regisUzivatel.Heslo,
                            salt: salt,
                            prf: KeyDerivationPrf.HMACSHA256,
                            iterationCount: 10000, // Количество итераций можно скорректировать при необходимости
                            numBytesRequested: 32 // 32 байта
                        ));

                        // Объединяем соль и хэш для хранения
                        string saltBase64 = Convert.ToBase64String(salt);
                        regisUzivatel.Heslo = $"{saltBase64}:{hashedPassword}";
                    }

                    using (var connection = new OracleConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        _logger.LogInformation("Подключение к базе данных открыто.");

                        using (var command = new OracleCommand("VLOZ_REGISUZIVATELE", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.BindByName = true;

                            // Передаём уже хэшированный пароль
                            command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = regisUzivatel.Username;
                            command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = regisUzivatel.Email;
                            command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = regisUzivatel.Jmeno;
                            command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = regisUzivatel.Prijmeni;
                            command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = regisUzivatel.Heslo;
                            command.Parameters.Add("p_role_idrole", OracleDbType.Int32).Value = regisUzivatel.RoleIdRole;
                            command.Parameters.Add("p_rolenazev", OracleDbType.Varchar2).Value = regisUzivatel.Rolenazev;

                            _logger.LogInformation("Выполнение процедуры VLOZ_REGISUZIVATELE.");
                            await command.ExecuteNonQueryAsync();
                            _logger.LogInformation("Процедура выполнена успешно.");
                        }
                    }

                    TempData["SuccessMessage"] = "Регистрация прошла успешно. Пожалуйста, авторизуйтесь.";
                    return RedirectToAction("Autorizace", "Home");
                }
                catch (OracleException oracleEx)
                {
                    _logger.LogError(oracleEx, "Произошла ошибка при выполнении процедуры в Oracle.");
                    ModelState.AddModelError(string.Empty, $"Ошибка базы данных: {oracleEx.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при регистрации пользователя");
                    ModelState.AddModelError(string.Empty, $"Произошла ошибка при регистрации пользователя: {ex.Message}");
                }
            }

            return View(regisUzivatel);
        }








        // Метод для отображения формы авторизации
        public IActionResult Autorizace()
        {
            return View();
        }

        // Метод для обработки авторизации пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Autorizace(Autorizace model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    _logger.LogInformation("Подключение к базе данных открыто.");

                    using (var command = new OracleCommand("SELECT HESLO, ROLENAZEV FROM REGISUZIVATEL WHERE USERNAME = :username", connection))
                    {
                        command.Parameters.Add("username", OracleDbType.Varchar2).Value = model.Username;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var storedPasswordHash = reader.GetString(reader.GetOrdinal("HESLO"));
                                var roleName = reader.GetString(reader.GetOrdinal("ROLENAZEV"));
                                var parts = storedPasswordHash.Split(':');
                                if (parts.Length == 2)
                                {
                                    var salt = Convert.FromBase64String(parts[0]);
                                    var storedHash = parts[1];

                                    // Хэшируем введенный пароль с использованием той же соли
                                    string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                        password: model.Heslo,
                                        salt: salt,
                                        prf: KeyDerivationPrf.HMACSHA256,
                                        iterationCount: 10000,
                                        numBytesRequested: 32
                                    ));

                                    if (hashedPassword == storedHash)
                                    {
                                        // Авторизация успешна
                                        _logger.LogInformation("Пользователь успешно авторизован.");
                                        HttpContext.Session.SetString("Username", model.Username);
                                        HttpContext.Session.SetString("Role", roleName);
                                        return RedirectToAction("Index", "Home");
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль.");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при авторизации пользователя");
                ModelState.AddModelError(string.Empty, $"Произошла ошибка при авторизации пользователя: {ex.Message}");
            }

            return View(model);
        }

        public IActionResult AdminPanel()
        {
            return View();
        }
    }
}
