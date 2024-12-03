namespace PropertyWebApp.Data.ViewModels
{
    public class PropertyScreenViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string MainImage { get; set; }
        public decimal Price { get; set; }
        public short NumberOfBedrooms { get; set; }
        public short NumberOfBathrooms { get; set; }
        public int Area { get; set; }
        public string Description { get; set; }

        public string TenantName { get; set; }

        public string TenantAvatar = "/img/default-avatar.jpg";
    }
}
