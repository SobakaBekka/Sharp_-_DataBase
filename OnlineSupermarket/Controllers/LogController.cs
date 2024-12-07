using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OnlineSupermarket.Controllers
{
    public class LogController : Controller
    {
        private readonly string _connectionString;

        public LogController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
        }

        public IActionResult Index()
        {
            List<LogEntry> logList = new List<LogEntry>();

            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("ZOBRAZ_LOG", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            logList.Add(new LogEntry
                            {
                                IdLogDatabaz = reader.GetInt32(0),
                                Tabulka = reader.GetString(1),
                                Operace = reader.GetString(2),
                                Datum = reader.GetDateTime(3),
                                Uzivatel = reader.GetString(4),
                                Zmeny = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return View(logList);
        }
    }
}