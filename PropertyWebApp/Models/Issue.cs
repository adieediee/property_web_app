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
        public string Description { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime? SolvedDate { get; set; }
        public decimal? RepairCost { get; set; }
        public int StatusId { get; set; } // FK na IssueStatus
        public int RentalId { get; set; } // FK na Rental

        // Navigačné vlastnosti
        public IssueStatus Status { get; set; }
        //public Rental Rental { get; set; }
        public ICollection<IssueImage> Images { get; set; }
    }
}