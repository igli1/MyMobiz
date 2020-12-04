using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyMobiz.Models;
namespace MyMobiz.ServicesCache
{
    public class ServicesUpdate
    {
        private mymobiztestContext _context;
        private IMemoryCache _cache;
        private readonly ILoggerManager _logger;
        public ServicesUpdate(mymobiztestContext context, IMemoryCache cache, ILoggerManager logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }
        public void UpdateNofQuotes()
        {
            _logger.LogInfo("Updating number of quotes on Service Rates");
            //Update NofQuotes in ServiceRates
            _context.Database.ExecuteSqlRaw("update servicerates set nquotes= (select count('vernum') from quotes where quotes.vernum = servicerates.vernum);");
        }
        public void UpdateServicesCache()
        {
            _logger.LogInfo("Updating cache for all Services");
            //Checks if Services exist in Cache and Removes them.
            if (_cache.Get("Services") != null)
                _cache.Remove("Services");
            //Gets all Services with their respective WebReferers, RateCategories, Servicerates, RatesDetails and Rate Targets.
            var services = _context.Services.Where(sv => sv.Tsd > DateTime.Now || sv.Tsd == null).Select(s => new
            {
                Id = s.Id,
                ApiKey = s.ApiKey,
                ServiceName = s.ServiceName,
                Webreferers = s.Webreferers,
                Ratecategories = s.Ratecategories,
                Servicerates = s.Servicerates.Where(sr => sr.Tsd > DateTime.Now || sr.Tsd == null).Select(sr => new
                {
                    VerNum = sr.VerNum,
                    EurKm = sr.EurKm,
                    EurMinDrive = sr.EurMinDrive,
                    EurMinimum = sr.EurMinimum,
                    EurMinWait = sr.EurMinWait,
                    MaxPax = sr.MaxPax,
                    AppDate = sr.AppDate,
                    EndDate = sr.EndDate,
                    Ratedetails = sr.Ratedetails.Where(rd => rd.Tsd > DateTime.Now || rd.Tsd == null).Select(rd => new
                    {
                        Id = rd.Id,
                        CategoryId = rd.CategoryId,
                        Ratetargets = rd.Ratetargets.Where(rt => rt.Tsd > DateTime.Now || rt.Tsd == null)
                    })
                }),
            }).ToList();
            //Serializes 'Undefined type': 'services' and Deserializes them to a 'List': of 'Services'.
            List<Services> s = JsonSerializer.Deserialize<List<Services>>(JsonSerializer.Serialize(services));
            //Adds List of Services to Cache
            _cache.Set("Services", s);
        }
        public void UpdateServiceRateCache(string ServiceId, int Vernum)
        {
            _logger.LogInfo("Updating Cache for ServiceRate: "+Vernum.ToString());
            List<Services> services = _cache.Get<List<Services>>("Services");
            _cache.Remove("Services");
            services.FirstOrDefault(e => e.Id == ServiceId).Servicerates.Remove(services.FirstOrDefault(e => e.Id == ServiceId).Servicerates.FirstOrDefault(e => e.VerNum == Vernum));
            var rate = _context.Servicerates.Where(sr => sr.VerNum == Vernum && (sr.Tsd > DateTime.Now || sr.Tsd == null)).Select(sr => new
            {
                VerNum = sr.VerNum,
                EurKm = sr.EurKm,
                EurMinDrive = sr.EurMinDrive,
                EurMinimum = sr.EurMinimum,
                EurMinWait = sr.EurMinWait,
                MaxPax = sr.MaxPax,
                AppDate = sr.AppDate,
                EndDate = sr.EndDate,
                Ratedetails = sr.Ratedetails.Where(rd => rd.Tsd > DateTime.Now || rd.Tsd == null).Select(rd => new
                {
                    Id = rd.Id,
                    CategoryId = rd.CategoryId,
                    Ratetargets = rd.Ratetargets.Where(rt => rt.Tsd > DateTime.Now || rt.Tsd == null)
                })
            }).FirstOrDefault();
            services.FirstOrDefault(e => e.Id == ServiceId).Servicerates.Add(JsonSerializer.Deserialize<Servicerates>(JsonSerializer.Serialize(rate)));
            _cache.Set("Services", services);
        }
    }
}
