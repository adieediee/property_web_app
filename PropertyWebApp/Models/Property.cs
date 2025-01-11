using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }
        [Required(ErrorMessage = "Pole dostupnosti je povinné.")]
        public bool IsAvailable { get; set; }

        [Required(ErrorMessage = "Názov nehnute¾nosti je povinnı.")]
        [StringLength(100, ErrorMessage = "Názov nehnute¾nosti môe ma maximálne 100 znakov.")]
        public string PropertyName { get; set; }

        [Required(ErrorMessage = "Typ nehnute¾nosti je povinnı.")]
        public int TypeId { get; set; } // FK to PropertyType

        [Required(ErrorMessage = "Dátum pridania je povinnı.")]
        public DateTime ListingDate { get; set; }

        [StringLength(1000, ErrorMessage = "Popis môe ma maximálne 1000 znakov.")]
        [Required(ErrorMessage = "Popis Nehnutelnosti je povinnı.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ulica je povinná.")]
        [StringLength(100, ErrorMessage = "Ulica môe ma maximálne 100 znakov.")]
        public string StreetName { get; set; }

        [Required(ErrorMessage = "Mesto je povinné.")]
        [StringLength(50, ErrorMessage = "Mesto môe ma maximálne 50 znakov.")]
        public string City { get; set; }

        [Required(ErrorMessage = "PSÈ je povinné.")]
        [RegularExpression(@"\d{5}", ErrorMessage = "PSÈ musí obsahova 5 èíslic.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Krajina je povinná.")]
        [StringLength(50, ErrorMessage = "Krajina môe ma maximálne 50 znakov.")]
        public string Country { get; set; }

        
        

        [Range(0, 1_000_000, ErrorMessage = "Cena musí by v rozmedzí od 0 do 1 000 000.")]
        public decimal Price { get; set; }

        [Range(1, 10_000, ErrorMessage = "Rozloha musí by v rozmedzí od 1 do 10 000 m?.")]
        public int Area { get; set; }

        [Range(0, 20, ErrorMessage = "Poèet spální musí by v rozmedzí od 0 do 20.")]
        public short NumberOfBedrooms { get; set; }

        [Range(0, 20, ErrorMessage = "Poèet kúpe¾ní musí by v rozmedzí od 0 do 20.")]
        public short NumberOfBathrooms { get; set; }

        [Required(ErrorMessage = "Informácia o zariadení je povinná.")]
        public bool IsFurnished { get; set; }

        [Required(ErrorMessage = "Informácia o parkovaní je povinná.")]
        public bool ParkingAvailable { get; set; }

        // Navigation properties
        public PropertyType PropertyType { get; set; }
        public ICollection<PropertyImage> PropertyImages { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public ICollection<Rental> Rentals { get; set; }
    }
}
