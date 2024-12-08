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


        public async Task<IActionResult> Host()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Username = username;
            ViewBag.Role = role;

            var zboziKategorieList = await GetZboziKategorieList();
            return View(zboziKategorieList);
        }




        public async Task<IActionResult> Index()
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

            var zboziKategorieList = await GetZboziKategorieList();
            return View(zboziKategorieList);
        }



        public async Task<IActionResult> AdminIndex()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Username = username;
            ViewBag.Role = role;

            var zboziKategorieList = await GetZboziKategorieList();
            return View(zboziKategorieList);
        }



        public async Task<IActionResult> AdminHost()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Username = username;
            ViewBag.Role = role;

            var zboziKategorieList = await GetZboziKategorieList();
            return View(zboziKategorieList);
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





        public async Task<List<ZboziKategorieViewModel>> GetZboziKategorieList()
        {
            List<ZboziKategorieViewModel> zboziKategorieList = new List<ZboziKategorieViewModel>();

            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SELECT IDZBOZI, ZboziNazev, AKTUALNICENA, CENAZEKLUBKARTOU, HMOTNOST, SLOZENI, KategorieNazev FROM ZboziKategorieView", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var zboziKategorie = new ZboziKategorieViewModel
                                {
                                    IDZbozi = reader.GetInt32(reader.GetOrdinal("IDZBOZI")),
                                    ZboziNazev = reader.GetString(reader.GetOrdinal("ZboziNazev")),
                                    AktualniCena = reader.GetDecimal(reader.GetOrdinal("AKTUALNICENA")),
                                    CenaZeKlubKartou = reader.IsDBNull(reader.GetOrdinal("CENAZEKLUBKARTOU")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("CENAZEKLUBKARTOU")),
                                    Hmotnost = reader.GetDecimal(reader.GetOrdinal("HMOTNOST")),
                                    Slozeni = reader.GetString(reader.GetOrdinal("SLOZENI")),
                                    KategorieNazev = reader.GetString(reader.GetOrdinal("KategorieNazev")),
                                    ImageFileName = $"{reader.GetInt32(reader.GetOrdinal("IDZBOZI"))}.jpg" // Установите имя файла изображения
                                };
                                zboziKategorieList.Add(zboziKategorie);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Došlo k chybě při získávání dat z pohledu ZboziKategorieView");
            }

            return zboziKategorieList;
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


        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Autorizace", "Home");
            }

            RegisUzivatel user = null;
            try
            {
                using (var connection = new OracleConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new OracleCommand("SELECT * FROM REGISUZIVATEL WHERE USERNAME = :username", connection))
                    {
                        command.Parameters.Add("username", OracleDbType.Varchar2).Value = username;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                user = new RegisUzivatel
                                {
                                    IdRegisUzivatelu = reader.GetInt32(reader.GetOrdinal("IDREGISUZIVATELU")),
                                    Username = reader.GetString(reader.GetOrdinal("USERNAME")),
                                    Email = reader.GetString(reader.GetOrdinal("EMAIL")),
                                    Jmeno = reader.GetString(reader.GetOrdinal("JMENO")),
                                    Prijmeni = reader.GetString(reader.GetOrdinal("PRIJMENI")),
                                    // Add other fields as necessary
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user profile");
            }

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(RegisUzivatel model)
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
                    using (var command = new OracleCommand("AKTUALIZUJ_REGISUZIVATELE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_idregisuzivatelu", OracleDbType.Int32).Value = model.IdRegisUzivatelu;
                        command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = model.Username;
                        command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = model.Email;
                        command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = model.Jmeno;
                        command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = model.Prijmeni;
                        // Add other parameters as necessary

                        await command.ExecuteNonQueryAsync();
                    }
                }

                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("UserProfile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user profile.");
                ModelState.AddModelError(string.Empty, $"An error occurred while updating the user profile: {ex.Message}");
            }

            return View(model);
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
