using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataStatistics.Api.Controllers;
using DataStatistics.Service.Quartz;
using DataStatistics.Service.Quartz.Impl;
using DataStatistics.Service.Repositorys;
using DataStatistics.Service.Repositorys.Impl;
using DataStatistics.Service.Services;
using DataStatistics.Service.Services.Impl;
using FluentData.Extensions;
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
            services.AddFluentData();
            //redis���ݿ�
            services.AddEasyCaching(option =>
            {
                option.UseRedis(Configuration, "userAction", "easycaching:redis").WithJson();
            });
            services.AddTransient<Quartz.IJob, DbStatisticsJob>();
            services.AddScoped<ICacheManage, CacheManage>();
            services.AddScoped<IDataService, DataService>();
            //������

            services.AddSingleton<IQuartzManager, QuartzManager>();
            services.AddSingleton<IMJLogOtherRepository>(option =>
            {
                var log = option.GetServices<ILogger<MJLogOtherRepository>>();
                return new MJLogOtherRepository(log.FirstOrDefault(), Configuration.GetConnectionString("mj_log_other_mysql"));
            });
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DataStatisticsService",
                    Version = "v1",
                    Description = "����ͳ�Ʒ���",
                    Contact = new OpenApiContact() { Email = "dyb628@queyouquan.net", Name = "����ͷ" }
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
            var quartz = app.ApplicationServices.GetService<IQuartzManager>();

            #region ��������������
            applicationLifetime.ApplicationStarted.Register(() =>
            {
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


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
