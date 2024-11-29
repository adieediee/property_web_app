namespace PropertyWebApp.Models.Services
{
    public class PropertyService
    {
        private readonly List<Property> _properties = new();

        public Task<List<Property>> GetPropertiesAsync()
        {
            return Task.FromResult(_properties);
        }

        public Task AddPropertyAsync(Property property)
        {
            _properties.Add(property);
            return Task.CompletedTask;
        }

        public Task DeletePropertyAsync(int propertyId)
        {
            var property = _properties.FirstOrDefault(p => p.PropertyId == propertyId);
            if (property != null)
            {
                _properties.Remove(property);
            }
            return Task.CompletedTask;
        }
    }

}
