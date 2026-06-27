using GymSys.DAL.Data.DataSeeding;
using GymSys.DAL.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GymSys.PL
{
    public static class ProgramExtension
    {
        public static async Task DatabaseMigrationAndSeedingAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()} pending migrations.");
                await dbContext.Database.MigrateAsync();
            }

            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);
        }
    }
}
