using BookGenerator.Client.ApiServices;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookGenerator.Client.BackgroundWorkers
{
    /// <summary>
    /// Calls the backend web api to wake it up on startup
    /// </summary>
    public class StartupWorker : IHostedService
    {
        private readonly IBookApiService bookApi;

        public StartupWorker(IBookApiService bookApi)
        {
            this.bookApi = bookApi ?? throw new System.ArgumentNullException(nameof(bookApi));
        }

        /// <summary>
        /// Calls the backend web api to wake it up on startup
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            bookApi.GetStatusAsync(Guid.Empty); // A backgound task, don't await
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
