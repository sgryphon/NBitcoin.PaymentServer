using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NBitcoin.PaymentServer.Services;
using NBitcoin.PaymentServer.Contracts;
using Microsoft.EntityFrameworkCore;

namespace NBitcoin.PaymentServer.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                // Will override with Azure environment variables, e.g. "ConnectionStrings:DefaultConnection"
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // Add framework services.
            var mvcBuilder = services.AddMvc(options => {
                options.Filters.Add(typeof(GlobalExceptionFilter));
            });

            // Add custom services
            services.AddDbContext<PaymentsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.Configure<BitcoinRpcOptions>(Configuration.GetSection("BitcoinRpc"));
            services.AddSingleton<ICurrencyConversionService, DummyCurrencyConversionService>();
            services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
            services.AddScoped<IVerificationService, BitcoinRpcVerificationService>();
            services.AddSingleton<PaymentProcessor>();

            // services.AddSingleton(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //var azureDiagnosticSettings = new AzureAppServicesDiagnosticsSettings { };
            //loggerFactory.AddAzureWebAppDiagnostics();
            loggerFactory.AddDebug(LogLevel.Trace);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            //app.Map("/api", api =>
            //{
            //    var config = new HttpConfiguration();
            //    config.MapHttpAttributeRoutes();
            //    api.UseWebApi(config);
            //});

            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    if (context.Response.StatusCode == 404
            //        && !Path.HasExtension(context.Request.Path.Value)
            //        && !context.Request.Path.ToString().StartsWith("/api", StringComparison.OrdinalIgnoreCase))
            //    {
            //        context.Request.Path = "/index.html";
            //        await next();
            //    }
            //});

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
