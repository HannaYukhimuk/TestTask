using Library.UI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class OverdueBookNotifier : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueBookNotifier> _logger;

    public OverdueBookNotifier(IServiceScopeFactory scopeFactory, ILogger<OverdueBookNotifier> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

                    var overdueLoans = await context.UserBooks
                        .Include(bl => bl.Book)   
                        .Where(bl => bl.ReturnBy < DateTime.UtcNow && bl.ReturnedAt == null)
                        .ToListAsync(stoppingToken);

                    foreach (var loan in overdueLoans)
                    {
                        if (loan.Book == null)
                        {
                            _logger.LogError($"Error: LoanId {loan.Id} has no associated book.");
                            continue;
                        }

                        _logger.LogWarning($"Book '{loan.Book.Title}' is expired by {loan.UserId}");


                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking expired books");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); 
        }
    }
}
