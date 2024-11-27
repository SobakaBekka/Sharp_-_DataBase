using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class RoleController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public RoleController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM ROLE";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var roleList = _dbHelper.MapRole(dataTable);

            return View(roleList);
        }

        public IActionResult Create()
        {
            return View(new Role());
        }

        [HttpPost]
        public IActionResult Create(Role role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            try
            {
                string sql = "INSERT INTO ROLE (NAZEV) VALUES (:Nazev)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Nazev", OracleDbType.Varchar2) { Value = role.Nazev }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(role);
            }
        }
    }
}
