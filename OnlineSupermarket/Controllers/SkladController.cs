using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class SkladController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public SkladController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM SKLAD";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var skladList = _dbHelper.MapSklad(dataTable);

            return View(skladList);
        }

        public IActionResult Create()
        {
            return View(new Sklad());
        }

        [HttpPost]
        public IActionResult Create(Sklad sklad)
        {
            if (!ModelState.IsValid)
            {
                return View(sklad);
            }

            try
            {
                string sql = "INSERT INTO SKLAD (POCETPOLICEK, PLOCHA) VALUES (:PocetPolicek, :Plocha)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("PocetPolicek", OracleDbType.Int32) { Value = sklad.PocetPolicek },
                    new OracleParameter("Plocha", OracleDbType.Decimal) { Value = (object)sklad.Plocha ?? DBNull.Value }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(sklad);
            }
        }
    }
}
