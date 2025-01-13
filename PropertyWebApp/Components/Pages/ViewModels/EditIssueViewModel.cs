namespace PropertyWebApp.Components.Pages.ViewModels
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.EntityFrameworkCore;
    using PropertyWebApp.Data;
    using PropertyWebApp.Models;
    using PropertyWebApp.Models.Services;
    using System.ComponentModel.DataAnnotations;

    public class EditIssueViewModel
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly PropertyService _propertyService;
        private readonly NavigationManager _navigationManager;

        public Issue NewIssue { get; set; } = new Issue();
        public List<IBrowserFile> UploadedFiles { get; set; } = new List<IBrowserFile>();
        public string? FileUploadError { get; set; }
        public List<Property> AvailableProperties { get; set; } = new();
        public Property? SelectedProperty { get; set; }
        public bool IsDropdownOpen { get; set; } = false;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

        public List<Tag> AvailableTags { get; set; } = new();
        public List<Tag> SelectedTags { get; set; } = new();

        public EditIssueViewModel(IDbContextFactory<AppDbContext> dbContextFactory, PropertyService propertyService, NavigationManager navigationManager)
        {
            _dbContextFactory = dbContextFactory;
            _propertyService = propertyService;
            _navigationManager = navigationManager;
        }

        public async Task InitializeAsync(int? issueId)
        {
            AvailableProperties = await _propertyService.LoadMyProperties();
            AvailableProperties = AvailableProperties.Where(p => p.Rentals.Any()).ToList();

            using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            // Load Available Tags
            AvailableTags = await dbContext.Tags.ToListAsync();

            if (issueId.HasValue)
            {
                var existingIssue = await dbContext.Issues
                    .Include(i => i.Images)
                    .Include(i => i.TaggedIssues)
                        .ThenInclude(ti => ti.Tag)
                        .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.IssueId == issueId.Value);

                if (existingIssue != null)
                {
                    NewIssue = existingIssue;
                    SelectedTags = existingIssue.TaggedIssues.Select(ti => ti.Tag).ToList();
                }
            }
            else
            {
                NewIssue = new Issue
                {
                    RentalId = 1,
                    PropertyId = 1,
                    ReportDate = DateTime.Now,
                    StatusId = (await dbContext.IssueStatus.FirstOrDefaultAsync(s => s.StatusName == "Rieši sa"))?.StatusId ?? 1,
                    Description = string.Empty
                };
            }
        }

        public void ToggleDropdown()
        {
            IsDropdownOpen = !IsDropdownOpen;
        }

        public void SelectProperty(Property property)
        {
            SelectedProperty = property;
            NewIssue.RentalId = property.Rentals.FirstOrDefault()?.PropertyId ?? 0;
            IsDropdownOpen = false;
        }

        public async Task HandleValidSubmitAsync()
{
    using var dbContext = await _dbContextFactory.CreateDbContextAsync();

    if (!await ValidateIssueAsync(NewIssue, dbContext))
    {
        return;
    }

    if (NewIssue.IssueId > 0)
    {
        dbContext.Issues.Update(NewIssue);
    }
    else
    {
        await dbContext.Issues.AddAsync(NewIssue);
    }

    // Uloženie zmien pre získanie IssueId
    await dbContext.SaveChangesAsync();

    // Upload obrázkov
    string uploadsFolder = Path.Combine("wwwroot", "uploads");
    if (!Directory.Exists(uploadsFolder))
    {
        Directory.CreateDirectory(uploadsFolder);
    }

    foreach (var file in UploadedFiles)
    {
        try
        {
            var filePath = Path.Combine(uploadsFolder, file.Name);
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream(MaxFileSize).CopyToAsync(fileStream);

            var issueImage = new IssueImage
            {
                IssueId = NewIssue.IssueId,
                ImagePath = $"/uploads/{file.Name}"
            };

            await dbContext.IssueImages.AddAsync(issueImage);
        }
        catch (Exception ex)
        {
            FileUploadError = $"Nahrávanie súboru {file.Name} zlyhalo: {ex.Message}";
        }
    }

    // Spracovanie tagov
    var existingTaggedIssues = await dbContext.TaggedIssues
        .Where(ti => ti.IssueId == NewIssue.IssueId)
        .ToListAsync();

    // Odstránenie existujúcich tagov
    dbContext.TaggedIssues.RemoveRange(existingTaggedIssues);

    // Pridanie vybraných tagov
    foreach (var tag in SelectedTags)
    {
        if (!existingTaggedIssues.Any(ti => ti.TagId == tag.TagId))
        {
            dbContext.TaggedIssues.Add(new TaggedIssue
            {
                IssueId = NewIssue.IssueId,
                TagId = tag.TagId
            });
        }
    }

    // Uloženie zmien
    await dbContext.SaveChangesAsync();

    // Navigácia späť
    _navigationManager.NavigateTo("/issues-screen");
}


        public async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            FileUploadError = null;
            var files = e.GetMultipleFiles();

            foreach (var file in files)
            {
                if (file.Size > MaxFileSize)
                {
                    FileUploadError = $"Súbor {file.Name} prekračuje maximálnu veľkosť {MaxFileSize / 1024 / 1024} MB.";
                    continue;
                }

                UploadedFiles.Add(file);
            }
        }

        private async Task<bool> ValidateIssueAsync(Issue issue, AppDbContext dbContext)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(issue);

            bool isValid = Validator.TryValidateObject(issue, validationContext, validationResults, true);

            if (!isValid)
            {
                FileUploadError = string.Join("\n", validationResults.Select(vr => vr.ErrorMessage));
                return false;
            }

            var rentalExists = await dbContext.Rentals.AnyAsync(r => r.RentalId == issue.RentalId);
            if (!rentalExists)
            {
                FileUploadError = "Zadané ID nájmu neexistuje.";
                return false;
            }

            var propertyExists = await dbContext.Properties.AnyAsync(p => p.PropertyId == issue.PropertyId);
            if (!propertyExists)
            {
                FileUploadError = "Zadané ID nehnuteľnosti neexistuje.";
                return false;
            }

            return true;
        }

        public void ToggleTag(Tag tag)
        {
            if (SelectedTags.Contains(tag))
            {
                SelectedTags.Remove(tag);
            }
            else
            {
                SelectedTags.Add(tag);
            }
        }
    }
}
