using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SoClover.Server.Context;

namespace SoClover.Server.Services
{
    public class DatabaseCleanupService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseCleanupService> _logger;

        public DatabaseCleanupService(IServiceProvider serviceProvider, ILogger<DatabaseCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application is shutting down. Running database cleanup procedure...");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SoCloverDBContext>();
                await context.Database.ExecuteSqlRawAsync("EXEC deleteDatas", cancellationToken);

                _logger.LogInformation("Database cleanup procedure 'deleteDatas' executed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the database cleanup procedure.");
            }
        }
    }
}