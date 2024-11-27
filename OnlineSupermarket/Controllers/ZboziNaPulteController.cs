using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class ZboziNaPulteController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ZboziNaPulteController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            try
            {
                string sql = "SELECT * FROM ZBOZINAPULTE";
                var dataTable = _dbHelper.ExecuteQuery(sql);
                var zboziNaPulteList = _dbHelper.MapZboziNaPulte(dataTable);
                return View(zboziNaPulteList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading data: " + ex.Message;
                return View(new List<ZboziNaPulte>());
            }
        }

        public IActionResult Create()
        {
            return View(new ZboziNaPulte());
        }

        [HttpPost]
        public IActionResult Create(ZboziNaPulte zboziNaPulte)
        {
            if (!ModelState.IsValid)
            {
                return View(zboziNaPulte);
            }

            try
            {
                string sql = "INSERT INTO ZBOZINAPULTE (PULT_IDPULTU, ZBOZI_IDZBOZI, POCET) " +
                             "VALUES (:PultIdPultu, :ZboziIdZbozi, :Pocet)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("PultIdPultu", OracleDbType.Int32) { Value = zboziNaPulte.PultIdPultu },
                    new OracleParameter("ZboziIdZbozi", OracleDbType.Int32) { Value = zboziNaPulte.ZboziIdZbozi },
                    new OracleParameter("Pocet", OracleDbType.Int32) { Value = zboziNaPulte.Pocet }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error creating record: " + ex.Message;
                return View(zboziNaPulte);
            }
        }
    }
}
