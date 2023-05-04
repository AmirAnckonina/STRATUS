using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using StratusApp.Data;
using StratusApp.Services;
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
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            // Add services to the container.
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(ConfigRoot.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddTransient<IStratusService, StratusService>();

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
            app.UseAuthorization();
            app.UseEndpoints(endPoints => endPoints.MapControllers());
            //app.Run();

        }
    }
}
