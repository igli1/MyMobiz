using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyMobiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MyMobiz.ServicesCache;
namespace MyMobiz.iHostedService
{
    //IHostedService runs in parallel with NetCore Services.
    public class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private IMemoryCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILoggerManager _logger;
        private ServicesUpdate _servicesUpdate;
        public TimedHostedService(IMemoryCache cache, IServiceScopeFactory scopeFactory, ILoggerManager logger)
        {
            _cache = cache;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            DoWork(null);
            //Timer Interval from Now to Midnight, calls DoWork() function.
            //Executes once at Startup and every Day at Midnight.
            _timer = new Timer(DoWork, null,
            GetNextMidnight().Subtract(DateTime.Now), TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }
        public void DoWork(object state)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                //Gets Context from ScopeFactory.
                var _context= scope.ServiceProvider.GetRequiredService<mymobiztestContext>();
                _servicesUpdate= new ServicesUpdate(_context, _cache, _logger);
                _servicesUpdate.UpdateServicesCache();
                _servicesUpdate.UpdateNofQuotes();    
            }
        }
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
        private static DateTime GetNextMidnight()
        {
            //Today = This Day at 00:00:00. Today.AddDAys(1) = Tomorrow at 00:00:00
            return DateTime.Today.AddDays(1);
        }
    }
}