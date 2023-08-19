using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebCrawler.BusinessLayer.Services;
using WebCrawler.DataAccessLayer.Context;

namespace WebCrawler.Api
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpClient();
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Data Source=localhost;Initial Catalog=CrawlerDB;Integrated Security=True; TrustServerCertificate=true")
            );
            
            services.AddHttpClient();
            services.AddScoped<RecordsService>();
            services.AddScoped<CrawlerService>();

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options =>
                {
                    options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowOrigin");

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
