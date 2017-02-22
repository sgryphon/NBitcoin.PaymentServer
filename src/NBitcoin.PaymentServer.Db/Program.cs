using DbUp;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Db
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var applicationVersion = PlatformServices.Default.Application.ApplicationVersion;
            var machineName = Environment.MachineName;
            var assembly = typeof(Program).GetTypeInfo().Assembly;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            Console.WriteLine("NBitcoin.PaymentServer.Db, v{0}, running on {1}", applicationVersion, machineName);
            Console.WriteLine("v{0} [v{1}]", fileVersionInfo.ProductVersion, assembly.GetName().Version);

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
