using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class KartaController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public KartaController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM KARTA";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var kartaList = _dbHelper.MapKarta(dataTable);

            return View(kartaList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Karta karta)
        {
            try
            {
                string sql = "INSERT INTO KARTA (IDTRANZAKCE, AUTORIZACNIKOD, CISLO) VALUES (:IdTranzakce, :AutorizacniKod, :Cislo)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("IdTranzakce", OracleDbType.Int32) { Value = karta.IdTranzakce },
                    new OracleParameter("AutorizacniKod", OracleDbType.Int32) { Value = karta.AutorizacniKod },
                    new OracleParameter("Cislo", OracleDbType.Int32) { Value = karta.Cislo }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(karta);
            }
        }
    }
}
