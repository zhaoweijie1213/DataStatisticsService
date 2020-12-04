using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStatistics.Service.Quartz;
using DataStatistics.Service.Services;
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
            
            var host=CreateHostBuilder(args).Build();
            var service = host.Services.GetService(typeof(ILoadDataService)) as ILoadDataService;
             Task.Run(()=> {
                host.Run();
            });
            Console.WriteLine("========初始化数据========");
            Console.WriteLine("========输入help查看命令========");
            string key = "";
            var run = true;
            while (run)
            {
                key = Console.ReadLine();
                switch (key)
                {
                    case "help":
                        Console.WriteLine("========初始化数据输入load========");
                        continue;
                    case "load":
                        //初始化数据
                        using (service)
                        {
                            service.LoadVersion();
                            service.LoadThirtyUserAction();
                        }
                        continue;
                    case "exit":
                        run = false;
                        break;
                    default:
                        break;
                }
            }
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
