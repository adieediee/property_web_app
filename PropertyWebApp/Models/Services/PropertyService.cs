namespace PropertyWebApp.Models.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PropertyWebApp.Data;
    using global::PropertyWebApp.Data.ViewModels;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Identity.Client;

    public class PropertyService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly UserStateService _userStateService;

        public PropertyService(IDbContextFactory<AppDbContext> dbContextFactory, UserStateService userStateService)
        {
            _dbContextFactory = dbContextFactory;
            _userStateService = userStateService;
        }

        public async Task<List<Property>> LoadMyProperties()
        {
            return await GetUserPropertiesAsync(_userStateService.Id, _userStateService.Role);
        }

        public async Task<List<Property>> GetAvailablePropertiesAsync()
        {
            // Napríklad Entity Framework Core:
            using var dbContext = _dbContextFactory.CreateDbContext();
            var properties = await dbContext.Properties
                .Include(properties => properties.PropertyImages)
                .Where(p => p.IsAvailable) // Filtrovanie dostupných nehnuteľností
                .ToListAsync();
            return properties;
        }


        public async Task<List<Property>> GetUserPropertiesAsync(string userId, string role)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            // Základný dotaz
            var query = dbContext.Properties
                .Include(p => p.Rentals)
                .Include(p => p.PropertyImages)
                .AsNoTracking();

            // Dynamické filtrovanie podľa role
            if (role == "Tenant")
            {
                query = query.Where(p => p.Rentals.Any(r => r.TenantId == userId));
            }
            else if (role == "Landlord")
            {
                query = query.Where(p => p.PropertyOwnerId == userId);
            }

            return await query.ToListAsync();
        }

        public async Task<List<PropertyViewModel>> LoadUserPropertyViewsAsync(string tenantId, string role)
        {

            // Načítajte všetky nehnuteľnosti pre daného nájomcu
            //TODO dat pred napevno tenant
            var properties = await GetUserPropertiesAsync(tenantId, role);

            // Spracovanie dát mimo LINQ dotazu
            var propertyViewModels = new List<PropertyViewModel>();

            foreach (var property in properties)
            {
                string userName;
                if (role == "Tenant")
                {
                    userName = await GetLandlordNameByProperty(property.PropertyId);
                }
                else
                {
                    userName = await GetTenantNameByProperty(property.PropertyId);
                }
                // Načítajte meno nájomcu pre danú nehnuteľnosť


                // Zostavte PropertyScreenViewModel
                propertyViewModels.Add(new PropertyViewModel
                {
                    PropertyId = property.PropertyId,
                    PropertyName = property.PropertyName,
                    City = property.City,

                    MainImage = property.PropertyImages.FirstOrDefault()?.ImagePath ?? "/img/default-placeholder-property.jpg",
                    Price = property.Price,
                    NumberOfBedrooms = property.NumberOfBedrooms,
                    NumberOfBathrooms = property.NumberOfBathrooms,
                    Area = property.Area,
                    Description = property.Description,
                    UserName = userName
                });
            }

            return propertyViewModels;
        }
        // Načítanie všetkých nehnuteľností cez VM
        public async Task<List<PropertyViewModel>> GetAllPropertiesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Properties
                .Include(p => p.PropertyImages)
                .Select(p => new PropertyViewModel
                {
                    PropertyId = p.PropertyId,
                    PropertyName = p.PropertyName,
                    City = p.City,

                    MainImage = p.PropertyImages.FirstOrDefault() != null ? p.PropertyImages.FirstOrDefault().ImagePath : "/img/default-placeholder-property.jpg",
                    Price = p.Price,
                    NumberOfBedrooms = p.NumberOfBedrooms,
                    NumberOfBathrooms = p.NumberOfBathrooms,
                    Area = p.Area,
                    Description = p.Description
                })
                .ToListAsync();
        }

        // Načítanie konkrétnej nehnuteľnosti podľa ID
        public async Task<PropertyViewModel> GetPropertyViewByIdAsync(int propertyId)
        {
            //TODO poriesit tento skaredy warning
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var property = await _dbContext.Properties
                .Include(p => p.PropertyImages)
                .Where(p => p.PropertyId == propertyId)
                .FirstOrDefaultAsync();
            //DOROBIT
            var tenant = "";
            var p = property;
            var propertyVM = new PropertyViewModel
            {
                PropertyId = p.PropertyId,
                PropertyName = p.PropertyName,
                City = p.City,

                MainImage = p.PropertyImages.FirstOrDefault()?.ImagePath ?? "/img/default-placeholder-property.jpg",

                Price = p.Price,
                NumberOfBedrooms = p.NumberOfBedrooms,
                NumberOfBathrooms = p.NumberOfBathrooms,
                Area = p.Area,
                Description = p.Description,
                UserName = tenant
            };
            return propertyVM;
        }

        // Metóda na vymazanie nehnuteľnosti
        public async Task<bool> DeletePropertyAsync(int propertyId)
        {
            // Odstránenie Property a závislostí bez DbContextFactory
            // TODO DbContextFactory pridať
            using var _dbContext = _dbContextFactory.CreateDbContext();
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Načítajte Property a závislostí
                var property = await _dbContext.Properties
                    .Include(p => p.PropertyImages)
                    .Include(p => p.Rentals)
                    .Include(p => p.Issues)
                        .ThenInclude(i => i.Images)
                    .Include(p => p.Issues)
                        .ThenInclude(i => i.TaggedIssues)
                    .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

                if (property == null)
                {
                    return false;
                }


                foreach (var issue in property.Issues)
                {
                    if (issue.Images != null)
                    {
                        _dbContext.IssueImages.RemoveRange(issue.Images);
                    }

                    if (issue.TaggedIssues != null)
                    {
                        _dbContext.TaggedIssues.RemoveRange(issue.TaggedIssues);
                    }
                }

                // Kaskádové odstránenie závislostí 

                // Odstránenie Issues
                _dbContext.Issues.RemoveRange(property.Issues);

                // Odstránenie Rentals
                _dbContext.Rentals.RemoveRange(property.Rentals);

                // Odstránenie PropertyImages
                _dbContext.PropertyImages.RemoveRange(property.PropertyImages);

                // Nakoniec odstránenie Property
                _dbContext.Properties.Remove(property);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error deleting property: {ex.Message}");
                throw;
            }
        }
        public async Task<string> GetPropertyImageAsync(int propertyId)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var propertyImage = await _dbContext.PropertyImages
                .AsNoTracking()
                .FirstOrDefaultAsync(pi => pi.PropertyId == propertyId);
            return propertyImage?.ImagePath ?? "/img/placeholder.png";
        }

        internal async Task<List<Property>> GetPropertiesByTenantIdAsync(string tenantId)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Properties
                .Where(p => p.Rentals.Any(r => r.TenantId == tenantId))
                .Include(p => p.PropertyImages)
                .ToListAsync();
        }
        //TU JE NIECO ZLE
        internal async Task<string> GetTenantNameByProperty(int propertyID)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();


            // Získaj TenantId pre konkrétnu Property
            var tenantID = _dbContext.Rentals
                .Where(r => r.PropertyId == propertyID)
                .Select(r => r.TenantId) // Vyber TenantId
                .FirstOrDefault();       // Vráť prvý záznam (alebo null)

            if (tenantID != null) // Over, či TenantId existuje
            {
                // Nájdeme používateľa podľa TenantId
                var tenant = await _dbContext.Users
                    .Where(u => u.Id == tenantID)   // Filtrovanie podľa TenantId
                    .Select(u => u.UserName)        // Vyber UserName
                    .FirstOrDefaultAsync();
                if (tenant != null)
                {
                    return tenant;
                }
                else
                {
                    return "error";
                }
                // Vráť výsledok

                // Teraz môžeš použiť `tenant` (obsahuje UserName)
            }
            else
            {
                return ""; // Ak TenantId neexistuje, vráť prázdny reťazec
            }

        }
        internal async Task<string> GetLandlordNameByProperty(int propertyID)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            // Získaj TenantId pre konkrétnu Property
            var tenantID = _dbContext.Rentals
                .Where(r => r.PropertyId == propertyID)
                .Select(r => r.Property.PropertyOwner) // Vyber TenantId
                .FirstOrDefault();       // Vráť prvý záznam (alebo null)

            if (tenantID != null) // Over, či TenantId existuje
            {
                // Nájdeme používateľa podľa TenantId
                var tenant = await _dbContext.Users
                    .Where(u => u.Id == tenantID.Id)   // Filtrovanie podľa TenantId
                    .Select(u => u.UserName)        // Vyber UserName
                    .FirstOrDefaultAsync();
                if (tenant != null)
                {
                    return tenant;
                }
                else
                {
                    return "error";
                }
                // Vráť výsledok

                // Teraz môžeš použiť `tenant` (obsahuje UserName)
            }
            else
            {
                return ""; // Ak TenantId neexistuje, vráť prázdny reťazec
            }

        }

        public async Task<List<Property>> SearchPropertiesAsync(string name, string location, string type, decimal? maxPrice)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            // Začiatok dotazu - získaj všetky nehnuteľnosti
            var query = _dbContext.Properties
                .Include(p => p.PropertyImages)
                .Where(p => p.IsAvailable)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.PropertyName.Contains(name));
            }

            // Filtruj podľa lokality, ak je zadaná
            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(p => p.City.Contains(location) || p.StreetName.Contains(location));
            }

            // Filtruj podľa typu nehnuteľnosti, ak je zadaný
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(p => p.PropertyType.TypeName == type);
            }

            // Filtruj podľa maximálnej ceny, ak je zadaná
            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Vykonaj dotaz a vráť výsledky
            return await query.ToListAsync();
        }
        public async Task<List<PropertyType>> GetPropertyTypesAsync()
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.PropertyTypes.ToListAsync();
        }
        public async Task GetPropertyType(int propertyID)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var propertyType = await _dbContext.Properties
                .Where(pt => pt.PropertyId == propertyID)
                .FirstOrDefaultAsync();

        }
    }

   
   
}
