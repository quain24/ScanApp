using FluentValidation;
using Fluxor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Extensions;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Installers;
using ScanApp.Common.Installers;
using ScanApp.Data;
using ScanApp.Infrastructure.Common.Installers;
using Serilog;

namespace ScanApp
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
            ValidatorOptions.Global.LanguageManager.Enabled = false;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Try to create installers to keep this method as clean as possible.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = _env.IsDevelopment());

            services.AddSecurityConfiguration();
            services.AddMediatR();
            services.AddInfrastructureServices(Configuration);
            services.AddGuiServices();
            services.AddDatabases(Configuration, _env.IsDevelopment());
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddMudBlazor();
            services.AddFluxorStateManagement();
            services.AddHttpContextAccessor();
            services.AddValidatorsFromAssemblies(new[] { typeof(ApplicationUser).Assembly, typeof(DateTimeExtensions).Assembly });
            services.AddCommonFluentValidationPropertyValidators();
            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseBrowserLink();
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}