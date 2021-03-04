using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyNetCore.IServices;
using MyNetCore.Services;
using MyNetCore.IRepository;
using MyNetCore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNetCore.Web.SetUp;

namespace MyNetCore.Web
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
            //��ʼ������DI��Ҫ
            services.AddHttpContextAccessor();

            //���Swgger
            services.AddSwaggerServices();

            //���FreeSql
            services.AddFreeSqlServices();

            //ǿ����תhttps
            services.AddHttpsRedirectionServices();

            //ע��Services���е�ҵ��
            services.AddMyCustomServices();

            //���MVC���
            services.AddWebMVCServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //��ȡ����ע��ķ���
            ServiceLocator.Instance = app.ApplicationServices;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //��̬�ļ�
            app.UseMyStaticFiles();

            //��������/ֹͣ���еĲ���
            app.UseMyAppStartup();

            //Swagger
            app.UseMySwagger();

            //MVC
            app.UseMyWebMVC();
        }
    }
}
