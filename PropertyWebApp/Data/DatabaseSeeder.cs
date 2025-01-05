using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
                // Seedovanie používateľov a rolí
                var scopedServiceProvider = scope.ServiceProvider;

                // Získanie RoleManager a UserManager zo scoped provideru
                var roleManager = scopedServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scopedServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Vytvorenie rolí
                if (!await roleManager.RoleExistsAsync("Tenant"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Tenant"));
                }
                if (!await roleManager.RoleExistsAsync("Landlord"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Landlord"));
                }

                // Vytvorenie predvoleného nájomcu
                if (await userManager.FindByEmailAsync("tenant@example.com") == null)
                {
                    var tenantUser = new IdentityUser
                    {
                        UserName = "tenant@example.com",
                        Email = "tenant@example.com",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(tenantUser, "Tenant@12345");
                    await userManager.AddToRoleAsync(tenantUser, "Tenant");
                }

                // Vytvorenie predvoleného prenajímateľa
                if (await userManager.FindByEmailAsync("landlord@example.com") == null)
                {
                    var landlordUser = new IdentityUser
                    {
                        UserName = "landlord@example.com",
                        Email = "landlord@example.com",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(landlordUser, "Landlord@12345");
                    await userManager.AddToRoleAsync(landlordUser, "Landlord");
                }




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
                    var landlord = await userManager.FindByEmailAsync("landlord@example.com");
                    var tenant = await userManager.FindByEmailAsync("tenant@example.com");
                    //var id = tenant.Id;
                    //var id2 = landlord.Id;
                    var rentals = new List<Rental>()
                    {
                        new Rental
                        {
                            PropertyId = context.Properties.FirstOrDefault(p => p.PropertyName == "Moderný byt v centre")?.PropertyId ?? 1,
                            StartDate = DateTime.Now.AddMonths(-6),
                            EndDate = DateTime.Now.AddMonths(-3),
                            //PaymentDay = 15,
                            TenantId = tenant.Id,
                            PropertyOwnerId = landlord.Id
                        },
                        new Rental
                        {

                            PropertyId = context.Properties.FirstOrDefault(p => p.PropertyName == "Rodinný dom v tichom prostredí")?.PropertyId ?? 2,
                            StartDate = DateTime.Now.AddMonths(-8),
                            EndDate = DateTime.Now.AddMonths(-4),
                            //PaymentDay = 1,
                            TenantId = tenant.Id,
                            PropertyOwnerId = landlord.Id
                        }
                    };

                    context.Rentals.AddRange(rentals);
                    await context.SaveChangesAsync();
                    // MonthlyPayments
                    foreach (var rental in rentals)
                    {
                        for (int i = 1; i <= 6; i++) // Generuj platby na posledných 6 mesiacov
                        {
                            var paymentDate = DateTime.Now.AddMonths(-i);

                            var payment = new MonthlyPayment
                            {
                                RentalId = rental.RentalId,
                                PaymentDate = paymentDate,
                                RentAmount = 500,
                                UtilitiesAmount = 150,
                                isPaid = i % 2 == 0 // Napríklad: každý druhý mesiac je zaplatený
                            };

                            context.MonthlyPayments.Add(payment);

                        }
                    }
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
