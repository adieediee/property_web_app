using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }
        [Required(ErrorMessage = "Pole dostupnosti je povinn�.")]
        public bool IsAvailable { get; set; }

        [Required(ErrorMessage = "N�zov nehnute�nosti je povinn�.")]
        [StringLength(100, ErrorMessage = "N�zov nehnute�nosti m��e ma� maxim�lne 100 znakov.")]
        public string PropertyName { get; set; }

        [Required(ErrorMessage = "Typ nehnute�nosti je povinn�.")]
        public int TypeId { get; set; } // FK to PropertyType

        [Required(ErrorMessage = "D�tum pridania je povinn�.")]
        public DateTime ListingDate { get; set; }

        [StringLength(1000, ErrorMessage = "Popis m��e ma� maxim�lne 1000 znakov.")]
        [Required(ErrorMessage = "Popis Nehnutelnosti je povinn�.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ulica je povinn�.")]
        [StringLength(100, ErrorMessage = "Ulica m��e ma� maxim�lne 100 znakov.")]
        public string StreetName { get; set; }

        [Required(ErrorMessage = "Mesto je povinn�.")]
        [StringLength(50, ErrorMessage = "Mesto m��e ma� maxim�lne 50 znakov.")]
        public string City { get; set; }

        [Required(ErrorMessage = "PS� je povinn�.")]
        [RegularExpression(@"\d{5}", ErrorMessage = "PS� mus� obsahova� 5 ��slic.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Krajina je povinn�.")]
        [StringLength(50, ErrorMessage = "Krajina m��e ma� maxim�lne 50 znakov.")]
        public string Country { get; set; }

        
        

        [Range(0, 1_000_000, ErrorMessage = "Cena mus� by� v rozmedz� od 0 do 1 000 000.")]
        public decimal Price { get; set; }

        [Range(1, 10_000, ErrorMessage = "Rozloha mus� by� v rozmedz� od 1 do 10 000 m?.")]
        public int Area { get; set; }

        [Range(0, 20, ErrorMessage = "Po�et sp�ln� mus� by� v rozmedz� od 0 do 20.")]
        public short NumberOfBedrooms { get; set; }

        [Range(0, 20, ErrorMessage = "Po�et k�pe�n� mus� by� v rozmedz� od 0 do 20.")]
        public short NumberOfBathrooms { get; set; }

        [Required(ErrorMessage = "Inform�cia o zariaden� je povinn�.")]
        public bool IsFurnished { get; set; }

        [Required(ErrorMessage = "Inform�cia o parkovan� je povinn�.")]
        public bool ParkingAvailable { get; set; }

        // Navigation properties
        public PropertyType PropertyType { get; set; }
        public ICollection<PropertyImage> PropertyImages { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public ICollection<Rental> Rentals { get; set; }
    }
}
