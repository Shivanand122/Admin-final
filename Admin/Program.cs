using AdminDAL;
using AdminDAL.Entities2;
using AdminDAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using NuGet.Configuration;
using Microsoft.Extensions.Configuration;



namespace Admin
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var azureAppConfigConnectionString = builder.Configuration.GetConnectionString("AzureAppConfig");

            ConfigureServices(builder.Services, builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddRazorPages();

            builder.Services.AddAzureAppConfiguration();

            builder.Services.AddFeatureManagement();

            builder.Services.Configure<Settings>(builder.Configuration.GetSection("TestApp:Settings"));


            var app = builder.Build();

           builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(azureAppConfigConnectionString)
                       .Select("TestApp:*", LabelFilter.Null)
                       .ConfigureRefresh(refreshOptions =>
                            refreshOptions.Register("TestApp:Settings:Sentinel", refreshAll: true));


                options.UseFeatureFlags(featureFlagOptions =>
                {
                    featureFlagOptions.Select("AdminSide");
                    featureFlagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(1);

                });



            });

            

            Configure(app, app.Environment);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, string constring = "")
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddFeatureManagement();

            services.AddControllersWithViews();

            services.AddScoped<IFeatureRepository, FeatureRepository>();

            services.AddDbContext<AdminCont>(options => options.UseSqlServer(constring));
        }

        private static void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
           app.UseAzureAppConfiguration();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=AdminM}/{action=Log}/{id?}");
        }




    }
}
