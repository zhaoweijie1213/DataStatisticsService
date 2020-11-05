using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataStatistics.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(options =>
            {
                options.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                options.AddJsonFile("config/appsettings.json", false, true);
            }).ConfigureLogging(builder =>
            {
                builder.AddLog4Net(AppDomain.CurrentDomain.BaseDirectory + "config/log4net.config");
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
