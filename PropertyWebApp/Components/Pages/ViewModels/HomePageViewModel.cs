using Microsoft.JSInterop;
using PropertyWebApp.Models.Services;
using PropertyWebApp.Models;

namespace PropertyWebApp.Components.Pages.ViewModels
{
    public class HomePageViewModel
    {
        private readonly PropertyService _propertyService;
        private readonly IJSRuntime _jsRuntime;

        public List<Property> AvailableProperties { get; set; } = new List<Property>();
        public List<Property> FilteredProperties { get; set; } = new List<Property>();
        public List<PropertyType> PropertyTypes { get; set; } = new List<PropertyType>();
        public string SearchLocation { get; set; } = string.Empty;
        public string SearchType { get; set; } = string.Empty;
        public string SearchPrice { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SortBy { get; set; } = "Default";

        public HomePageViewModel(PropertyService propertyService, IJSRuntime jsRuntime)
        {
            _propertyService = propertyService;
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            PropertyTypes = await _propertyService.GetPropertyTypesAsync();
            AvailableProperties = await _propertyService.GetAvailablePropertiesAsync();
            FilteredProperties = AvailableProperties; // Default filter
        }

        public async Task HandleSearchAsync()
        {
            decimal.TryParse(SearchPrice, out var maxPrice);
            FilteredProperties = await _propertyService.SearchPropertiesAsync(Name, SearchLocation, SearchType, maxPrice);
            await ScrollToResultsAsync();
        }

        public async Task HandlePopularSearchAsync(string location, string type)
        {
            SearchLocation = location;
            SearchType = type;
            await HandleSearchAsync();
        }

        public async Task HandleSortChangeAsync(string sortBy)
        {
            SortBy = sortBy;
            ApplySorting();
        }

        public void ApplySorting()
        {
            switch (SortBy)
            {
                case "PriceLowHigh":
                    FilteredProperties = FilteredProperties.OrderBy(p => p.Price).ToList();
                    break;
                case "PriceHighLow":
                    FilteredProperties = FilteredProperties.OrderByDescending(p => p.Price).ToList();
                    break;
                case "Newest":
                    FilteredProperties = FilteredProperties.OrderByDescending(p => p.ListingDate).ToList();
                    break;
            }
        }

        private async Task ScrollToResultsAsync()
        {
            await _jsRuntime.InvokeVoidAsync("scrollToElement", "resultSection");
        }
    }
}
