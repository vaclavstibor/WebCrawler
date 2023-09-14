using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WebCrawler.BusinessLayer.Services;
using WebCrawler.DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore.SqlServer;
using WebsiteCrawler.Services;
using GraphQL.AspNet.Configuration;

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

        //Uncomment this for local connection

        //services.AddDbContext<AppDbContext>(options =>
        //    options.UseSqlServer("Data Source=localhost;Initial Catalog=CrawlerDB;Integrated Security=True; TrustServerCertificate=true")
        //); 

        //Uncomment this for docker connection
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer("Server=sql_server2022;Database=SalesDb;User Id=SA;Password=A&VeryComplex123Password;MultipleActiveResultSets=true;TrustServerCertificate=True")
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
    }
}

public static class StartupExtensions
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

    public static IWebHost RunCrawler(this IWebHost webHost)
    {
        Executor crawlerStartup = new Executor();
        Task.Run(() => crawlerStartup.Run());

        return webHost;
    }
}
