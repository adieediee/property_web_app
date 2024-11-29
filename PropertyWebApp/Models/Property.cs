using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }
        public bool IsAvailable { get; set; }
        public string PropertyName { get; set; }
        public int TypeId { get; set; }
        public DateTime ListingDate { get; set; }
        public string Description { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public decimal Price { get; set; }
        public int Area { get; set; }
        public short NumberOfBedrooms { get; set; }
        public short NumberOfBathrooms { get; set; }
        public bool IsFurnished { get; set; }
        public bool ParkingAvailable { get; set; }

    }
}
