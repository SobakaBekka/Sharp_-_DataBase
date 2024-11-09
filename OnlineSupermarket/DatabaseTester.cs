using System;
using Oracle.ManagedDataAccess.Client;

namespace OnlineSupermarket
{
    public class DatabaseTester
    {
        public static void TestConnection()
        {
            string connectionString = "User Id=st69704;Password=abcde;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=BDAS)))";

            using (var connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Підключення успішне!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Помилка підключення: " + ex.Message);
                }
            }
        }
    }
}
