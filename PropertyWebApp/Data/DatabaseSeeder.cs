﻿using System;
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
                if (!context.IssueStatuses.Any())
                {
                    var statuses = new List<IssueStatus>
                    {
                        new IssueStatus { StatusName = "Rieši sa" },
                        new IssueStatus { StatusName = "Vyriešené" }
                    };

                    context.IssueStatuses.AddRange(statuses);
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
                if (!context.IssueImages.Any())
                {
                    var images = new List<IssueImage>
                    {
                        new IssueImage { ImagePath = "/images/heating_issue.jpg", IssueId = 1 },
                        new IssueImage { ImagePath = "/images/drain_issue.jpg", IssueId = 2 }
                    };

                    context.IssueImages.AddRange(images);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
