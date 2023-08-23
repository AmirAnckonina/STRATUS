using CloudApiClient.AwsServices.AwsUtils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Writers;
using MongoDB.Driver;
using StratusApp.Data;
using StratusApp.Services;
using StratusApp.Services.Collector;
using StratusApp.Services.MongoDBServices;
using StratusApp.Services.Recommendations;
using System.Text.Json.Serialization;
using StratusApp.Settings;

namespace StratusApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration) 
        {
            ConfigRoot = configuration;
        }

        public IConfiguration ConfigRoot { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(3); // Set the session timeout as desired
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
                //options.Cookie.Name = "userDBEmail";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });
            

            services.Configure<AppSettings>(ConfigRoot.GetSection(nameof(AppSettings)));

            services.AddSingleton<AppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                var enumConverter = new JsonStringEnumConverter();
                options.JsonSerializerOptions.Converters.Add(enumConverter);
            });


            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", builder =>
                builder.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
                //.AllowAnyOrigin());
            });

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add MongoDB connection
            services.AddSingleton<EC2ClientFactory>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<MongoDBService>();
            services.AddSingleton<AlertsService>();
            services.AddSingleton<AwsService>();
            services.AddSingleton<IStratusService, StratusService>();
            services.AddSingleton<CollectorService>();
            services.AddSingleton<RecommendationsService>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<AuthService>();

            //services.AddRazorPages();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                await next();
            });

            app.UseSession();
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();
            //app.UseCors("AllowAnyOrigin");
            app.UseEndpoints(endPoints => endPoints.MapControllers());
            //app.Run();
        }
    }
}
