using PropertyWebApp.Models;

namespace PropertyWebApp.Components.Pages
{
    // VM pre Issue Screen
    //TODO refaktorovat kod na pouzitie ViewModel

    public class IssueViewModel
    {
        public int IssueId { get; set; }
        public string Title { get; set; }
        public DateTime ReportDate { get; set; }
        public string StatusName { get; set; }
        public string PropertyName { get; set; }
        public string PropertyImagePath { get; set; }
        public decimal? RepairCost { get; set; }
        public List<TaggedIssue> TaggedIssues { get; set; }
        public string Description { get; set; }

        public List<IssueImage> Images { get; set; }
    }
}
