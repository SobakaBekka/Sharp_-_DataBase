using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using System.Diagnostics;

namespace OnlineSupermarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult TestConnection()
        {
            try
            {
                // Перевірка підключення шляхом виконання запиту на підрахунок записів у таблиці
                var count = _context.Zbozi.Count();  // Використовуйте будь-яку з ваших таблиць
                ViewBag.Message = $"Підключення успішне. Кількість записів у таблиці Zbozi: {count}";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Помилка підключення: {ex.Message}";
            }

            return View();
        }
    }
}
