using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class ZboziNaSkladeController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ZboziNaSkladeController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            try
            {
                string sql = "SELECT * FROM ZBOZINASKLADE";
                var dataTable = _dbHelper.ExecuteQuery(sql);
                var zboziNaSkladeList = _dbHelper.MapZboziNaSklade(dataTable);
                return View(zboziNaSkladeList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading data: " + ex.Message;
                return View(new List<ZboziNaSklade>());
            }
        }

        public IActionResult Create()
        {
            return View(new ZboziNaSklade());
        }

        [HttpPost]
        public IActionResult Create(ZboziNaSklade zboziNaSklade)
        {
            if (!ModelState.IsValid)
            {
                return View(zboziNaSklade);
            }

            try
            {
                string sql = "INSERT INTO ZBOZINASKLADE (ZBOZI_IDZBOZI, SKLAD_IDSKLADU, POCET, ADRESA_IDADRESY, KATEGORIE_IDKATEGORII) " +
                             "VALUES (:ZboziIdZbozi, :SkladIdSkladu, :Pocet, :AdresaIdAdresy, :KategorieIdKategorii)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("ZboziIdZbozi", OracleDbType.Int32) { Value = zboziNaSklade.ZboziIdZbozi },
                    new OracleParameter("SkladIdSkladu", OracleDbType.Int32) { Value = zboziNaSklade.SkladIdSkladu },
                    new OracleParameter("Pocet", OracleDbType.Int32) { Value = zboziNaSklade.Pocet },
                    new OracleParameter("AdresaIdAdresy", OracleDbType.Int32) { Value = (object)zboziNaSklade.AdresaIdAdresy ?? DBNull.Value },
                    new OracleParameter("KategorieIdKategorii", OracleDbType.Int32) { Value = zboziNaSklade.KategorieIdKategorii }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error creating record: " + ex.Message;
                return View(zboziNaSklade);
            }
        }
    }
}
