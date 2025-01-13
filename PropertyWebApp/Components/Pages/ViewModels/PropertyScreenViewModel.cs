namespace PropertyWebApp.Components.Pages.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using PropertyWebApp.Data.ViewModels;
    using PropertyWebApp.Models.Services;

    public class PropertyScreenViewModel
    {
        private readonly PropertyService _propertyService;
        private readonly UserStateService _userStateService;

        public List<PropertyViewModel> Properties { get; private set; } = new();
        public List<PropertyViewModel> FilteredProperties { get; private set; } = new();
        public PropertyViewModel? SelectedProperty { get; private set; }
        public bool showDeleteConfirmation { get; private set; } = false;

        public PropertyScreenViewModel(PropertyService propertyService, UserStateService userStateService)
        {
            _propertyService = propertyService;
            _userStateService = userStateService;
        }

        public async Task InitializeAsync()
        {
            Properties = await _propertyService.LoadUserPropertyViewsAsync(_userStateService.Id, _userStateService.Role);
            FilteredProperties = Properties;
            
        }

        public void PerformPropertySearch(string query)
        {
            FilteredProperties = Properties
                .Where(i => (i.PropertyName != null && i.PropertyName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                            (i.Description != null && i.Description.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public async Task ShowPropertyDetails(int propertyId)
        {
            SelectedProperty = await _propertyService.GetPropertyViewByIdAsync(propertyId);
        }
        public void HidePropertyDetails()
        {
            SelectedProperty = null;
        }



        public void ShowDeleteConfirmation()
        {
            showDeleteConfirmation = true;
        }

        public async Task ConfirmDelete()
        {
            if (SelectedProperty != null)
            {
                bool isDeleted = await _propertyService.DeletePropertyAsync(SelectedProperty.PropertyId);
                if (isDeleted)
                {
                    Properties.RemoveAll(p => p.PropertyId == SelectedProperty.PropertyId);
                    SelectedProperty = null;
                    FilteredProperties = Properties;
                }
            }

            showDeleteConfirmation = false;
        }

        public void CancelDelete()
        {
            showDeleteConfirmation = false;
        }


    }
}
