using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSupermarket.Data;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

public class EmployeeController : Controller
{
    private readonly OnlineSupermarketContext db;

    public EmployeeController(OnlineSupermarketContext context)
    {
        db = context;
    }

    public ActionResult CalculateSalary(int employeeId, int hoursWorked)
    {
        var salaryResult = db.SalaryResults.FromSqlRaw("SELECT Vypocet_Mzdy(:employeeId, :hoursWorked) AS Mzda FROM dual",
            new OracleParameter("employeeId", employeeId),
            new OracleParameter("hoursWorked", hoursWorked)).First();
        return Content($"Calculated Salary: {salaryResult.Mzda}");
    }

    public ActionResult AddEmployee(string firstName, string lastName, DateTime birthNumber, int phone, int positionId, int storeId)
    {
        db.Database.ExecuteSqlRaw("BEGIN PRIDAT_NOVY_ZAMESTNANEC(:firstName, :lastName, :birthNumber, :phone, :positionId, :storeId); END;",
            new OracleParameter("firstName", firstName),
            new OracleParameter("lastName", lastName),
            new OracleParameter("birthNumber", birthNumber),
            new OracleParameter("phone", phone),
            new OracleParameter("positionId", positionId),
            new OracleParameter("storeId", storeId));
        return RedirectToAction("Index");
    }

    public ActionResult UpdateEmployee(int employeeId, string firstName, string lastName, int phone, int positionId)
    {
        db.Database.ExecuteSqlRaw("BEGIN Aktualizovat_Zamestnanec(:employeeId, :firstName, :lastName, :phone, :positionId); END;",
            new OracleParameter("employeeId", employeeId),
            new OracleParameter("firstName", firstName),
            new OracleParameter("lastName", lastName),
            new OracleParameter("phone", phone),
            new OracleParameter("positionId", positionId));
        return RedirectToAction("Index");
    }

    public ActionResult UpdateEmployeePosition(int employeeId, int positionId)
    {
        db.Database.ExecuteSqlRaw("BEGIN AKTUALIZACE_POZICE(:employeeId, :positionId); END;",
            new OracleParameter("employeeId", employeeId),
            new OracleParameter("positionId", positionId));
        return RedirectToAction("Index");
    }
}