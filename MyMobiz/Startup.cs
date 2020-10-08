using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyMobiz.Models;
using MyMobiz.BackgroundServices;

namespace MyMobiz
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
            
            services.AddControllers();  
            services.AddControllers().AddNewtonsoftJson();
            services.AddTransient<DbContext, mymobiztestContext>();
            //added dbContex and connection string
            // checking if app is on debug mode

            services.AddDbContext<mymobiztestContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DbTestConnection")));
            /*if (Debugger.IsAttached)
            {
                services.AddDbContext<mymobiztestContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DbTestConnection")));
            }*/
            // Adding Background Services
            services.AddHostedService<QueueService>();
            services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {   
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
