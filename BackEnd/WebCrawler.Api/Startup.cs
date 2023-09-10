using Microsoft.EntityFrameworkCore;
using WebCrawler.BusinessLayer.Services;
using WebCrawler.DataAccessLayer.Context;
using WebsiteCrawler.Services;
using GraphQL.AspNet.Configuration;
using WebsiteCrawler.Infrastructure.Storage;
using WebsiteCrawler.Services.Storage;

namespace WebCrawler.Api;

public class Startup
{

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpClient();
        services.AddGraphQL();

        services.AddSingleton<DbContextOptionsBuilder>();

        services.AddDbContext<AppDbContext>(options =>
            //options.UseSqlServer("Server=sql_server2022;Database=SalesDb;User Id=SA;Password=A&VeryComplex123Password;MultipleActiveResultSets=true;TrustServerCertificate=True")
            options.UseSqlServer("Data Source=localhost;Initial Catalog=CrawlerDB;Integrated Security=True; TrustServerCertificate=true"),
            optionsLifetime: ServiceLifetime.Singleton
        );

        services.AddDbContextFactory<AppDbContext>(options => options.UseSqlServer("Data Source=localhost;Initial Catalog=CrawlerDB;Integrated Security=True; TrustServerCertificate=true"));

        services.AddHttpClient();
        services.AddScoped<RecordsService>();
        services.AddScoped<CrawlerService>();
        services.AddSingleton<ICrawlingNodeStorage, CrawlingNodeStorage>();

        services.AddCors(c =>
        {
            c.AddPolicy("AllowOrigin", options =>
            {
                options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICrawlingNodeStorage crawlingNodeStorage)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseGraphQL();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("AllowOrigin");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        Executor crawlerStartup = new Executor(crawlingNodeStorage);
        Task.Run(() => crawlerStartup.Run());
    }
}

public static class MigrationExtensions
{
    public static IWebHost MigrateDatabase<T>(this IWebHost webHost) where T : DbContext
    {
        using (var scope = webHost.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var db = services.GetRequiredService<T>();
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
        return webHost;
    }
}
