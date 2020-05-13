using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using XueLeMeBackend.Data;
using XueLeMeBackend.Services;

namespace XueLeMeBackend
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
            services.AddSingleton<DbInitializer>();
            services.AddDbContext<XueLeMeContext>(
                o => o.UseMySql(
                    Configuration.GetConnectionString("MySQLConnectString"),
                    mysqlopt => mysqlopt.ServerVersion(new Version(8, 0, 3), ServerType.MySql)
                ),
                ServiceLifetime.Scoped);

            services.AddSingleton<IMailService, QQMailService>();
            services.AddSingleton<MD5Service>();
            services.AddScoped<IFileService, DbFileService>();

            services.AddSingleton<ISecurityService, MD5SecurityService>();
            services.AddScoped<IMailAccountService, MailAccountService>();

            services.AddControllers();
            services.AddSwaggerDocument();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
