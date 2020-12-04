using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataStatistics.Api.Controllers;
using DataStatistics.Service.Quartz;
using DataStatistics.Service.Quartz.Impl;
using DataStatistics.Service.Quartz.Jobs;
using DataStatistics.Service.Quartz.Jobs.Interface;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Repositorys.Impl;
using DataStatistics.Service.Services;
using DataStatistics.Service.Services.DataProcessing;
using DataStatistics.Service.Services.DataProcessingl.Impl;
using DataStatistics.Service.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace DataStatistics.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            //redis���ݿ�
            services.AddEasyCaching(option =>
            {
                option.UseRedis(Configuration, "userAction", "easycaching:redis").WithJson();
            });
            //services.AddTransient<Quartz.IJob, DbStatisticsJob>();       
            //���jobs
            services.AddTransient<IDbStatisticsJob, DbStatisticsJob>();
            services.AddTransient<IDataGroupBy1MinJob, DataGroupBy1MinJob>();
            services.AddTransient<IDataGroupBy5MinJob, DataGroupBy5MinJob>();
            services.AddTransient<IDataGroupBy10MinJob, DataGroupBy10MinJob>();
            services.AddTransient<IDataGroupBy1HourJob, DataGroupBy1HourJob>();
            services.AddTransient<IRidesDataJob, RidesDataJob>();
            //���ݼ��ط���
            services.AddTransient<ILoadDataService, LoadDataService>();
            //
            services.AddSingleton<ICacheManage, CacheManage>();
            services.AddScoped<IDataService, DataService>();
            //������
            services.AddSingleton<IQuartzManager, QuartzManager>();
            services.AddSingleton<IMJLogOtherRepository>(option =>
            {
                var log = option.GetServices<ILogger<MJLogOtherRepository>>();
                return new MJLogOtherRepository(log.FirstOrDefault(), Configuration.GetConnectionString("mj_log_other_mysql"));
            });       
            services.AddSingleton<IMJLog3Repository>(option =>
            {
                var log = option.GetServices<ILogger<MJLog3Repository>>();
                return new MJLog3Repository(log.FirstOrDefault(), Configuration.GetConnectionString("mj_log3_mysql"));
            });
            services.AddScoped<IDataProcessing, DataProcessing>();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DataStatisticsService",
                    Version = "v1",
                    Description = "����ͳ�Ʒ���",
                    Contact = new OpenApiContact() { Email = "949210784@QQ.COM", Name = "zhaoweijie" }
                });
                //���header��֤��Ϣ
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) �����ṹ: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,// "header",//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                //�����֤    
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                // ���س��򼯵�xml�����ĵ�
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var xmlFile = AppDomain.CurrentDomain.FriendlyName + ".xml";
                //model��xml�ĵ� 
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
                c.IncludeXmlComments(Path.Combine(baseDirectory, "DataStatistics.Model.xml"));
            });
            //cap
            services.AddCap(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("dotnet_cap_mysql"));
                options.UseRabbitMQ(config =>
                {
                    config.HostName = Configuration["CAP:CAP_MQ:HostName"];
                    config.UserName = Configuration["CAP:CAP_MQ:UserName"];
                    config.Port = Convert.ToInt32(Configuration["CAP:CAP_MQ:Port"]);
                    config.Password = Configuration["CAP:CAP_MQ:Password"];
                    config.ExchangeName = Configuration["CAP:CAP_MQ:ExchangeName"];
                });


                options.DefaultGroup = Configuration["CAP:DefaultGroup"];


                options.UseDashboard();

                //ʧ�ܺ�����Դ�����Ĭ��50�Σ���FailedRetryIntervalĬ��60�������£���Ĭ������50*60��(50����)֮�����ʧ������
                options.FailedRetryCount = 10;

                //ʧ�ܺ����ʰ�����Ĭ��60��
                options.FailedRetryInterval = 30;

                //���óɹ���Ϣ��ɾ��ʱ��Ĭ��24*3600��
                options.SucceedMessageExpiredAfter = 60 * 60;

            });
            services.AddMvc(options=> {
                options.Filters.Add<ApiResultFilter>();
            });
            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var quartz = app.ApplicationServices.GetRequiredService<IQuartzManager>();

            #region ��������������
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                //quartz.LoadVersion();
                //quartz.LoadThirtyUserAction();
                quartz.LoadScheduleJob(app.ApplicationServices);
            });
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                quartz.EndScheduler();
            });
            #endregion
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DataStatisticsService V1");
            });

            //ǿ��ִ��https
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
