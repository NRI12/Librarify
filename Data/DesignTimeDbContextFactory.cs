// Data/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace libraryproject.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<QLTVContext>
    {
        public QLTVContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<QLTVContext>();
            var connectionString = configuration.GetConnectionString("QLTVConnection");
            builder.UseSqlServer(connectionString);

            return new QLTVContext(builder.Options);
        }
    }
}