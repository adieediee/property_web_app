using PropertyWebApp.Models;

namespace PropertyWebApp.Components.Pages
{
    using Microsoft.EntityFrameworkCore;
    using PropertyWebApp.Data;
    using PropertyWebApp.Models;
    using PropertyWebApp.Models.Services.PropertyWebApp.Services;

    public class IssueScreenViewModel2
    {
        public List<Issue> Issues { get; set; } = new();
        public int? ExpandedIssueId { get; set; }
        public bool ShowConfirmDialog { get; set; }
        public int? IssueIdToDelete { get; set; }

        private readonly IssueService _issueService;
        private readonly AppDbContext _dbContext;

        public IssueScreenViewModel2(IssueService issueService, AppDbContext dbContext)
        {
            _issueService = issueService;
            _dbContext = dbContext;
        }

        public async Task LoadIssuesAsync()
        {
            Issues = await _dbContext.Issues
                .Include(i => i.Images)
                .Include(i => i.Status)
                .Include(i => i.TaggedIssues).ThenInclude(ti => ti.Tag)
                .AsNoTracking()
                .ToListAsync();
        }

        public string GetPropertyName(int propertyId)
        {
            var property = _dbContext.Properties.FirstOrDefault(p => p.PropertyId == propertyId);
            return property?.PropertyName ?? "Neznáma nehnuteľnosť";
        }

        public async Task<string> GetPropertyImageAsync(int propertyId)
        {
            var propertyImage = await _dbContext.PropertyImages
                .AsNoTracking()
                .FirstOrDefaultAsync(pi => pi.PropertyId == propertyId);
            return propertyImage?.ImagePath ?? "/img/placeholder.png";
        }

        public decimal? GetIssueCost(int issueId)
        {
            var repairCost = _dbContext.RepairCosts.FirstOrDefault(rc => rc.RepairId == issueId);
            return repairCost?.PaymentId;
        }

        public void ToggleDetails(int issueId)
        {
            ExpandedIssueId = ExpandedIssueId == issueId ? null : issueId;
        }

        public void DisplayConfirmDialog(int issueId)
        {
            IssueIdToDelete = issueId;
            ShowConfirmDialog = true;
        }

        public void CancelDelete()
        {
            ShowConfirmDialog = false;
            IssueIdToDelete = null;
        }

        public async Task ConfirmDeleteAsync()
        {
            if (IssueIdToDelete.HasValue)
            {
                bool success = await _issueService.DeleteIssueAsync(IssueIdToDelete.Value);

                if (success)
                {
                    await LoadIssuesAsync();
                }
                else
                {
                    Console.WriteLine("Error deleting issue.");
                }
            }
            CancelDelete();
        }
    }

}
