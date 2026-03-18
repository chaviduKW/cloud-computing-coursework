using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SearchApi.Data
{
    // Used by EF Core tools (dotnet-ef migrations) at design time
    public class SearchDbContextFactory : IDesignTimeDbContextFactory<SearchDbContext>
    {
        public SearchDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SearchDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=SearchApiDb;Username=postgres;Password=12345");
            return new SearchDbContext(optionsBuilder.Options);
        }
    }
}
