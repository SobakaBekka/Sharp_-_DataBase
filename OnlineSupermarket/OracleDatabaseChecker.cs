using System;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class OracleDatabaseChecker
{
    private readonly string _connectionString;
    private readonly ILogger<OracleDatabaseChecker> _logger;

    public OracleDatabaseChecker(IConfiguration configuration, ILogger<OracleDatabaseChecker> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    public bool TestConnection()
    {
        try
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                _logger.LogInformation("Подключение к Oracle базе данных успешно.");
                return true;
            }
        }
        catch (OracleException ex)
        {
            _logger.LogError($"Ошибка подключения к Oracle базе данных: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Произошла ошибка: {ex.Message}");
            return false;
        }
    }
}
