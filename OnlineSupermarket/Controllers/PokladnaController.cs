using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class PokladnaController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public PokladnaController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM POKLADNA";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var pokladnaList = _dbHelper.MapPokladna(dataTable);

            return View(pokladnaList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Pokladna pokladna)
        {
            try
            {
                string sql = "INSERT INTO POKLADNA (SAMOOBSLUZNA) VALUES (:Samoobsluzna)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Samoobsluzna", OracleDbType.Int32) { Value = pokladna.Samoobsluzna ? 1 : 0 }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(pokladna);
            }
        }
    }
}
