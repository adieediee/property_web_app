using Microsoft.EntityFrameworkCore;
using PropertyWebApp.Data;
using PropertyWebApp.Models;

public class RentalService
{
    private readonly AppDbContext _dbContext;

    public RentalService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Rental>> GetAllRentals()
    {
        var rentals = await _dbContext.Rentals
            .Include(r => r.Property)
            .ToListAsync();
        return rentals;
    }

    // Retrieve all monthly payments for a tenant
    public async Task<List<MonthlyPayment>> GetMonthlyPaymentsAsync(string tenantId)
    {
        return await _dbContext.MonthlyPayments
            .Include(mp => mp.Rental)
            .Where(mp => mp.Rental.TenantId == tenantId)
            .ToListAsync();
    }

    // Check if all rents are paid for the current month
    public async Task<bool> AreAllRentsPaidForTenantAsync(string tenantId, DateTime currentMonth)
    {
        var rentals = await _dbContext.Rentals
            .Where(r => r.TenantId == tenantId)
            .ToListAsync();

        foreach (var rental in rentals)
        {
            var isPaid = await _dbContext.MonthlyPayments
                .AnyAsync(mp => mp.RentalId == rental.RentalId &&
                                mp.PaymentDate.HasValue &&
                                mp.PaymentDate.Value.Month == currentMonth.Month &&
                                mp.PaymentDate.Value.Year == currentMonth.Year);

            if (!isPaid)
            {
                return false; // If any rental is unpaid, return false
            }
        }

        return true; // All rents are paid
    }
}
