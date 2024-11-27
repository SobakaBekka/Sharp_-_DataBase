using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class PoziceController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public PoziceController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Display all Pozice records
        public IActionResult Index()
        {
            try
            {
                string sql = "SELECT * FROM POZICE";
                var dataTable = _dbHelper.ExecuteQuery(sql);
                var poziceList = _dbHelper.MapPozice(dataTable);

                return View(poziceList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error fetching data: " + ex.Message;
                return View(new List<Pozice>());
            }
        }

        // GET: Create Pozice
        public IActionResult Create()
        {
            // Pass a new instance of Pozice to prevent null reference errors
            return View(new Pozice());
        }

        // POST: Create Pozice
        [HttpPost]
        public IActionResult Create(Pozice pozice)
        {
            if (!ModelState.IsValid)
            {
                // If model validation fails, return to the view with the current model
                return View(pozice);
            }

            try
            {
                string sql = "INSERT INTO POZICE (NAZEV, MZDA) VALUES (:Nazev, :Mzda)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Nazev", OracleDbType.Varchar2) { Value = pozice.Nazev },
                    new OracleParameter("Mzda", OracleDbType.Decimal) { Value = pozice.Mzda }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle database errors and return the user to the Create view
                ViewBag.ErrorMessage = "Error creating record: " + ex.Message;
                return View(pozice);
            }
        }

        // GET: Edit Pozice
        public IActionResult Edit(int id)
        {
            try
            {
                string sql = "SELECT * FROM POZICE WHERE IDPOZICE = :IdPozice";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("IdPozice", OracleDbType.Int32) { Value = id }
                };

                var dataTable = _dbHelper.ExecuteQuery(sql, parameters);
                var poziceList = _dbHelper.MapPozice(dataTable);
                var pozice = poziceList.Count > 0 ? poziceList[0] : null;

                if (pozice == null)
                {
                    return NotFound();
                }

                return View(pozice);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error fetching record: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Edit Pozice
        [HttpPost]
        public IActionResult Edit(Pozice pozice)
        {
            if (!ModelState.IsValid)
            {
                return View(pozice);
            }

            try
            {
                string sql = "UPDATE POZICE SET NAZEV = :Nazev, MZDA = :Mzda WHERE IDPOZICE = :IdPozice";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Nazev", OracleDbType.Varchar2) { Value = pozice.Nazev },
                    new OracleParameter("Mzda", OracleDbType.Decimal) { Value = pozice.Mzda },
                    new OracleParameter("IdPozice", OracleDbType.Int32) { Value = pozice.IdPozice }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error updating record: " + ex.Message;
                return View(pozice);
            }
        }
    }
}
