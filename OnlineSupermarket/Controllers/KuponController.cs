using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;

namespace OnlineSupermarket.Controllers
{
    public class KuponController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public KuponController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM KUPON";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var kuponList = _dbHelper.MapKupon(dataTable);

            return View(kuponList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Kupon kupon)
        {
            try
            {
                string sql = "INSERT INTO KUPON (IDTRANZAKCE, CISLO) VALUES (:IdTranzakce, :Cislo)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("IdTranzakce", OracleDbType.Int32) { Value = kupon.IdTranzakce },
                    new OracleParameter("Cislo", OracleDbType.Varchar2) { Value = kupon.Cislo }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(kupon);
            }
        }

        public IActionResult Edit(int id)
        {
            // Fetch the Kupon details by ID
            string sql = "SELECT * FROM KUPON WHERE IDTRANZAKCE = :IdTranzakce";
            var parameters = new OracleParameter[]
            {
                new OracleParameter("IdTranzakce", OracleDbType.Int32) { Value = id }
            };

            var dataTable = _dbHelper.ExecuteQuery(sql, parameters);
            var kuponList = _dbHelper.MapKupon(dataTable);
            var kupon = kuponList.Count > 0 ? kuponList[0] : null;

            if (kupon == null)
            {
                return NotFound();
            }

            return View(kupon);
        }

        [HttpPost]
        public IActionResult Edit(Kupon kupon)
        {
            try
            {
                string sql = "UPDATE KUPON SET CISLO = :Cislo WHERE IDTRANZAKCE = :IdTranzakce";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Cislo", OracleDbType.Varchar2) { Value = kupon.Cislo },
                    new OracleParameter("IdTranzakce", OracleDbType.Int32) { Value = kupon.IdTranzakce }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(kupon);
            }
        }
    }
}
