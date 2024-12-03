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
        }
    }

}
