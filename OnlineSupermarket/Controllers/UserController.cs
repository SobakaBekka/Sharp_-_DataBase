using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSupermarket.Data;
using Oracle.ManagedDataAccess.Client;

public class UserController : Controller
{
    private readonly OnlineSupermarketContext db;

    public UserController(OnlineSupermarketContext context)
    {
        db = context;
    }

    public IActionResult Index()
    {
        var users = db.RegisUzivatele.Include(u => u.Role).ToList();
        return View(users);
    }

    public ActionResult ChangeUserRole(int userId, int newRoleId)
    {
        db.Database.ExecuteSqlRaw("UPDATE Users SET RoleID = :newRoleId WHERE UserID = :userId",
            new OracleParameter("userId", userId),
            new OracleParameter("newRoleId", newRoleId));
        return RedirectToAction("Index");
    }

    public ActionResult ChangePassword(int userId, string newPasswordHash)
    {
        db.Database.ExecuteSqlRaw("UPDATE Users SET PasswordHash = :newPasswordHash WHERE UserID = :userId",
            new OracleParameter("userId", userId),
            new OracleParameter("newPasswordHash", newPasswordHash));
        return RedirectToAction("Profile");
    }
}