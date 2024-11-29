using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PropertyWebApp.Data;
using PropertyWebApp.Models;

namespace PropertyWebApp
{
    public static class DatabaseSeeder
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Skontroluje, či už sú dáta v IssueStatus
                if (!context.IssueStatus.Any())
                {
                    var statuses = new List<IssueStatus>
                    {
                        new IssueStatus { StatusName = "Rieši sa", Color = "#E34848"},
                        new IssueStatus { StatusName = "Vyriešené", Color = "#3FBD5A"}
                    };

                    context.IssueStatus.AddRange(statuses);
                    await context.SaveChangesAsync();
                }

                // Skontroluje, či už sú dáta v Issues
                if (!context.Issues.Any())
                {
                    var issues = new List<Issue>
                    {
                        new Issue
                        {
                            Title = "Pokazené kúrenie",
                            Description = "Kúrenie prestalo fungovať v obývačke.",
                            ReportDate = DateTime.Now.AddDays(-7),
                            RepairCost = 120.50m,
                            StatusId = 1, // Rieši sa
                            RentalId = 1 // Musí byť v DB
                        },
                        new Issue
                        {
                            Title = "Netesniaci odtok",
                            Description = "Voda presakuje cez sprchový odtok.",
                            ReportDate = DateTime.Now.AddDays(-15),
                            SolvedDate = DateTime.Now.AddDays(-5),
                            RepairCost = 45.00m,
                            StatusId = 2, // Vyriešené
                            RentalId = 2 // Musí byť v DB
                        }
                    };

                    context.Issues.AddRange(issues);
                    await context.SaveChangesAsync();
                }

                // Skontroluje, či už sú dáta v IssueImages
                if (!context.IssueImage.Any())
                {
                    var images = new List<IssueImage>
                    {
                        new IssueImage { ImagePath = "/img/heatingIssue.jpg", IssueId = 1 },
                        new IssueImage { ImagePath = "/img/drainIssue.jpeg", IssueId = 2 }
                    };

                    context.IssueImage.AddRange(images);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
