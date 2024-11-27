using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class PultController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public PultController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM PULT";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var pultList = _dbHelper.MapPult(dataTable);

            return View(pultList);
        }

        public IActionResult Create()
        {
            return View(new Pult());
        }

        [HttpPost]
        public IActionResult Create(Pult pult)
        {
            if (!ModelState.IsValid)
            {
                return View(pult);
            }

            try
            {
                string sql = "INSERT INTO PULT (CISLO, POCETPOICEK, PRODEJNA_IDPRODEJNY, IDKATEGORII) " +
                             "VALUES (:Cislo, :PocetPoicek, :ProdejnaIdProdejny, :IdKategorii)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Cislo", OracleDbType.Varchar2) { Value = pult.Cislo },
                    new OracleParameter("PocetPoicek", OracleDbType.Int32) { Value = pult.PocetPoicek },
                    new OracleParameter("ProdejnaIdProdejny", OracleDbType.Int32) { Value = (object)pult.ProdejnaIdProdejny ?? DBNull.Value },
                    new OracleParameter("IdKategorii", OracleDbType.Int32) { Value = (object)pult.IdKategorii ?? DBNull.Value }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(pult);
            }
        }
    }
}
