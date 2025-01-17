﻿namespace PropertyWebApp.Models.Services
{
    using global::PropertyWebApp.Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

    namespace PropertyWebApp.Services
    {
        public class IssueService
        {
            private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
            private readonly UserStateService _userStateService;

            public IssueService(IDbContextFactory<AppDbContext> dbContextFactory, UserStateService userStateService)
            {
                _dbContextFactory = dbContextFactory;
                _userStateService = userStateService;
            }
            public async Task<List<Issue>> GetIssuesAsync(string id)
            {

                using var dbContext = _dbContextFactory.CreateDbContext();
                var issues = await dbContext.Issues
                    .Include(i => i.Images)
                    .Include(i => i.Status)
                    .Include(i => i.Property)
                    .Include(i => i.Rental)
                    .Include(i => i.TaggedIssues)
                    
                        .ThenInclude(t => t.Tag)
                    .AsNoTracking()
                    .ToListAsync();
                if(_userStateService.Role == "Landlord")
                {
                    return issues.Where(i => i.Property.PropertyOwnerId == id).ToList();
                } else
                {
                    return issues.Where(i => i.Rental.TenantId == id).ToList();
                }
                
            }

            public async Task<Issue> GetIssueByIdAsync(int issueId)
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                return await dbContext.Issues
                    .Include(i => i.Images)
                    .Include(i => i.Status)
                    .Include(i => i.TaggedIssues)
                        .ThenInclude(t => t.Tag)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.IssueId == issueId);
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

                // validacia
                await ValidateIssueAsync(issue);

                if (issue.IssueId == 0) 
                {
                    await dbContext.Issues.AddAsync(issue);
                }
                else 
                {
                    dbContext.Issues.Update(issue);
                }

                await dbContext.SaveChangesAsync();
            }
            

            //validacia poruchy
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

            public async Task<List<Issue>> GetUnresolvedIssuesByTenantIdAsync(string userId, string role)
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                var query = dbContext.Issues
                .Include(i => i.Property)
                .Include(i => i.Rental)
                .Include(i => i.Status)
                .Where(i => i.Status.StatusName != "Vyriešené"); 
                

                if (role == "Tenant")
    {
                    query = query.Where(i => i.Rental.TenantId == userId);
                }
                 else if (role == "Landlord")
                {
                    query = query.Where(i => i.Property.PropertyOwner.Id == userId);
                }

                return await query
                .ToListAsync();

            }

            internal async Task<int> GetResolvedIssuesCountByTenantIdAsync(string tenantId)
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                var issues = await dbContext.Issues
                    .Include(i => i.Property) 
                    .Where(i =>
                        i.Rental.TenantId == tenantId && 
                        i.Status.StatusName == "Vyriešené") 
                    .OrderByDescending(i => i.ReportDate) 
                    .ToListAsync();
                return issues.Count;
            }
        }
    }

}
