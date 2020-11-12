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
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

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
            Quotes quote = new Quotes(_context); //Creating a new Quote
            quote.RefererId = _context.Webreferers.AsNoTracking().FirstOrDefault(e => e.ServiceId == dtCalculateQuote.ServiceID).Id;
            quote.ServiceId = service.Id;
            DTCalculate calculate= CalculatePriceAsync(dtCalculateQuote, service); //calculating price
            quote.Price = calculate.Price;
            quote.VerNum = calculate.VerNum;
            if (quote.Price == null || quote.Price == 0)
                return BadRequest();
            //var Id = await _context.Database.ExecuteSqlRawAsync("INSERT INTO `mymobiztest`.`quotes`(`ServiceID`,`RefererID`)VALUES('" + quote.ServiceId + "', " + quote.RefererId + ");SELECT max(id) as 'id' from quotes; ");

            /*MySqlConnection myConnection = _context.Database.GetDbConnection() as MySqlConnection;
            var mySelectQuery = "INSERT INTO `mymobiztest`.`quotes`(`ServiceID`,`RefererID`)VALUES('" + quote.ServiceId + "', " + quote.RefererId + ");SELECT max(id) as 'id' from quotes; ";
            MySqlCommand myCommand = new MySqlCommand(mySelectQuery, myConnection);
            myConnection.Open();
            MySqlDataReader myReader;
            myReader = myCommand.ExecuteReader();
            try
            {
                while (myReader.Read())
                {
                    quote.Id = myReader.GetString(0);
                }
            }
            finally
            {
                myReader.Close();
                myConnection.Close();
            }*/
            dynamic query = _context.Quotes.FromSqlRaw("INSERT INTO `mymobiztest`.`quotes`(`ID`,`ServiceID`,`RefererID`, `VerNum`)VALUES(QuotesNextId(),'" + quote.ServiceId + "', " + quote.RefererId + ", " + quote.VerNum + ");SELECT * FROM quotes WHERE quotes.ID=(SELECT MAX(id) FROM quotes)");
            quote.Id = Enumerable.FirstOrDefault<dynamic>(query).Id;
            /*using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "INSERT INTO `mymobiztest`.`quotes`(`ServiceID`,`RefererID`)VALUES('" + quote.ServiceId + "', " + quote.RefererId + ");SELECT max(id) as 'id' from quotes; ";
                _context.Database.OpenConnection();
                var result = command.ExecuteReader();
                MySqlDataReader reader= result as MySqlDataReader;
                while (reader.Read())
                {
                    System.Diagnostics.Debug.WriteLine(result);
                    quote.Id = Convert.ToString(result);
                }
            } */
            //await _context.Quotes.AddAsync(quote); // Adding Quote to Context
            //await _context.SaveChangesAsync(); // Saving Changes
            //Check if there are no Places for MobizAdmin Simulate.
            if (dtCalculateQuote.Places !=null && dtCalculateQuote.Places.Count >1)
            {
                _queue.QueueTask(async token =>   // Initiating Background Service
                {
                    await StoreGeoAsync(dtCalculateQuote, token, quote.Id);
                });
            }  
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
                Rides rides = new Rides(context);


                rides.QuoteId = quoteId;
                var query = context.Rides.FromSqlRaw("INSERT INTO `mymobiztest`.`rides`(`ID`,`QuoteID`)VALUES(RidesNextId(),'" + rides.QuoteId + "');SELECT * FROM rides WHERE rides.ID=(SELECT MAX(id) FROM rides) ");
                rides.Id = Enumerable.FirstOrDefault<dynamic>(query).Id;
                /*MySqlConnection myConnection = _context.Database.GetDbConnection() as MySqlConnection;
                var mySelectQuery = "INSERT INTO `mymobiztest`.`rides`(`QuoteId`)VALUES('" + rides.QuoteId + "');SELECT * FROM rides WHERE rides.ID=(SELECT MAX(id) FROM rides) ";
                MySqlCommand myCommand = new MySqlCommand(mySelectQuery, myConnection);
                myConnection.Open();
                MySqlDataReader myReader;
                myReader = myCommand.ExecuteReader();
                try
                {
                    while (myReader.Read())
                    {
                        rides.Id = myReader.GetString(0);
                    }
                }
                finally
                {
                    myReader.Close();
                    myConnection.Close();
                }*/

                //rides.Id = rides.NextId(); //Generating Rides Next Id;
                //rides.Id = "2020R000001";

                //var Id = await _context.Database.ExecuteSqlRawAsync("INSERT INTO `mymobiztest`.`rides`(`QuoteId`)VALUES('" + rides.QuoteId + "');SELECT max(id) as 'id' from rides; ");
                //rides.Id = Convert.ToString( Id);
                //await context.Rides.AddAsync(rides); //Inserting Rides to Context
                // Saving legs and rides to database

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
        private DTCalculate CalculatePriceAsync(DTCalculateQuote dtCalculateQuote, Services service)
        {
            decimal price;
            // If a service doesn't have categories only default rate
            if (dtCalculateQuote.Categories ==null || dtCalculateQuote.Categories.Count == 0)
            {
                _logger.LogInfo("No categories selected");
                var rates = service.Servicerates.Where(sr => sr.Locked == false).Where(sr => sr.AppDate <= DateTime.Today).OrderByDescending(sr => sr.AppDate).Select(sr => new
                {
                    VerNum=sr.VerNum,
                    EurKm = sr.EurKm,
                    EurMinDrive = sr.EurMinDrive,
                    EurMinimum = sr.EurMinimum,
                    EurMinWait = sr.EurMinWait
                }).FirstOrDefault();
                price = ((rates.EurKm * dtCalculateQuote.Kms) + (rates.EurMinDrive * dtCalculateQuote.DriveTime) +
                    (rates.EurMinWait * dtCalculateQuote.WaitTime));
                if (price < rates.EurMinimum && rates.EurMinimum != null)
                    price = rates.EurMinimum ?? 0;
                
                return (new DTCalculate() { VerNum = rates.VerNum, Price = price });
            }
            //If a service has categories and rate details
            else
            {
                //Getting Service Rate for selected service filtering by date. 
                //Getting Rate Details and Rate Targets for selected Rate Category.
                var rates = service.Servicerates.Where(sr => sr.Locked == false).Where(sr => sr.AppDate <= DateTime.Today).OrderByDescending(sr => sr.AppDate).Select(sr => new
                {
                    VerNum = sr.VerNum,
                    EurKm = sr.EurKm,
                    EurMinDrive = sr.EurMinDrive,
                    EurMinimum = sr.EurMinimum,
                    EurMinWait = sr.EurMinWait,
                    Lexo = sr.Lexo,
                    Ratedetails = sr.Ratedetails.Where(rd => service.Ratecategories.Any(rc => dtCalculateQuote.Categories.Any(dtc => (dtc.Option==null || dtc.Option == true) &&  dtc.Category == rc.Lexo) && rc.Id == rd.CategoryId)).Select(rd => new
                    {
                        Ratetargets = rd.Ratetargets.ToArray()
                    }).ToArray()
                }).FirstOrDefault();
                dtCalculateQuote.VerNum = rates.VerNum;
                Servicerates calculatedRate = new Servicerates();
                List<Target> totalPrice = new List<Target>();
                //new calculation method
                for (int i = 0; i < rates.Ratedetails.Length; i++)
                {
                    for (int j = 0; j < rates.Ratedetails[i].Ratetargets.Length; j++)
                    {
                        if (rates.Ratedetails[i].Ratetargets[j].RateTarget != "TotalPrice" && rates.Ratedetails[i].Ratetargets[j].RateTarget != "EurMile" && rates.Ratedetails[i].Ratetargets[j].RateTarget != "MaxPax")
                        {
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
}