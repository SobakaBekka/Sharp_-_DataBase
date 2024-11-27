using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;

namespace OnlineSupermarket.Controllers
{
    public class RegisUzivatelController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly IPasswordHasher<RegisUzivatel> _passwordHasher;
        private readonly ILogger<RegisUzivatelController> _logger;

        public RegisUzivatelController(DatabaseHelper dbHelper, IPasswordHasher<RegisUzivatel> passwordHasher, ILogger<RegisUzivatelController> logger)
        {
            _dbHelper = dbHelper;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        // Проверка подключения к базе данных
        private bool CheckDatabaseConnection()
        {
            try
            {
                string sql = "SELECT 1 FROM DUAL"; // Простой запрос для проверки соединения
                var result = _dbHelper.ExecuteScalar(sql);
                return result != null;
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Ошибка подключения к базе данных.");
                return false;
            }
        }

        // GET: RegisUzivatel/Index
        public IActionResult Index()
        {
            if (!CheckDatabaseConnection())
            {
                TempData["ErrorMessage"] = "Не удалось подключиться к базе данных. Проверьте настройки подключения.";
                return View(new List<RegisUzivatel>());
            }

            try
            {
                string sql = "SELECT * FROM ST69704.RegisUzivatel";
                var dataTable = _dbHelper.ExecuteQuery(sql);
                List<RegisUzivatel> users = _dbHelper.MapRegisUzivatel(dataTable);

                // Передача сообщений TempData в ViewBag
                if (TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }
                if (TempData["ErrorMessage"] != null)
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
                }

                return View(users);
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка пользователей.");
                ModelState.AddModelError(string.Empty, "Произошла ошибка при загрузке списка пользователей.");
                return View(new List<RegisUzivatel>());
            }
        }

        // GET: RegisUzivatel/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: RegisUzivatel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegisUzivatel model)
        {
            if (!CheckDatabaseConnection())
            {
                TempData["ErrorMessage"] = "Не удалось подключиться к базе данных. Проверьте настройки подключения.";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Логируем данные модели для отладки
                    _logger.LogInformation("Регистрация пользователя. Полученные данные: {Jmeno}, {Prijmeni}, {HesloHash}",
                                           model.Jmeno, model.Prijmeni, model.HesloHash);

                    // Хеширование пароля перед сохранением
                    var hashedPassword = _passwordHasher.HashPassword(model, model.HesloHash);
                    model.HesloHash = hashedPassword;

                    // Установка значений по умолчанию для обязательных полей
                    model.RoleIdRole = 1; // Например, роль пользователя
                    model.SouborIdSouboru = 1; // Например, ID файла
                    model.IdSouboru = 1; // Например, ID файла

                    // Логируем SQL-запрос перед его выполнением
                    string sql = "INSERT INTO ST69704.RegisUzivatel (JMENO, PRIJMENI, HESLOHASH, HESLOSUL, ROLE_IDROLE, SOUBOR_IDSOUBORU, IDSOUBORU) " +
                                 "VALUES (:JMENO, :PRIJMENI, :HESLOHASH, :HESLOSUL, :ROLE_IDROLE, :SOUBOR_IDSOUBORU, :IDSOUBORU)";

                    var parameters = new OracleParameter[]
                    {
                        new OracleParameter("JMENO", model.Jmeno),
                        new OracleParameter("PRIJMENI", model.Prijmeni),
                        new OracleParameter("HESLOHASH", model.HesloHash),
                        new OracleParameter("HESLOSUL", model.HesloSul), // Соль пароля
                        new OracleParameter("ROLE_IDROLE", model.RoleIdRole),
                        new OracleParameter("SOUBOR_IDSOUBORU", model.SouborIdSouboru),
                        new OracleParameter("IDSOUBORU", model.IdSouboru)
                    };

                    // Выполнение запроса
                    int rowsAffected = _dbHelper.ExecuteNonQuery(sql, parameters);
                    _logger.LogInformation("Запрос выполнен. Количество затронутых строк: {rowsAffected}", rowsAffected);

                    // Проверка результата выполнения
                    if (rowsAffected > 0)
                    {
                        _logger.LogInformation("Пользователь {Jmeno} успешно зарегистрирован.", model.Jmeno);
                        TempData["SuccessMessage"] = "Регистрация прошла успешно.";
                    }
                    else
                    {
                        _logger.LogWarning("Не удалось зарегистрировать пользователя {Jmeno}. Количество затронутых строк: 0", model.Jmeno);
                        TempData["ErrorMessage"] = "Не удалось зарегистрировать пользователя. Попробуйте еще раз.";
                    }

                    return RedirectToAction("Index");
                }
                catch (OracleException ex)
                {
                    _logger.LogError(ex, "Ошибка при регистрации пользователя.");
                    TempData["ErrorMessage"] = "Произошла ошибка при регистрации пользователя.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Неизвестная ошибка при регистрации пользователя.");
                    TempData["ErrorMessage"] = "Произошла неизвестная ошибка при регистрации пользователя.";
                }
            }

            TempData["ErrorMessage"] = "Проверьте правильность введенных данных.";
            return View(model);
        }
    }
}
