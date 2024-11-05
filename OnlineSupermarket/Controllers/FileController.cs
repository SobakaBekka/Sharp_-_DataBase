using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSupermarket.Data;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class FileController : Controller
{
    private readonly OnlineSupermarketContext db;

    public FileController(OnlineSupermarketContext context)
    {
        db = context;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    public IActionResult Index()
    {
        var files = db.Soubory.ToList();
        return View(files);
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileData = memoryStream.ToArray();

                db.Database.ExecuteSqlRaw("INSERT INTO SOUBOR (IDSOUBOR, NAZEV, SOUBOR, PRIPONA, UPLOADDATE, TYPSOUBORU) " +
                                          "VALUES (SOUBOR_SEQ.NEXTVAL, :fileName, :fileData, :fileExtension, SYSDATE, :fileType)",
                    new OracleParameter("fileName", file.FileName),
                    new OracleParameter("fileData", fileData),
                    new OracleParameter("fileExtension", Path.GetExtension(file.FileName)),
                    new OracleParameter("fileType", file.ContentType));
            }
        }
        return RedirectToAction("Index");
    }

    public IActionResult DisplayImage(int fileId)
    {
        var fileData = db.Soubory
                         .Where(f => f.IdSoubor == fileId)
                         .Select(f => f.FileData)
                         .FirstOrDefault();

        if (fileData != null)
        {
            return File(fileData, "image/jpeg");
        }
        return NotFound();
    }

    public IActionResult DeleteFile(int fileId)
    {
        db.Database.ExecuteSqlRaw("DELETE FROM SOUBOR WHERE IDSOUBOR = :fileId",
                                  new OracleParameter("fileId", fileId));
        return RedirectToAction("Index");
    }
}