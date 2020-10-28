using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyMobiz.Models;
using MyMobiz.BackgroundServices;
using MyMobiz.iHostedService;
using MyMobiz.Extensions;
using LoggerService;
using NLog;
using NLog.Extensions.Logging;

namespace MyMobiz
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddControllers();  
            services.AddControllers().AddNewtonsoftJson();
            services.AddTransient<DbContext, mymobiztestContext>();
            //added dbContex and connection string
            services.AddDbContext<mymobiztestContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DbTestConnection")));
            // Adding Background Services
            services.AddHostedService<QueueService>();
            services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
            services.AddMemoryCache();
            services.AddHostedService<TimedHostedService>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }            
            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}