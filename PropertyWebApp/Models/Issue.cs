using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Issue
    {
        [Key]
        public int IssueId { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; } // FK to IssueStatus
        public int RentalId { get; set; } // FK to Rental
        public DateTime ReportDate { get; set; }
        public DateTime? SolvedDate { get; set; }

        public int PropertyId { get; set; } // FK to Property



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
