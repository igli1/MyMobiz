using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyMobiz.Models;
using MyMobiz.Models.DTOs;
using MyMobiz.BackgroundServices;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System;
using MyMobiz.RatesTarget;
using Microsoft.Extensions.Caching.Memory;
using LoggerService;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;
using MyMobiz.iHostedService;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyMobiz.ServicesCache;

namespace MyMobiz.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly mymobiztestContext _context;
        private IBackgroundQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMemoryCache _cache;
        private ServicesUpdate _servicesUpdate;
        public QuotesController(mymobiztestContext context, IBackgroundQueue queue, IServiceScopeFactory scopeFactory, IMemoryCache cache, ILoggerManager logger)
        {
            _scopeFactory = scopeFactory;
            _context = context;
            _queue = queue;
            _cache = cache;
            _logger = logger;
        }
        // Accepting Post request from Route 'api/quotes/calculate'
        // Calculating the Price
        // Initiating the Background Service for insterting Places, Legs, Rides, RideLegs
        // Creating and returning a new Quote
        [HttpPost]
        [Route("calculate")]
        public ActionResult<DTCalculateQuote> CalculateQuoteTasksAsync(DTCalculateQuote dtCalculateQuote)
        {
            _logger.LogInfo("Calculating the quote");
            // Getting Service for Selected ServiceId
            Services service = _cache.Get<List<Services>>("Services").FirstOrDefault(e => e.Id == dtCalculateQuote.ServiceID);
            Quotes quote = new Quotes(); //Creating a new Quote
            quote.RefererId = service.Webreferers.FirstOrDefault().Id; // for testing purposes
            quote.ServiceId = service.Id;
            List<DTCategories> dtCategories = dtCalculateQuote.Categories;
            dynamic rates;
            if (dtCalculateQuote.Categories == null || dtCalculateQuote.Categories.Count == 0)
            {
                rates = service.Servicerates.Where(sr => sr.EndDate >= DateTime.Today).Where(sr => sr.AppDate <= DateTime.Today).OrderByDescending(sr => sr.AppDate).FirstOrDefault();
            }
            else
            {
                rates = service.Servicerates.Where(sr => sr.EndDate >= DateTime.Today).Where(sr => sr.AppDate <= DateTime.Today).OrderByDescending(sr => sr.AppDate).Select(sr => new
                {
                    VerNum = sr.VerNum,
                    EurKm = sr.EurKm,
                    EurMinDrive = sr.EurMinDrive,
                    EurMinimum = sr.EurMinimum,
                    EurMinWait = sr.EurMinWait,
                    Ratedetails = sr.Ratedetails.Where(rd => service.Ratecategories.Any(rc => dtCalculateQuote.Categories.Any(dtc => (dtc.Option == null || dtc.Option == true) && dtc.Category == rc.Lexo) && rc.Id == rd.CategoryId)).Select(rd => new
                    {
                        Ratetargets = rd.Ratetargets.ToArray()
                    }).ToArray()
                }).FirstOrDefault();
            }
            if(rates==null)
                return BadRequest();
            DTCalculate calculate = CalculatePriceAsync(dtCalculateQuote, rates); //calculating price
            quote.Price = calculate.Price;
            quote.VerNum = calculate.VerNum;
            if (quote.Price == null || quote.Price == 0)
                return BadRequest();
            dynamic query = _context.Quotes.FromSqlRaw("INSERT INTO `mymobiztest`.`quotes`(`ID`,`ServiceID`,`RefererID`, `VerNum`)VALUES(QuotesNextId(),'" + quote.ServiceId + "', " + quote.RefererId + ", " + quote.VerNum + ");SELECT * FROM quotes WHERE quotes.ID=(SELECT MAX(id) FROM quotes)");
            quote.Id = Enumerable.FirstOrDefault<dynamic>(query).Id;
            _queue.QueueTask(async token =>   // Initiating Background Service
            {
                await StoreGeoAsync(dtCalculateQuote, token, quote.Id);
            });
            return Ok(new DTQuote() { Id = quote.Id, Price = quote.Price });
        }
        //Background Service
        //Inserting to Database Places, Legs, Rides and RidesLegs
        private async Task StoreGeoAsync(DTCalculateQuote dtCalculateQuote, CancellationToken ct, string quoteId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                // Creating a new DB Context
                var context = scope.ServiceProvider.GetRequiredService<mymobiztestContext>();
                List<Places> orderedPlaces = new List<Places>();
                //Adding ordered places to collection and to context
                for (int i = 0; i < dtCalculateQuote.Journey.Count; i++)
                {
                    orderedPlaces.Add(dtCalculateQuote.Places.FirstOrDefault(e => e.Address == dtCalculateQuote.Journey[i].Word));
                }
                await context.Places.AddRangeAsync(orderedPlaces);
                //Saving places to database
                await context.SaveChangesAsync();
                //check if we have an extra leg
                if (dtCalculateQuote.Legs.Count == dtCalculateQuote.Places.Count)
                {
                    dtCalculateQuote.Legs.RemoveAt(0);
                }
                //Adding legs to context
                for (int i = 0; i < dtCalculateQuote.Legs.Count; i++)
                {
                    dtCalculateQuote.Legs[i].FromPlaceId = orderedPlaces[i].Id;
                    dtCalculateQuote.Legs[i].ToPlaceId = orderedPlaces[i + 1].Id;
                }
                //Adding Leg to Context
                await context.Legs.AddRangeAsync(dtCalculateQuote.Legs);
                await context.SaveChangesAsync();
                //Inserting Rides to context
                Rides rides = new Rides();
                rides.QuoteId = quoteId;
                var query = context.Rides.FromSqlRaw("INSERT INTO `mymobiztest`.`rides`(`ID`,`QuoteID`)VALUES(RidesNextId(),'" + rides.QuoteId + "');SELECT * FROM rides WHERE rides.ID=(SELECT MAX(id) FROM rides) ");
                rides.Id = Enumerable.FirstOrDefault<dynamic>(query).Id;
                //Inserting RidesLegs to context
                List<Ridelegs> ridesLegs = new List<Ridelegs>();
                for (int i = 0; i < dtCalculateQuote.Legs.Count; i++)
                {
                    ridesLegs.Add(new Ridelegs()); //Adding a new RideLeg to Collection
                    ridesLegs[i].LegId = dtCalculateQuote.Legs[i].Id;
                    ridesLegs[i].RideId = rides.Id;
                    ridesLegs[i].Seqnr = i + 1;
                }
                await context.Ridelegs.AddRangeAsync(ridesLegs);
                //Saving rides legs to database
                await context.SaveChangesAsync(ct);
            }
        }
        //Calculating the Price using Service Rates, Categories and Rates Details
        private DTCalculate CalculatePriceAsync(DTCalculateQuote dtCalculateQuote, dynamic rates)
        {
            decimal price;
            // If a service doesn't have categories only default rate
            if (dtCalculateQuote.Categories == null || dtCalculateQuote.Categories.Count == 0)
            {
                _logger.LogInfo("No categories selected");
                return CalculateWithoutTargets(dtCalculateQuote, rates);
            }
            //If a service has categories and rate details
            else
            {
                Servicerates calculatedRate = new Servicerates();
                List<Target> totalPrice = new List<Target>();
                //price calculation with rate targets
                int count = 0;
                for (int i = 0; i < rates.Ratedetails.Length; i++)
                {
                    for (int j = 0; j < rates.Ratedetails[i].Ratetargets.Length; j++)
                    {
                        if (rates.Ratedetails[i].Ratetargets[j].RateTarget != "TotalPrice" && rates.Ratedetails[i].Ratetargets[j].RateTarget != "EurMile" && rates.Ratedetails[i].Ratetargets[j].RateTarget != "MaxPax")
                        {
                            count++;
                            PropertyInfo newRateProperty = calculatedRate.GetType().GetProperty(rates.Ratedetails[i].Ratetargets[j].RateTarget);
                            decimal ratesValue = (decimal)((rates.GetType().GetProperty(rates.Ratedetails[i].Ratetargets[j].RateTarget).GetValue(rates, null)));
                            decimal previousValue = 0;
                            if (calculatedRate.GetType().GetProperty(rates.Ratedetails[i].Ratetargets[j].RateTarget).GetValue(calculatedRate, null) != null)
                            {
                                previousValue = (decimal)((calculatedRate.GetType().GetProperty(rates.Ratedetails[i].Ratetargets[j].RateTarget).GetValue(calculatedRate, null)));
                            }
                            //Switching betwen Operator cases
                            //Adding values to target property
                            switch (rates.Ratedetails[i].Ratetargets[j].RateOperator)
                            {
                                case "+":
                                    newRateProperty.SetValue(calculatedRate, previousValue + (ratesValue + rates.Ratedetails[i].Ratetargets[j].RateFigure));
                                    break;
                                case "*":
                                    newRateProperty.SetValue(calculatedRate, previousValue + (ratesValue * rates.Ratedetails[i].Ratetargets[j].RateFigure));
                                    break;
                                case "=":
                                    newRateProperty.SetValue(calculatedRate, previousValue + rates.Ratedetails[i].Ratetargets[j].RateFigure);
                                    break;
                                case "%":
                                    newRateProperty.SetValue(calculatedRate, previousValue + (((rates.Ratedetails[i].Ratetargets[j].RateFigure / 100) * ratesValue) + ratesValue));
                                    break;
                            }
                        }
                        else if (rates.Ratedetails[i].Ratetargets[j].RateTarget == "TotalPrice")
                        {
                            totalPrice.Add(new Target() { Figure = rates.Ratedetails[i].Ratetargets[j].RateFigure, op = rates.Ratedetails[i].Ratetargets[j].RateOperator });
                        }
                    }
                }
                //if it has rates details but no rate targets
                if(count == 0)
                {
                    return CalculateWithoutTargets(dtCalculateQuote, rates);
                }
                //if it hase rate targets
                else
                {
                    //calculating the price
                    price = ((calculatedRate.EurKm * dtCalculateQuote.Kms) + (calculatedRate.EurMinDrive * dtCalculateQuote.DriveTime) +
                        (calculatedRate.EurMinWait * dtCalculateQuote.WaitTime));
                    //looping for total price targets
                    for (int i = 0; i < totalPrice.Count; i++)
                    {
                        switch (totalPrice[i].op)
                        {
                            case "+":
                                price += totalPrice[i].Figure;
                                break;
                            case "*":
                                price *= totalPrice[i].Figure;
                                break;
                            case "%":
                                price += ((totalPrice[i].Figure / 100) * price);
                                break;
                            case "=":
                                price = totalPrice[i].Figure;
                                break;
                        }
                    }
                    if (price < rates.EurMinimum && rates.EurMinimum != null)
                        price = rates.EurMinimum ?? 0;
                    return new DTCalculate() { VerNum = rates.VerNum, Price = price };
                }
            }
        }
        // Accepting Post request from Route 'api/quotes/simulate'
        // Calculating the Price
        [HttpPost]
        [Route("simulate")]
        public ActionResult<DTCalculateQuote> SimulateQuote(DTCalculateQuote dtCalculateQuote)
        {
            Services service = _cache.Get<List<Services>>("Services").FirstOrDefault(e => e.Id == dtCalculateQuote.ServiceID);
            dynamic rates;
            if (dtCalculateQuote.Categories == null || dtCalculateQuote.Categories.Count == 0)
            {
                rates = service.Servicerates.FirstOrDefault(sr => sr.VerNum == dtCalculateQuote.VerNum);
            }
            else
            {
                rates = service.Servicerates.Where(sr => sr.VerNum == dtCalculateQuote.VerNum).Select(sr => new
                {
                    VerNum = sr.VerNum,
                    EurKm = sr.EurKm,
                    EurMinDrive = sr.EurMinDrive,
                    EurMinimum = sr.EurMinimum,
                    EurMinWait = sr.EurMinWait,
                    Lexo = sr.Lexo,
                    Ratedetails = sr.Ratedetails.Where(rd => service.Ratecategories.Any(rc => dtCalculateQuote.Categories.Any(dtc => (dtc.Option == null || dtc.Option == true) && dtc.Category == rc.Lexo) && rc.Id == rd.CategoryId)).Select(rd => new
                    {
                        Ratetargets = rd.Ratetargets.ToArray()
                    }).ToArray()
                }).FirstOrDefault();
            }
            if (rates == null)
                return BadRequest();
            return Ok(CalculatePriceAsync(dtCalculateQuote, rates));
        }
        [HttpPost]
        [Route("updaterate")]
        public ActionResult<dynamic> UpdateCacheForSinglerate(DTServiceRates rates)
        {
            _servicesUpdate = new ServicesUpdate(_context, _cache, _logger);
            _servicesUpdate.UpdateServiceRateCache(rates.ServiceId, rates.VerNum);
            return Ok("Cache Updated. Vernum: " + rates.VerNum);
        }
        [HttpPost]
        [Route("update")]
        public ActionResult<dynamic> UpdateCache(DTServiceRates rates)
        {
            _servicesUpdate = new ServicesUpdate(_context, _cache, _logger);
            _servicesUpdate.UpdateServicesCache();
            return Ok("Cache Updated");
        }
        private DTCalculate CalculateWithoutTargets(DTCalculateQuote dtCalculateQuote, dynamic rates)
        {
            decimal price;
            price = ((rates.EurKm * dtCalculateQuote.Kms) + (rates.EurMinDrive * dtCalculateQuote.DriveTime) +
                    (rates.EurMinWait * dtCalculateQuote.WaitTime));
            if (price < rates.EurMinimum && rates.EurMinimum != null)
                price = rates.EurMinimum ?? 0;

            return (new DTCalculate() { VerNum = rates.VerNum, Price = price });
        }
    }
}