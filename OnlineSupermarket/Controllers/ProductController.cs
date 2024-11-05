using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSupermarket.Data;
using Oracle.ManagedDataAccess.Client;

public class ProductController : Controller
{
    private readonly OnlineSupermarketContext db;

    public ProductController(OnlineSupermarketContext context)
    {
        db = context;
    }

    public IActionResult Index()
    {
        var products = db.Zbozi.Include(z => z.Soubor).ToList();
        return View(products);
    }

    public ActionResult UpdateProduct(int id, decimal newPrice, int newStockQty)
    {
        db.Database.ExecuteSqlRaw("BEGIN UpdateProduct(:id, :newPrice, :newStockQty); END;",
            new OracleParameter("id", id),
            new OracleParameter("newPrice", newPrice),
            new OracleParameter("newStockQty", newStockQty));
        return RedirectToAction("Index");
    }
}