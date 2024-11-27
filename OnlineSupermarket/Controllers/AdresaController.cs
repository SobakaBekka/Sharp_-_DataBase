using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class AdresaController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public AdresaController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM ADRESA";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var adresaList = _dbHelper.MapAdresa(dataTable);

            return View(adresaList);
        }

        // Implement additional CRUD actions as needed
    }
}
