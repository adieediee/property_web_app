namespace PropertyWebApp.Models.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::PropertyWebApp.Data;
    using global::PropertyWebApp.Data.ViewModels;
    using Microsoft.EntityFrameworkCore;

    public class PropertyService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public PropertyService(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;  
        }
        public async Task<List<PropertyViewModel>> LoadUserPropertyViewsAsync(string tenantId)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            // Načítajte všetky nehnuteľnosti pre daného nájomcu
            var properties = await dbContext.Properties
                .Where(p => p.Rentals.Any(r => r.TenantId == tenantId)) // Filtrovanie podľa TenantId
                .Include(p => p.PropertyImages) // Načítajte obrázky nehnuteľností
                .AsNoTracking() // Zníženie režijných nákladov pri čítaní údajov
                .ToListAsync();

            // Spracovanie dát mimo LINQ dotazu
            var propertyViewModels = new List<PropertyViewModel>();

            foreach (var property in properties)
            {
                // Načítajte meno nájomcu pre danú nehnuteľnosť
                var tenantName = await GetTenantNameByProperty(property.PropertyId);

                // Zostavte PropertyScreenViewModel
                propertyViewModels.Add(new PropertyViewModel
                {
                    PropertyId = property.PropertyId,
                    PropertyName = property.PropertyName,
                    City = property.City,
                    State = property.State,
                    MainImage = property.PropertyImages.FirstOrDefault()?.ImagePath ?? "/img/default-placeholder-property.jpg",
                    Price = property.Price,
                    NumberOfBedrooms = property.NumberOfBedrooms,
                    NumberOfBathrooms = property.NumberOfBathrooms,
                    Area = property.Area,
                    Description = property.Description,
                    TenantName = tenantName
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
                    State = p.State,
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
                State = p.State,
                MainImage = p.PropertyImages.FirstOrDefault()?.ImagePath ?? "/img/default-placeholder-property.jpg",

                Price = p.Price,
                NumberOfBedrooms = p.NumberOfBedrooms,
                NumberOfBathrooms = p.NumberOfBathrooms,
                Area = p.Area,
                Description = p.Description,
                TenantName = tenant
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
                var tenant =  await _dbContext.Users
                    .Where(u => u.Id == tenantID)   // Filtrovanie podľa TenantId
                    .Select(u => u.UserName)        // Vyber UserName
                    .FirstOrDefaultAsync();
                if (tenant != null)
                {
                    return tenant;
                } else
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
    }
}
