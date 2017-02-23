using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ExampleMerchant.TanstaaflCafe
{
    public class Program
    {
        static string _productVersion;

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }

        public static string ProductVersion
        {
            get
            {
                if (_productVersion == null)
                {
                    var assembly = typeof(Program).GetTypeInfo().Assembly;
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    _productVersion = fileVersionInfo.ProductVersion;
                }
                return _productVersion;
            }
        }
    }
}
