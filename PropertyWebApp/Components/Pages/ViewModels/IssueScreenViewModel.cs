using PropertyWebApp.Data;
using PropertyWebApp.Models.Services.PropertyWebApp.Services;
using PropertyWebApp.Models;
using PropertyWebApp.Models.Services;


namespace PropertyWebApp.Components.Pages.ViewModels
{

    public class IssueScreenViewModel
    {
        public List<Issue> Issues { get; set; } = new();
        public List<Issue> FilteredIssues { get; set; } = new();

        Dictionary<int, string > imageMap = new Dictionary<int, string>();
        public List<string> PropertyIssueImage { get; set; } = new();
        public int? ExpandedIssueId { get; set; }
        public bool ShowConfirmDialog { get; set; }
        public int? IssueIdToDelete { get; set; }

        private readonly IssueService _issueService;
        private readonly PropertyService _propertyService;

        public IssueScreenViewModel(IssueService issueService, PropertyService propertyService)
        {
            _issueService = issueService;
            _propertyService = propertyService;
        }

        public async Task LoadIssuesAsync(string id)
        {

            Issues = await _issueService.GetIssuesAsync(id);

        }

        public async Task LoadPropertyIssueImageAsync()
        {
            foreach (var issue in Issues)
            {
                var propertyImage = await _propertyService.GetPropertyImageAsync(issue.PropertyId);
                if (!imageMap.ContainsKey(issue.PropertyId))
                {
                    imageMap.Add(issue.PropertyId, propertyImage);
                }
            }
        }

        public string GetPropertyImage(int number)
        {
            if (imageMap.ContainsKey(number))
            {
                return imageMap[number];
            }
            return "/img/placeholder.png";
        }

        public async Task<string> GetPropertyImageAsync(int propertyId)
        {
            var propertyImage = await _propertyService.GetPropertyImageAsync(propertyId);
            return propertyImage != null ? propertyImage : "/img/placeholder.png";
        }

        public async Task<string> GetPropertyNameAsync(int propertyId)
        {
            var property = await _propertyService.GetPropertyViewByIdAsync(propertyId);
            return property?.PropertyName ?? "Unknown";
        }

        public decimal? GetIssueCost(int issueId)
        {
            //TODO dorobit issue cost - tabulka repair
            //return _issueService.GetIssueByIdAsync(issueId).Result.
            return issueId * 2;
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
                FilteredIssues.RemoveAll(i => i.IssueId == IssueIdToDelete.Value);

                if (success)
                {
                    //await LoadIssuesAsync();
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
