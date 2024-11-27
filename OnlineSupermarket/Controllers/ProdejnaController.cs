using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class ProdejnaController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ProdejnaController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM PRODEJNA";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var prodejnaList = _dbHelper.MapProdejna(dataTable);

            return View(prodejnaList);
        }

        public IActionResult Create()
        {
            return View(new Prodejna());
        }

        [HttpPost]
        public IActionResult Create(Prodejna prodejna)
        {
            if (!ModelState.IsValid)
            {
                return View(prodejna);
            }

            try
            {
                string sql = "INSERT INTO PRODEJNA (KONTAKTNICISLO, PLOCHA, POKLADNA_IDPOKLADNY) " +
                             "VALUES (:KontaktniCislo, :Plocha, :PokladnaIdPokladny)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("KontaktniCislo", OracleDbType.Int32) { Value = prodejna.KontaktniCislo },
                    new OracleParameter("Plocha", OracleDbType.Decimal) { Value = prodejna.Plocha },
                    new OracleParameter("PokladnaIdPokladny", OracleDbType.Int32) { Value = (object)prodejna.PokladnaIdPokladny ?? DBNull.Value }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(prodejna);
            }
        }
    }
}
