using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

                // Predvoleny prenajimatel
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

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();


                if (!context.Tags.Any())
                {
                    var tags = new List<Tag>
                    {
                        new Tag { TagName = "Elektrina" },
                        new Tag { TagName = "Voda" },
                        new Tag { TagName = "Kúrenie" },
                        new Tag { TagName = "Okná" },
                        new Tag { TagName = "Strecha" }
                    };

                    context.Tags.AddRange(tags);
                    await context.SaveChangesAsync();
                }

                // IssueStatus
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
                        new PropertyType { TypeName = "Dom" },
                        new PropertyType { TypeName = "Byt" },
                        new PropertyType { TypeName = "Chata" }
                  
                    };

                    context.PropertyTypes.AddRange(propertyTypes);
                    await context.SaveChangesAsync();
                }

                //  TypeId pre PropertyTypes
                var apartmentTypeId = context.PropertyTypes.FirstOrDefault(pt => pt.TypeName == "Byt")?.TypeId;
                var houseTypeId = context.PropertyTypes.FirstOrDefault(pt => pt.TypeName == "Dom")?.TypeId;
                var chaletTypeId = context.PropertyTypes.FirstOrDefault(pt => pt.TypeName == "Chata")?.TypeId;

                var landlord = await userManager.FindByEmailAsync("landlord@example.com");
                var tenant = await userManager.FindByEmailAsync("tenant@example.com");
                //  Properties
                if (!context.Properties.Any())
                {
                    var properties = new List<Property>
                    {
                        new Property
                        {
                            PropertyName = "Moderný byt v centre",
                            IsAvailable = false,
                            TypeId = apartmentTypeId.Value, 
                            ListingDate = DateTime.Now.AddDays(-30),
                            Description = "Priestranný 2-izbový byt s balkónom v centre mesta.",
                            StreetName = "Hlavná 1",
                            City = "Bratislava",
                            PostalCode = "81101",
                            Country = "Slovensko",
                            
                            Price = 850.00m,
                            Area = 65,
                            NumberOfBedrooms = 2,
                            NumberOfBathrooms = 1,
                            IsFurnished = true,
                            ParkingAvailable = false,

                            PropertyOwnerId = landlord.Id
                        },
                        new Property
                        {
                            PropertyName = "Rodinný dom v tichom prostredí",
                            IsAvailable = false,
                            TypeId = houseTypeId.Value, 
                            ListingDate = DateTime.Now.AddDays(-60),
                            Description = "Štýlový rodinný dom s veľkou záhradou.",
                            StreetName = "Záhradná 12",
                            City = "Žilina",
                            PostalCode = "01001",
                            Country = "Slovensko",
                            
                            Price = 1200.00m,
                            Area = 120,
                            NumberOfBedrooms = 4,
                            NumberOfBathrooms = 2,
                            IsFurnished = false,
                            ParkingAvailable = true,

                            PropertyOwnerId = landlord.Id
                        },
                             new Property
    {
        PropertyName = "Luxusná chata v horách",
        IsAvailable = true,
        TypeId = chaletTypeId.Value, 
        ListingDate = DateTime.Now.AddDays(-15),
        Description = "Krásna chata v horách s výhľadom na panorámu Tatier.",
        StreetName = "Horská 5",
        City = "Vysoké Tatry",
        PostalCode = "06201",
        Country = "Slovensko",
        
        Price = 2000.00m,
        Area = 150,
        NumberOfBedrooms = 3,
        NumberOfBathrooms = 2,
        IsFurnished = true,
        ParkingAvailable = true,

        PropertyOwnerId = landlord.Id
    },
    
    new Property
    {
        PropertyName = "Malý domček pri jazere",
        IsAvailable = true,
        TypeId = houseTypeId.Value, 
        ListingDate = DateTime.Now.AddDays(-45),
        Description = "Útulný domček ideálny na víkendové pobyty pri vode.",
        StreetName = "Jazerná 9",
        City = "Šamorín",
        PostalCode = "93101",
        Country = "Slovensko",
        
        Price = 600.00m,
        Area = 50,
        NumberOfBedrooms = 1,
        NumberOfBathrooms = 1,
        IsFurnished = true,
        ParkingAvailable = true,

        PropertyOwnerId = landlord.Id
    },
    new Property
    {
        PropertyName = "Podkrovný byt s výhľadom na mesto",
        IsAvailable = true,
        TypeId = apartmentTypeId.Value, 
        ListingDate = DateTime.Now.AddDays(-10),
        Description = "Štýlový podkrovný byt s moderným zariadením a krásnym výhľadom.",
        StreetName = "Vyhliadková 3",
        City = "Nitra",
        PostalCode = "94901",
        Country = "Slovensko",
        
        Price = 700.00m,
        Area = 75,
        NumberOfBedrooms = 2,
        NumberOfBathrooms = 1,
        IsFurnished = true,
        ParkingAvailable = false,

        PropertyOwnerId = landlord.Id
    },
                    };

                    context.Properties.AddRange(properties);
                    await context.SaveChangesAsync();
                }

                
                if (!context.PropertyImages.Any())
                {
                    var propertyImages = new List<PropertyImage>
                    {
                        new PropertyImage { PropertyId = 1, ImagePath = "/img/property1.jpg" },
                        new PropertyImage { PropertyId = 2, ImagePath = "/img/property2.jpg" },
                        new PropertyImage { PropertyId = 3, ImagePath = "/img/property3.png" },
                        new PropertyImage { PropertyId = 4, ImagePath = "/img/property4.jpg" },
                        new PropertyImage { PropertyId = 5, ImagePath = "/img/property5.jpg" }
                    };

                    context.PropertyImages.AddRange(propertyImages);
                    await context.SaveChangesAsync();
                }
                

                //  Rentals
                if (!context.Rentals.Any())
                {
                   
                    
                    var rentals = new List<Rental>()
                    {
                        new Rental
                        {
                            PropertyId = context.Properties.FirstOrDefault(p => p.PropertyName == "Moderný byt v centre")?.PropertyId ?? 1,
                            StartDate = DateTime.Now.AddMonths(-6),
                            EndDate = DateTime.Now.AddMonths(-3),
                            //PaymentDay = 15,
                            TenantId = tenant.Id,
                             
                        },
                        new Rental
                        {

                            PropertyId = context.Properties.FirstOrDefault(p => p.PropertyName == "Rodinný dom v tichom prostredí")?.PropertyId ?? 2,
                            StartDate = DateTime.Now.AddMonths(-8),
                            EndDate = DateTime.Now.AddMonths(-4),
                            //PaymentDay = 1,
                            TenantId = tenant.Id,
                            
                        }
                    };

                    context.Rentals.AddRange(rentals);
                    await context.SaveChangesAsync();
                    // MonthlyPayments
                    foreach (var rental in rentals)
                    {
                        for (int i = 1; i <= 6; i++) 
                        {
                            var paymentDate = DateTime.Now.AddMonths(-i);

                            var payment = new MonthlyPayment
                            {
                                RentalId = rental.RentalId,
                                PaymentDate = paymentDate,
                                RentAmount = 500,
                                UtilitiesAmount = 150,
                                isPaid = i % 2 == 0 
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
