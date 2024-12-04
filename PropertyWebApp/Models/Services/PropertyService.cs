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
        private readonly AppDbContext _dbContext;

        public PropertyService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Načítanie všetkých nehnuteľností cez VM
        public async Task<List<PropertyScreenViewModel>> GetAllPropertiesAsync()
        {
            return await _dbContext.Properties
                .Include(p => p.PropertyImages)
                .Select(p => new PropertyScreenViewModel
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
        public async Task<PropertyScreenViewModel> GetPropertyByIdAsync(int propertyId)
        {
            //TODO poriesit tento skaredy warning
            return await _dbContext.Properties
                .Include(p => p.PropertyImages)
                .Where(p => p.PropertyId == propertyId)
                .Select(p => new PropertyScreenViewModel
                {
                    PropertyId = p.PropertyId,
                    PropertyName = p.PropertyName,
                    City = p.City,
                    State = p.State,
                    MainImage = p.PropertyImages.FirstOrDefault().ImagePath ?? "/img/default-placeholder-property.png",
                    Price = p.Price,
                    NumberOfBedrooms = p.NumberOfBedrooms,
                    NumberOfBathrooms = p.NumberOfBathrooms,
                    Area = p.Area,
                    Description = p.Description
                })
                .FirstOrDefaultAsync();
        }

        // Metóda na vymazanie nehnuteľnosti
        public async Task<bool> DeletePropertyAsync(int propertyId)
        {
            // Odstránenie Property a závislostí bez DbContextFactory
            // TODO DbContextFactory pridať
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
    }
}
