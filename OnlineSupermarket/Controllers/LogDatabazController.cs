using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class LogDatabazController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public LogDatabazController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM LOGDATABAZ ORDER BY DATUM DESC";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var logList = _dbHelper.MapLogDatabaz(dataTable);

            return View(logList);
        }
    }
}
