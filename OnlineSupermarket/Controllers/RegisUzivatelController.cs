using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class RegisUzivatelController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public RegisUzivatelController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM REGISUZIVATEL";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var regisUzivatelList = _dbHelper.MapRegisUzivatel(dataTable);

            return View(regisUzivatelList);
        }

        public IActionResult Create()
        {
            return View(new RegisUzivatel());
        }

        [HttpPost]
        public IActionResult Create(RegisUzivatel regisUzivatel)
        {
            if (!ModelState.IsValid)
            {
                return View(regisUzivatel);
            }

            try
            {
                string sql = "INSERT INTO REGISUZIVATEL (JMENO, PRIJMENI, HESLOHASH, HESLOSUL, ROLE_IDROLE, SOUBOR_IDSOUBORU, IDSOUBORU) " +
                             "VALUES (:Jmeno, :Prijmeni, :HesloHash, :HesloSul, :RoleIdRole, :SouborIdSouboru, :IdSouboru)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Jmeno", OracleDbType.Varchar2) { Value = regisUzivatel.Jmeno },
                    new OracleParameter("Prijmeni", OracleDbType.Varchar2) { Value = regisUzivatel.Prijmeni },
                    new OracleParameter("HesloHash", OracleDbType.Varchar2) { Value = regisUzivatel.HesloHash },
                    new OracleParameter("HesloSul", OracleDbType.Varchar2) { Value = regisUzivatel.HesloSul },
                    new OracleParameter("RoleIdRole", OracleDbType.Int32) { Value = regisUzivatel.RoleIdRole },
                    new OracleParameter("SouborIdSouboru", OracleDbType.Int32) { Value = regisUzivatel.SouborIdSouboru },
                    new OracleParameter("IdSouboru", OracleDbType.Int32) { Value = regisUzivatel.IdSouboru }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(regisUzivatel);
            }
        }
    }
}
