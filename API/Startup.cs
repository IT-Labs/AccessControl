using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SoyalControllers;
using SoyalControllers.ControllerIdentification;

using Communication.Sockets;

namespace API
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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddScoped<IControllerAddress, ControllerAddress>();
            services.AddScoped<IControllerIdentificationService, ControllerIdentificationService>();

            services.AddScoped<IAsyncClient, AsyncClient>();

            services.AddScoped<IGetRealTimeClock, GetRealTimeClock>();
            services.AddScoped<IGetDeviceStatus, GetDeviceStatus>();
            services.AddScoped<IGetUserData, GetUserData>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IControllerAddress controllerAddress)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
