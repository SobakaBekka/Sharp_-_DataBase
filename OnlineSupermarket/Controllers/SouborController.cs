using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.IO;

namespace OnlineSupermarket.Controllers
{
    public class SouborController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public SouborController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IActionResult Index()
        {
            string sql = "SELECT * FROM SOUBOR";
            var dataTable = _dbHelper.ExecuteQuery(sql);
            var souborList = _dbHelper.MapSoubor(dataTable);

            return View(souborList);
        }

        public IActionResult Upload()
        {
            // Pass an initialized model to the view
            var model = new Soubor
            {
                UploadDate = DateTime.Now, // Default initialization
                ModifyDate = DateTime.Now // Default initialization
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Upload(Soubor soubor, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Vyberte soubor pro nahrání.");
                return View(soubor);
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    soubor.SouborContent = memoryStream.ToArray();
                }

                soubor.UploadDate = DateTime.Now;
                soubor.ModifyDate = DateTime.Now;
                soubor.Pripona = Path.GetExtension(file.FileName);
                soubor.TypSouboru = file.ContentType;

                string sql = "INSERT INTO SOUBOR (NAZEV, SOUBOR, PRIPONA, UPLOADDATE, MODIFYDATE, TYPSOUBORU, REGISUZIVATEL_IDREGISUZIVATELU) " +
                             "VALUES (:Nazev, :SouborContent, :Pripona, :UploadDate, :ModifyDate, :TypSouboru, :RegisUzivatelId)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Nazev", OracleDbType.Varchar2) { Value = soubor.Nazev },
                    new OracleParameter("SouborContent", OracleDbType.Blob) { Value = soubor.SouborContent },
                    new OracleParameter("Pripona", OracleDbType.Varchar2) { Value = soubor.Pripona },
                    new OracleParameter("UploadDate", OracleDbType.Date) { Value = soubor.UploadDate },
                    new OracleParameter("ModifyDate", OracleDbType.Date) { Value = soubor.ModifyDate },
                    new OracleParameter("TypSouboru", OracleDbType.Varchar2) { Value = soubor.TypSouboru },
                    new OracleParameter("RegisUzivatelId", OracleDbType.Int32) { Value = soubor.RegisUzivatelId }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(soubor);
            }
        }

        public FileResult Download(int id)
        {
            string sql = "SELECT * FROM SOUBOR WHERE IDSOUBORU = :IdSouboru";
            var parameters = new OracleParameter[]
            {
                new OracleParameter("IdSouboru", OracleDbType.Int32) { Value = id }
            };

            var dataTable = _dbHelper.ExecuteQuery(sql, parameters);
            var soubor = _dbHelper.MapSoubor(dataTable)[0];

            return File(soubor.SouborContent, soubor.TypSouboru, soubor.Nazev);
        }
    }
}
