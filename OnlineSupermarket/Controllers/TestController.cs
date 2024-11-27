using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using System.Data;

namespace OnlineSupermarket.Controllers
{
    public class TestController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public TestController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM ADRESA";
            DataTable data = _dbHelper.ExecuteQuery(sql);

            // Pass data to the view or process as needed
            return View(data);
        }
    }
}
