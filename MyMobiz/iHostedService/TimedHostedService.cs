﻿using LoggerService;
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

namespace MyMobiz.iHostedService
{
    //IHostedService runs in parallel with NetCore Services.
    public class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private IMemoryCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILoggerManager _logger;
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
                _logger.LogInfo("Updating cache");
                //Gets Context from ScopeFactory.
                var _context= scope.ServiceProvider.GetRequiredService<mymobiztestContext>();
                //Update NofQuotes in ServiceRates
                _context.Database.ExecuteSqlRaw("update servicerates set nquotes= (select count('vernum') from quotes where quotes.vernum = servicerates.vernum);");
                //Checks if Services exist in Cache and Removes them.
                if (_cache.Get("Services")!=null)
                _cache.Remove("Services");
                //Gets all Services with their respective WebReferers, RateCategories, Servicerates, RatesDetails and Rate Targets.
                var services = _context.Services.Where(sv => sv.Tsd > DateTime.Now || sv.Tsd == null).Select(s => new
                {
                    Id = s.Id,
                    ApiKey = s.ApiKey,
                    ServiceName = s.ServiceName,
                    Webreferers = s.Webreferers,
                    Ratecategories = s.Ratecategories,
                    Servicerates = s.Servicerates.Where(sr=>sr.Tsd > DateTime.Now || sr.Tsd == null).Select(sr=>new
                    {
                        VerNum=sr.VerNum,
                        EurKm=sr.EurKm,
                        EurMinDrive=sr.EurMinDrive,
                        EurMinimum= sr.EurMinimum,
                        EurMinWait=sr.EurMinWait,
                        MaxPax=sr.MaxPax,
                        AppDate=sr.AppDate,
                        EndDate=sr.EndDate,
                        Ratedetails = sr.Ratedetails.Where(rd => rd.Tsd > DateTime.Now || rd.Tsd == null).Select(rd=>new
                        {
                            Id=rd.Id,
                            CategoryId=rd.CategoryId,
                            Ratetargets = rd.Ratetargets.Where(rt => rt.Tsd > DateTime.Now || rt.Tsd == null)
                        })
                    }),
                }).ToList();
                //Serializes 'Undefined type': 'services' and Deserializes them to a 'List': of 'Services'.
                List <Services> s = JsonSerializer.Deserialize<List<Services>>(JsonSerializer.Serialize(services));
                //Adds List of Services to Cache
                _cache.Set("Services", s);
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