namespace PropertyWebApp.Models.Services
{
    using global::PropertyWebApp.Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;

    namespace PropertyWebApp.Services
    {
        public class IssueService
        {
            private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

            public IssueService(IDbContextFactory<AppDbContext> dbContextFactory)
            {
                _dbContextFactory = dbContextFactory;
            }
            public async Task<List<Issue>> GetIssuesAsync()
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                return await dbContext.Issues
                    .Include(i => i.Images)
                    .Include(i => i.Status)
                    .Include(i => i.TaggedIssues)
                        .ThenInclude(t => t.Tag)
                    .AsNoTracking()
                    .ToListAsync();
            }

            public async Task<bool> DeleteIssueAsync(int issueId)
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                using var transaction = await dbContext.Database.BeginTransactionAsync();

                try
                {
                    var issue = await dbContext.Issues
                        .Include(i => i.Images)
                        .Include(i => i.TaggedIssues)
                        .FirstOrDefaultAsync(i => i.IssueId == issueId);

                    if (issue == null) return false;

                    if (issue.Images != null)
                        dbContext.IssueImages.RemoveRange(issue.Images);

                    if (issue.TaggedIssues != null)
                        dbContext.TaggedIssues.RemoveRange(issue.TaggedIssues);

                    dbContext.Issues.Remove(issue);

                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            public async Task AddOrUpdateIssueAsync(Issue issue)
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                // Validácia
                await ValidateIssueAsync(issue);

                if (issue.IssueId == 0) // Nový záznam
                {
                    await dbContext.Issues.AddAsync(issue);
                }
                else // Aktualizácia existujúceho záznamu
                {
                    dbContext.Issues.Update(issue);
                }

                await dbContext.SaveChangesAsync();
            }
            // Validácia poruchy
            public async Task ValidateIssueAsync(Issue issue)
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                if (string.IsNullOrWhiteSpace(issue.Title))
                    throw new ArgumentException("Názov poruchy je povinný.");

                if (issue.Title.Length > 100)
                    throw new ArgumentException("Názov poruchy môže obsahovať maximálne 100 znakov.");

                if (string.IsNullOrWhiteSpace(issue.Description))
                    throw new ArgumentException("Popis poruchy je povinný.");

                if (issue.Description.Length > 1000)
                    throw new ArgumentException("Popis poruchy môže obsahovať maximálne 1000 znakov.");

                if (!await dbContext.Rentals.AnyAsync(r => r.RentalId == issue.RentalId))
                    throw new ArgumentException("Zadané ID nájmu neexistuje.");

                if (!await dbContext.Properties.AnyAsync(p => p.PropertyId == issue.PropertyId))
                    throw new ArgumentException("Zadané ID nehnuteľnosti neexistuje.");

                if (issue.SolvedDate.HasValue && issue.SolvedDate <= issue.ReportDate)
                    throw new ArgumentException("Dátum vyriešenia musí byť po dátume nahlásenia.");
            }
        }
    }

}
