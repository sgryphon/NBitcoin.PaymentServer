using DbUp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XyzDonateDb
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var connectionString =
                args.FirstOrDefault()
                ?? "Data Source=(local);Initial Catalog=NBitcoinPayments;Integrated Security=True;MultipleActiveResultSets=True";

            //TestSql(connectionString);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(typeof(Program).GetTypeInfo().Assembly)
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
#if DEBUG
            Console.ReadLine();
#endif
            return 0;
        }

        private static void TestSql(string connectionString)
        {
            Console.WriteLine($"Connection: {connectionString}");

            var connection = new SqlConnection(connectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = "select count(*) from sys.tables;";

            var result = command.ExecuteScalar();

            Console.WriteLine($"Result: {result}");
        }
    }
}
