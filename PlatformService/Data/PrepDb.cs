using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data
{
    public class PrepDb
    {
        public static  void PrepPopulation(IApplicationBuilder app,bool isProd)
        {
            using (var serviceScope=app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(),isProd);
            }
        }

        private static void SeedData(AppDbContext context,bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attemting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"--> Could not run migrations...:{ex.Message}");
                }
                
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("--->Seeding Data<---");
                context.Platforms.AddRange(
                    new Models.Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Models.Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Models.Platform { Name = "Kubernates", Publisher = "Cloud Native Computing Foundation", Cost = "Free" });
                context.SaveChanges();

            }
            else
            {
                Console.WriteLine("Db already has data");
            }
        }
    }
}
