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
                // IssueStatus
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (!context.IssueStatus.Any())
                {
                    var statuses = new List<IssueStatus>
                    {
                        new IssueStatus { StatusName = "Rieši sa", Color = "#E34848" },
                        new IssueStatus { StatusName = "Vyriešené", Color = "#3FBD5A" }
                    };

                    context.IssueStatus.AddRange(statuses);
                    await context.SaveChangesAsync();
                }

                // PropertyTypes
                if (!context.PropertyTypes.Any())
                {
                    var propertyTypes = new List<PropertyType>
                    {
                        new PropertyType { TypeName = "Apartment" },
                        new PropertyType { TypeName = "House" }
                    };

                    context.PropertyTypes.AddRange(propertyTypes);
                    await context.SaveChangesAsync();
                }

                //  TypeId pre PropertyTypes
                var apartmentTypeId = context.PropertyTypes.FirstOrDefault(pt => pt.TypeName == "Apartment")?.TypeId;
                var houseTypeId = context.PropertyTypes.FirstOrDefault(pt => pt.TypeName == "House")?.TypeId;
                
                //  Properties
                if (!context.Properties.Any())
                {
                    var properties = new List<Property>
                    {
                        new Property
                        {
                            PropertyName = "Moderný byt v centre",
                            IsAvailable = true,
                            TypeId = apartmentTypeId.Value, // Dynamicky získané TypeId
                            ListingDate = DateTime.Now.AddDays(-30),
                            Description = "Priestranný 2-izbový byt s balkónom v centre mesta.",
                            StreetName = "Hlavná 1",
                            City = "Bratislava",
                            PostalCode = "81101",
                            Country = "Slovensko",
                            State = "Bratislava",
                            Price = 850.00m,
                            Area = 65,
                            NumberOfBedrooms = 2,
                            NumberOfBathrooms = 1,
                            IsFurnished = true,
                            ParkingAvailable = false
                        },
                        new Property
                        {
                            PropertyName = "Rodinný dom v tichom prostredí",
                            IsAvailable = false,
                            TypeId = houseTypeId.Value, // Dynamicky získané TypeId
                            ListingDate = DateTime.Now.AddDays(-60),
                            Description = "Štýlový rodinný dom s veľkou záhradou.",
                            StreetName = "Záhradná 12",
                            City = "Žilina",
                            PostalCode = "01001",
                            Country = "Slovensko",
                            State = "Žilinský kraj",
                            Price = 1200.00m,
                            Area = 120,
                            NumberOfBedrooms = 4,
                            NumberOfBathrooms = 2,
                            IsFurnished = false,
                            ParkingAvailable = true
                        }
                    };

                    context.Properties.AddRange(properties);
                    await context.SaveChangesAsync();
                }

                // PropertyImages
                if (!context.PropertyImages.Any())
                {
                    var propertyImages = new List<PropertyImage>
        {
            new PropertyImage { PropertyId = 1, ImagePath = "/img/property1.jpg" },
            new PropertyImage { PropertyId = 2, ImagePath = "/img/property2.jpg" }
        };

                    context.PropertyImages.AddRange(propertyImages);
                    await context.SaveChangesAsync();
                }

                //  Rentals
                if (!context.Rentals.Any())
                {
                    var rentals = new List<Rental>
                    {
                        new Rental
                        {
                            
                            PropertyId = context.Properties.FirstOrDefault(p => p.PropertyName == "Moderný byt v centre")?.PropertyId ?? 1,
                            StartDate = DateTime.Now.AddMonths(-6),
                            EndDate = DateTime.Now.AddMonths(-3),
                            //PaymentDay = 15,
                            TenantId = 1
                        },
                        new Rental
                        {
                            
                            PropertyId = context.Properties.FirstOrDefault(p => p.PropertyName == "Rodinný dom v tichom prostredí")?.PropertyId ?? 2,
                            StartDate = DateTime.Now.AddMonths(-8),
                            EndDate = DateTime.Now.AddMonths(-4),
                            //PaymentDay = 1,
                            TenantId = 2
                        }
                    };

                    context.Rentals.AddRange(rentals);
                    await context.SaveChangesAsync();
                }

                //  Issues
                if (!context.Issues.Any())
                {
                    var issues = new List<Issue>
                    {
                        new Issue
                        {
                            Title = "Pokazené kúrenie",
                            Description = "Kúrenie prestalo fungovať v obývačke.",
                            ReportDate = DateTime.Now.AddDays(-7),
                            StatusId = context.IssueStatus.FirstOrDefault(s => s.StatusName == "Rieši sa")?.StatusId ?? 1,
                            RentalId = 1,
                            PropertyId = context.Properties.FirstOrDefault(p => p.PropertyName == "Moderný byt v centre")?.PropertyId ?? 1
                        },
                        new Issue
                        {
                            Title = "Netesniaci odtok",
                            Description = "Voda presakuje cez sprchový odtok.",
                            ReportDate = DateTime.Now.AddDays(-15),
                            SolvedDate = DateTime.Now.AddDays(-5),
                            StatusId = context.IssueStatus.FirstOrDefault(s => s.StatusName == "Vyriešené")?.StatusId ?? 2,
                            RentalId = 2,
                            PropertyId = context.Properties.FirstOrDefault(p => p.PropertyName == "Rodinný dom v tichom prostredí")?.PropertyId ?? 2
                        }
                    };

                    context.Issues.AddRange(issues);
                    await context.SaveChangesAsync();
                }

                //  IssueImages
                if (!context.IssueImages.Any())
                {
                    var images = new List<IssueImage>
                    {
                        new IssueImage { ImagePath = "/img/heatingIssue.jpg", IssueId = 1 },
                        new IssueImage { ImagePath = "/img/drainIssue.jpeg", IssueId = 2 }
                    };

                    context.IssueImages.AddRange(images);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
