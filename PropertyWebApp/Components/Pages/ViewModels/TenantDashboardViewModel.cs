using PropertyWebApp.Models.Services.PropertyWebApp.Services;
using PropertyWebApp.Models.Services;
using PropertyWebApp.Models;
using PropertyWebApp.Data.ViewModels;
using System.Data;


public class TenantDashboardViewModel
{
    public string TenantName { get; private set; } = "Unknown Tenant";

    
    private Dictionary<int, string> _propertyImages = new();
    
    public string UserRole = "Tenant";


    public List<PropertyViewModel> Properties { get; private set; } = new();
    public List<Issue> UnresolvedIssues { get; private set; } = new();
    public List<MonthlyPayment> Payments { get; private set; } = new();
    public MonthlyPayment UpcomingPayment { get; private set; }
    public decimal TotalMonthlyRent { get; private set; }
    public bool AreAllRentsPaid { get; private set; }
    public List<string> Notifications { get; private set; } = new();
    public int ResolvedIssuesCount { get; private set; }

    private readonly PropertyService _propertyService;
    private readonly IssueService _issueService;
    private readonly RentalService _rentalService;
    private readonly UserStateService _userStateService;

    public TenantDashboardViewModel(PropertyService propertyService, IssueService issueService, RentalService rentalService, UserStateService userStateService)
    {
        _propertyService = propertyService;
        _issueService = issueService;
        _rentalService = rentalService;
        _userStateService = userStateService;   
    }

    public async Task LoadDataAsync(string tenantId)
    {
        UserRole = _userStateService.Role;

        if (UserRole == "Tenant")
        {
            TenantName = await LoadTenantNameAsync(tenantId);
        }
       
        // Load properties rented by the tenant
        Properties = await _propertyService.LoadUserPropertyViewsAsync(tenantId, _userStateService.Role);
        foreach (var property in Properties)
        {
            if (!_propertyImages.ContainsKey(property.PropertyId))
            {
                _propertyImages[property.PropertyId] = await _propertyService.GetPropertyImageAsync(property.PropertyId);
            }
        }

        // Load unresolved issues
        var issues = await _issueService.GetUnresolvedIssuesByTenantIdAsync(tenantId, _userStateService.Role);
        UnresolvedIssues = issues
            .OrderByDescending(i => i.ReportDate)
            .Take(2)
            .ToList();

        // Load monthly payments for all rentals associated with the tenant
        Payments = (await _rentalService.GetMonthlyPaymentsAsync(tenantId, _userStateService.Role))
        .Where(p => p.isPaid) // Filtrovanie iba zaplatených platieb
        .OrderByDescending(p => p.PaymentDate) // Usporiadanie podľa najnovšieho dátumu
        .Take(2) // Výber iba dvoch najnovších
        .ToList();
        TotalMonthlyRent = Payments.Sum(p => p.TotalAmount);

        // Find upcoming payment
        if (UserRole == "Landlord")
        {
            UpcomingPayment = Payments
                .Where(p => p.PaymentDate > DateTime.Now && !p.isPaid)
                .OrderBy(p => p.PaymentDate)
                .FirstOrDefault();
        } else
        {
            UpcomingPayment = Payments
            .Where(p => p.PaymentDate > DateTime.Now && !p.isPaid)
            .OrderBy(p => p.PaymentDate)
            .FirstOrDefault();
        }
        
        // Check if all rents for the current month are paid
        var currentMonth = DateTime.Now;
        AreAllRentsPaid = await _rentalService.AreAllRentsPaidForTenantAsync(tenantId, currentMonth);

        // Calculate resolved issues count
        ResolvedIssuesCount = await _issueService.GetResolvedIssuesCountByTenantIdAsync(tenantId);

        // Generate notifications
        Notifications = GenerateNotifications();
    }

    public string GetPropertyImage(int propertyId)
    {
        if (_propertyImages.ContainsKey(propertyId))
        {
            return _propertyImages[propertyId];
        }
        return "/img/default-placeholder-property.jpg";
    }

    public string GetPropertyName(int propertyId)
    {
        return Properties.FirstOrDefault(p => p.PropertyId == propertyId).PropertyName;
    }

    public async Task<string> GetIssueUserName(int propertyId)
    {
        if(_userStateService.Role == "Landlord")
        {
            return await _propertyService.GetTenantNameByProperty(propertyId);
        }
        else
        {
            return await _propertyService.GetTenantNameByProperty(propertyId);
        }
        
    }
    public async Task<string> GetIssueUserAvatar(int propertyId)
    {
        //TODO
        return "";

    }



    private List<string> GenerateNotifications()
    {
        var notifications = new List<string>();

        if (UpcomingPayment != null && !UpcomingPayment.isPaid)
        {
            notifications.Add($"Upcoming payment due on {UpcomingPayment.PaymentDate?.ToString("dd/MM/yyyy")}: {UpcomingPayment.TotalAmount} €");
        }

        if (UnresolvedIssues.Any())
        {
            notifications.Add($"You have {UnresolvedIssues.Count} unresolved issues.");
        }

        return notifications;
    }

    private async Task<string> LoadTenantNameAsync(string tenantId)
    {
        // Example logic for retrieving tenant's name
        // Replace with actual logic based on your setup
        return $"Tenant {tenantId}";
    }
}
