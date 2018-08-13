using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MyCI.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            RunMvc.EnsureDirectories();
            var configuration = RunMvc.MakeConfigurationByAppSettingsJson();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                //.Enrich.FromLogContext()
                //.WriteTo.Console()
                .CreateLogger();


            Log.Information("Getting the engines running...");
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    /*
                     * To permit upload of BIG file you have to edit also the web.config:<requestLimits maxAllowedContentLength="1073741824" />
                     * tnx to: https://stackoverflow.com/questions/38698350/increase-upload-file-size-in-asp-net-core
                     *
                     */

                    options.Limits.MaxRequestBodySize = null;
                
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseConfiguration(configuration) // Initialize configuration
                .UseStartup<Startup>()
                .UseSerilog(Log.Logger, true); // Serilog integration (set also ILogFactory)
            //Do NOT Close the Log otherwise no logs will works..
            //finally
            //{
            //  //Log.CloseAndFlush();
            //}
        }
    }
}