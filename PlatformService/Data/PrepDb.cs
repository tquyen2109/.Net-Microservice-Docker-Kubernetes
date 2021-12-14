using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }
        
        private static void SeedData(AppDbContext context, bool isProduction) 
        {
            if (isProduction)
            {
                Console.WriteLine("Applying migration...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            if (!context.Platfomrs.Any())
            {
                Console.WriteLine("Seeding data...");
                context.Platfomrs.AddRange(
                    new Platform()
                    {
                        Name = "Dot net",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    },
                    new Platform()
                    {
                        Name = "SQL Server",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    },
                    new Platform()
                    {
                        Name = "Docker",
                        Publisher = "Cloud",
                        Cost = "Free"
                    }
                    );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("We already have data");
            }
        }
    }
   
}