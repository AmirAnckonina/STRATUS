using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Writers;
using MongoDB.Driver;
using StratusApp.Data;
using StratusApp.Models.MongoDB;
using StratusApp.Services;
using StratusApp.Services.MongoDBServices;
using System.Text.Json.Serialization;

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
            services.Configure<MyDatabaseSettings>(ConfigRoot.GetSection(nameof(MyDatabaseSettings)));

            services.AddSingleton<MyDatabaseSettings>(sp =>
              sp.GetRequiredService<IOptions<MyDatabaseSettings>>().Value);
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                var enumConverter = new JsonStringEnumConverter();
                options.JsonSerializerOptions.Converters.Add(enumConverter);
            });


            /*services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(ConfigRoot.GetConnectionString("DefaultConnection")));*/

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
            services.AddSingleton<MongoDBService>();
            services.AddSingleton<IStratusService, StratusService>();
            services.AddSingleton<AlertsService>();
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            //app.UseCors("AllowAnyOrigin");
            app.UseAuthorization();
            app.UseEndpoints(endPoints => endPoints.MapControllers());
            //app.Run();
        }
    }
}
