namespace PropertyWebApp.Models.Services
{
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

        // Načítanie všetkých nehnuteľností ako ViewModel
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
                    MainImage = p.PropertyImages.FirstOrDefault().ImagePath ?? "/img/default-placeholder.png",
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
    }


}
