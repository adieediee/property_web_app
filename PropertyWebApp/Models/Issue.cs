using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Issue
    {
        [Key]
        public int IssueId { get; set; }

        [Required(ErrorMessage = "N�zov poruchy je povinn�.")]
        [StringLength(100, ErrorMessage = "N�zov poruchy m��e obsahova� maxim�lne 100 znakov.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Status je povinn�.")]
        public int StatusId { get; set; } // FK to IssueStatus

        [Required(ErrorMessage = "N�jom (Rental) je povinn�.")]
        public int RentalId { get; set; } // FK to Rental

        [Required(ErrorMessage = "D�tum nahl�senia poruchy je povinn�.")]
        [DataType(DataType.Date, ErrorMessage = "Neplatn� form�t d�tumu.")]
        public DateTime ReportDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Neplatn� form�t d�tumu.")]
        public DateTime? SolvedDate { get; set; }

        [Required(ErrorMessage = "Nehnute�nos� je povinn�.")]
        public int PropertyId { get; set; } // FK to Property

        [Required(ErrorMessage = "Popis poruchy je povinn�.")]
        [StringLength(1000, ErrorMessage = "Popis poruchy m��e obsahova� maxim�lne 1000 znakov.")]
        public string Description { get; set; }

        // Navigation properties
        public IssueStatus Status { get; set; }
        public Rental Rental { get; set; }
        public ICollection<IssueImage> Images { get; set; }
        public ICollection<TaggedIssue> TaggedIssues { get; set; }
        public ICollection<Repair> Repairs { get; set; } 
        public Property Property { get; set; }

    }
}
