using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using MyMobiz.Models;
namespace MyMobiz.BackgroundServices
{
    public class QueueService : BackgroundService
    {
        private IBackgroundQueue _queue;

        public QueueService(IBackgroundQueue queue)
        {
            _queue = queue;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _queue.PopQueue(stoppingToken);
                await task(stoppingToken);
            }
        }
    }
}