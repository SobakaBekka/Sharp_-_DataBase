using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class PlatbaController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public PlatbaController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM PLATBA";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var platbaList = _dbHelper.MapPlatba(dataTable);

            return View(platbaList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Platba platba)
        {
            try
            {
                string sql = "INSERT INTO PLATBA (CELKOVACENA, PRODEJ_IDPRODEJE, PRODEJ_ZBOZI_IDZBOZI, TYP) " +
                             "VALUES (:CelkovaCena, :ProdejIdProdeje, :ProdejZboziIdZbozi, :Typ)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("CelkovaCena", OracleDbType.Decimal) { Value = platba.CelkovaCena },
                    new OracleParameter("ProdejIdProdeje", OracleDbType.Int32) { Value = platba.ProdejIdProdeje },
                    new OracleParameter("ProdejZboziIdZbozi", OracleDbType.Int32) { Value = platba.ProdejZboziIdZbozi },
                    new OracleParameter("Typ", OracleDbType.Varchar2) { Value = platba.Typ }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(platba);
            }
        }
    }
}
