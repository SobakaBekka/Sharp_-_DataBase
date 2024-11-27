using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class HotoveController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public HotoveController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM HOTOVE";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var hotoveList = _dbHelper.MapHotove(dataTable);

            return View(hotoveList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Hotove hotove)
        {
            try
            {
                string sql = "INSERT INTO HOTOVE (IDTRANZAKCE, VRACENI) VALUES (:IdTranzakce, :Vraceni)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("IdTranzakce", OracleDbType.Int32) { Value = hotove.IdTranzakce },
                    new OracleParameter("Vraceni", OracleDbType.Decimal) { Value = hotove.Vraceni }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(hotove);
            }
        }
    }
}
