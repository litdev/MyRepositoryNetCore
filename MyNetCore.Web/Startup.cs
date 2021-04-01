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
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace MyNetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IServiceCollection _services;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
            //��ʼ������DI��Ҫ
            services.AddHttpContextAccessor();

            //���Swgger
            services.AddSwaggerServices();

            //���FreeSql
            services.AddFreeSqlServices();

            //Excel���뵼��
            //services.AddMagicodesIEServices();

            //��ӻ���
            services.AddMyCache();

            //ǿ����תhttps
            services.AddHttpsRedirectionServices();

            //����ע��Services�������ݿ�ʵ��ҵ��ע�����baseType�ǹ�������ҵ����(BaseServices<,>)
            services.BatchRegisterServices(new Assembly[] { Assembly.GetExecutingAssembly(), Assembly.Load($"{services.GetProjectMainName()}.Services") }, typeof(BaseServices<,>));

            //����ע��Services������ͨҵ��ע�����baseType�ǽӿ�����(IBatchDIServicesTag)
            services.BatchRegisterServices(new Assembly[] { Assembly.Load($"{services.GetProjectMainName()}.Services") }, typeof(IBatchDIServicesTag));

            //���Razor����htmlʱ���ı�ת��Unicode�������
            services.Configure<WebEncoderOptions>(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

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
                app.Map("/allservices", builder => builder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync($"<h1>���з���{_services.Count}��</h1><table><thead><tr><th>����</th><th>��������</th><th>ʵ��</th></tr></thead><tbody>");
                    foreach (var svc in _services)
                    {
                        await context.Response.WriteAsync("<tr>");
                        await context.Response.WriteAsync($"<td>{svc.ServiceType.FullName}</td>");
                        await context.Response.WriteAsync($"<td>{svc.Lifetime}</td>");
                        await context.Response.WriteAsync($"<td>{svc.ImplementationType?.FullName}</td>");
                        await context.Response.WriteAsync("</tr>");
                    }
                    await context.Response.WriteAsync("</tbody></table>");
                }));
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseWebResponseStatus();
                app.UseDefaultImage(defaultImagePath: Configuration.GetSection("DefaultImagePath").Value);
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
