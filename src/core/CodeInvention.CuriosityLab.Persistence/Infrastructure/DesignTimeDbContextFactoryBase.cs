using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CodeInvention.CuriosityLab.Persistence.Infrastructure
{
    public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
       where TContext : DbContext
    {
        private const string ConnectionStringName = "Default";
        private const string ASPNETCORE_ENVIRONMENT = "Development";

        public TContext CreateDbContext(string[] args)
        {
            var basePath = System.IO.Directory.GetCurrentDirectory() + string.Format("{0}..{0}CodeInvention.CuriosityLab.Web.API", Path.DirectorySeparatorChar);
            return Create(basePath, "Development");
        }

        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

        private TContext Create(string basePath, string environmentName)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{ASPNETCORE_ENVIRONMENT}.json")
                .Build();

            var connectionString = configuration.GetConnectionString(ConnectionStringName);
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException($"Connectionstring '{ConnectionStringName}' is null or empty", nameof(connectionString));

            return Create(connectionString);

        }

        private TContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException($"Connectionstring '{ConnectionStringName}' is null or empty", nameof(connectionString));

            var optionBuilder = new DbContextOptionsBuilder<TContext>();
            optionBuilder.UseNpgsql(connectionString);

            return CreateNewInstance(optionBuilder.Options);
        }
    }
}
