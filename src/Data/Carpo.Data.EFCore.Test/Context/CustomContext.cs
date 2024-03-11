using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Carpo.Data.EFCore.Test.Context
{
    public class CustomContext : CarpoContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            optionsBuilder
                .UseSqlServer("Server=ROM;Database=CarpoInterpres;Persist Security Info=True;User ID=carpobase;Password=C@rpo;MultipleActiveResultSets=True;TrustServerCertificate=True;", options =>
                {
                    options.EnableRetryOnFailure();
                })
                  .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}
