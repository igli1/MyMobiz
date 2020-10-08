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

namespace MyMobiz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly mymobiztestContext _context;
        private IBackgroundQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        public QuotesController(mymobiztestContext context, IBackgroundQueue queue, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _context = context;
            _queue = queue;
        }
        // Accepting Post request from Route 'Api/Quotes/calculate'
        // Creating and returning a new Quote
        // Initiating the Background Service
        [HttpPost]
        [Route("calculate")]
        public async Task<ActionResult<DTCalculateQuote>> CalculateQuoteTasksAsync(DTCalculateQuote dtCalculateQuote)
        {
            
            Quotes quote = new Quotes(_context); //Creating a new Quote
            quote.Id = quote.NextId(); //Generating a new Quotes ID from Quotes.NextID();
            quote.RefererId = _context.Webreferers.AsNoTracking().FirstOrDefault(e => e.ServiceId == dtCalculateQuote.ServiceID).Id;
            quote.ServiceId = dtCalculateQuote.ServiceID;
            // should become optional in database
            quote.Price = await CalculatePriceAsync(dtCalculateQuote); //calculating price
            if (quote.Price == null || quote.Price == 0)
                return BadRequest();
            await _context.Quotes.AddAsync(quote); // Adding Quote to Context
            await _context.SaveChangesAsync(); // Saving Changes
            _queue.QueueTask(async token =>   // Initiating Background Service
            {
                await StoreGeoAsync(dtCalculateQuote, token, quote.Id);

            });
            return Ok(new DTQuote() { Id = quote.Id, Price = quote.Price });
        }
        //Background Service
        //Inserting to Database Places, Legs, Rides and RidesLegs
        public async Task StoreGeoAsync(DTCalculateQuote dtCalculateQuote, CancellationToken ct, string quoteId)
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
                //Inserting Rides to context
                Rides rides = new Rides(context);
                rides.Id = rides.NextId(); //Generating Rides Next Id;
                rides.QuoteId = quoteId;
                await context.Rides.AddAsync(rides); //Inserting Rides to Context
                // Saving legs and rides to database
                await context.SaveChangesAsync();

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
        public async Task<decimal> CalculatePriceAsync(DTCalculateQuote dtCalculateQuote)
        {
            //Getting service rates the newest
            Servicerates rates = await _context.Servicerates.Where(s => s.AppDate <= DateTime.Today && s.ServiceId == dtCalculateQuote.ServiceID && s.Locked==false).OrderByDescending(s => s.AppDate).AsNoTracking().FirstOrDefaultAsync();
            decimal price;
            // If a service doesn't have categories only default rate
            if (dtCalculateQuote.Categories == null)
            {
                price = ((rates.EurKm * dtCalculateQuote.Kms) + (rates.EurMinDrive * dtCalculateQuote.DriveTime) +
                    (rates.EurMinWait * dtCalculateQuote.WaitTime));
                if (price < rates.EurMinimum && rates.EurMinimum != null)
                    price = rates.EurMinimum ?? 0;
                return price;
            }
            //If a service has categories and rate details
            else
            {
                Servicerates calculatedRate = new Servicerates();
                List<Target> totalPrice = new List<Target>();
                // loop through categories
                List<Ratecategories> categories = new List<Ratecategories>();
                for(int i = 0; i < dtCalculateQuote.Categories.Count; i++)
                {
                    if(dtCalculateQuote.Categories[i].Option==null || dtCalculateQuote.Categories[i].Option == true)
                    {
                        categories.Add(await _context.Ratecategories.AsNoTracking().FirstOrDefaultAsync(e => e.Lexo == dtCalculateQuote.Categories[i].Category ));
                    }
                }
                for (int i=0;i<categories.Count;i++)
                {
                    foreach (Ratedetails detail in await _context.Ratedetails.Where(e => e.Vernum == rates.VerNum && e.CategoryId==categories[i].Id).AsNoTracking().ToListAsync())
                    {
                        foreach (Ratetargets target in await _context.Ratetargets.Where(e => e.RateDetailId == detail.Id).AsNoTracking().ToListAsync())
                        {
                            // checking if target is not total price bcs total price gets calculated in the end
                            if (target.RateTarget != "TotalPrice" && target.RateTarget != "EurMile")
                            {
                                //getting target property values
                                PropertyInfo newRateProperty = calculatedRate.GetType().GetProperty(target.RateTarget);
                                decimal ratesValue = (decimal)((rates.GetType().GetProperty(target.RateTarget).GetValue(rates, null)));
                                decimal previousValue = 0;
                                if (calculatedRate.GetType().GetProperty(target.RateTarget).GetValue(calculatedRate, null) != null)
                                {
                                    previousValue = (decimal)((calculatedRate.GetType().GetProperty(target.RateTarget).GetValue(calculatedRate, null)));
                                }
                                //switching betwen operator cases
                                //Adding values to target property
                                switch (target.RateOperator)
                                {
                                    case "+":
                                        newRateProperty.SetValue(calculatedRate, previousValue + (ratesValue + target.RateFigure));
                                        break;
                                    case "*":
                                        newRateProperty.SetValue(calculatedRate, previousValue + (ratesValue * target.RateFigure));
                                        break;
                                    case "=":
                                        newRateProperty.SetValue(calculatedRate, previousValue + target.RateFigure);
                                        break;
                                    case "%":
                                        newRateProperty.SetValue(calculatedRate, previousValue + (((target.RateFigure / 100) * ratesValue) + ratesValue));
                                        break;
                                }
                            }
                            else if (target.RateTarget == "TotalPrice")
                            {
                                totalPrice.Add(new Target() { Figure = target.RateFigure, op = target.RateOperator });
                            }
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
                return price;
            }
        }
    }
}