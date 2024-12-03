using System;
using System.Collections.Generic;

namespace PropertyWebApp.Models
{
    public class Property
    {
        public int PropertyId { get; set; }
        public bool IsAvailable { get; set; }
        public string PropertyName { get; set; }
        public int TypeId { get; set; } // FK to PropertyType
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

        // Navigation properties
        public PropertyType PropertyType { get; set; }
        public ICollection<PropertyImage> PropertyImages { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public ICollection<Rental> Rentals { get; set; }
    }
}
