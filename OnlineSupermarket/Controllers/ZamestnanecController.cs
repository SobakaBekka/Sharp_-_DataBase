using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class ZamestnanecController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ZamestnanecController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM ZAMESTANEC";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var zamestnanecList = _dbHelper.MapZamestnanec(dataTable);

            return View(zamestnanecList);
        }

        public IActionResult Create()
        {
            return View(new Zamestnanec());
        }

        [HttpPost]
        public IActionResult Create(Zamestnanec zamestnanec)
        {
            if (!ModelState.IsValid)
            {
                return View(zamestnanec);
            }

            try
            {
                string sql = "INSERT INTO ZAMESTANEC (JMENO, PRIJMENI, RODNECISLO, TELEFONICISLO, POZICE_IDPOZICE, PRODEJNA_IDPRODEJNY, IDNADREZENE) " +
                             "VALUES (:Jmeno, :Prijmeni, :RodneCislo, :TelefonniCislo, :PoziceIdPozice, :ProdejnaIdProdejny, :IdNadrezene)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Jmeno", OracleDbType.Varchar2) { Value = zamestnanec.Jmeno },
                    new OracleParameter("Prijmeni", OracleDbType.Varchar2) { Value = zamestnanec.Prijmeni },
                    new OracleParameter("RodneCislo", OracleDbType.Varchar2) { Value = zamestnanec.RodneCislo },
                    new OracleParameter("TelefonniCislo", OracleDbType.Int64) { Value = zamestnanec.TelefonniCislo },
                    new OracleParameter("PoziceIdPozice", OracleDbType.Int32) { Value = zamestnanec.PoziceIdPozice },
                    new OracleParameter("ProdejnaIdProdejny", OracleDbType.Int32) { Value = zamestnanec.ProdejnaIdProdejny },
                    new OracleParameter("IdNadrezene", OracleDbType.Int32) { Value = (object)zamestnanec.IdNadrezene ?? DBNull.Value }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(zamestnanec);
            }
        }
    }
}
