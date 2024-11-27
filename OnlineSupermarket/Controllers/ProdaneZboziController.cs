using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class ProdaneZboziController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ProdaneZboziController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM PRODANEZBOZI";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var prodaneZboziList = _dbHelper.MapProdaneZbozi(dataTable);

            return View(prodaneZboziList);
        }

        public IActionResult Create()
        {
            return View(new ProdaneZbozi());
        }

        [HttpPost]
        public IActionResult Create(ProdaneZbozi prodaneZbozi)
        {
            if (!ModelState.IsValid)
            {
                return View(prodaneZbozi);
            }

            try
            {
                string sql = "INSERT INTO PRODANEZBOZI (POCET, PRODEJNICENA, ZBOZI_IDZBOZI, PRODEJ_IDPRODEJE, PRODEJ_ZBOZI_IDZBOZI, IDTRANZAKCE) " +
                             "VALUES (:Pocet, :ProdejniCena, :ZboziIdZbozi, :ProdejIdProdeje, :ProdejZboziIdZbozi, :IdTranzakce)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Pocet", OracleDbType.Int32) { Value = prodaneZbozi.Pocet },
                    new OracleParameter("ProdejniCena", OracleDbType.Decimal) { Value = prodaneZbozi.ProdejniCena },
                    new OracleParameter("ZboziIdZbozi", OracleDbType.Int32) { Value = prodaneZbozi.ZboziIdZbozi },
                    new OracleParameter("ProdejIdProdeje", OracleDbType.Int32) { Value = prodaneZbozi.ProdejIdProdeje },
                    new OracleParameter("ProdejZboziIdZbozi", OracleDbType.Int32) { Value = prodaneZbozi.ProdejZboziIdZbozi },
                    new OracleParameter("IdTranzakce", OracleDbType.Int32) { Value = prodaneZbozi.IdTranzakce }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(prodaneZbozi);
            }
        }
    }
}
