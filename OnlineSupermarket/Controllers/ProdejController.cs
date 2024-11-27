using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class ProdejController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ProdejController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM PRODEJ";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var prodejList = _dbHelper.MapProdej(dataTable);

            return View(prodejList);
        }

        public IActionResult Create()
        {
            return View(new Prodej { Datum = DateTime.Now });
        }

        [HttpPost]
        public IActionResult Create(Prodej prodej)
        {
            if (!ModelState.IsValid)
            {
                return View(prodej);
            }

            try
            {
                string sql = "INSERT INTO PRODEJ (DATUM, CELKOVACENA, ZBOZI_IDZBOZI, PLATBA_IDTRANZAKCE) " +
                             "VALUES (:Datum, :CelkovaCena, :ZboziIdZbozi, :PlatbaIdTranzakce)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Datum", OracleDbType.Date) { Value = prodej.Datum },
                    new OracleParameter("CelkovaCena", OracleDbType.Decimal) { Value = prodej.CelkovaCena },
                    new OracleParameter("ZboziIdZbozi", OracleDbType.Int32) { Value = prodej.ZboziIdZbozi },
                    new OracleParameter("PlatbaIdTranzakce", OracleDbType.Int32) { Value = (object)prodej.PlatbaIdTranzakce ?? DBNull.Value }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(prodej);
            }
        }
    }
}
