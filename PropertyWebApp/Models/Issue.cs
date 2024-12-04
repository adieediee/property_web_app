using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Issue
    {
        [Key]
        public int IssueId { get; set; }

        [Required(ErrorMessage = "Názov poruchy je povinnı.")]
        [StringLength(100, ErrorMessage = "Názov poruchy môe obsahova maximálne 100 znakov.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Status je povinnı.")]
        public int StatusId { get; set; } // FK to IssueStatus

        [Required(ErrorMessage = "Nájom (Rental) je povinnı.")]
        public int RentalId { get; set; } // FK to Rental

        [Required(ErrorMessage = "Dátum nahlásenia poruchy je povinnı.")]
        [DataType(DataType.Date, ErrorMessage = "Neplatnı formát dátumu.")]
        public DateTime ReportDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Neplatnı formát dátumu.")]
        public DateTime? SolvedDate { get; set; }

        [Required(ErrorMessage = "Nehnute¾nos je povinná.")]
        public int PropertyId { get; set; } // FK to Property

        [Required(ErrorMessage = "Popis poruchy je povinnı.")]
        [StringLength(1000, ErrorMessage = "Popis poruchy môe obsahova maximálne 1000 znakov.")]
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
