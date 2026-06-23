using GymSys.DAL.Data.DbContexts;
using GymSys.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymSys.DAL.Data.DataSeeding
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext, string seedFolderPath, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                if (!await dbContext.Plans.AnyAsync(ct))
                {
                    var plans = LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.json");
                    if (plans.Any())
                    {
                        dbContext.Plans.AddRange(plans);
                        logger.LogInformation($"Seeded {plans.Count} plans!");
                    }
                }

                if (dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync();
                else
                    logger.LogInformation("Plans already seeded!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gym data seeding failed!");
                throw;
            }
        }

        private static List<T> LoadDataFromJsonFile<T>(string folderPath, string fileName)
        {
            var filepath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filepath))
                throw new FileNotFoundException($"Seeding data file not found!: {filepath}");

            var data = File.ReadAllText(filepath);

            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}
