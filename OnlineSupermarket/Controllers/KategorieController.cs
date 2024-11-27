using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class KategorieController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public KategorieController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM KATEGORIE";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var kategorieList = _dbHelper.MapKategorie(dataTable);

            return View(kategorieList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Kategorie kategorie)
        {
            try
            {
                string sql = "INSERT INTO KATEGORIE (NAZEV) VALUES (:Nazev)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Nazev", OracleDbType.Varchar2) { Value = kategorie.Nazev }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(kategorie);
            }
        }

        public IActionResult Edit(int id)
        {
            string sql = "SELECT * FROM KATEGORIE WHERE IDKATEGORII = :IdKategorii";
            var parameters = new OracleParameter[]
            {
                new OracleParameter("IdKategorii", OracleDbType.Int32) { Value = id }
            };
            var dataTable = _dbHelper.ExecuteQuery(sql, parameters);

            var kategorieList = _dbHelper.MapKategorie(dataTable);
            var kategorie = kategorieList.Count > 0 ? kategorieList[0] : null;

            return kategorie != null ? View(kategorie) : NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Kategorie kategorie)
        {
            try
            {
                string sql = "UPDATE KATEGORIE SET NAZEV = :Nazev WHERE IDKATEGORII = :IdKategorii";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Nazev", OracleDbType.Varchar2) { Value = kategorie.Nazev },
                    new OracleParameter("IdKategorii", OracleDbType.Int32) { Value = kategorie.IdKategorii }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(kategorie);
            }
        }
    }
}
