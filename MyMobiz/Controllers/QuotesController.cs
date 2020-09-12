using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMobiz.Models;
using Newtonsoft.Json;
using MyMobiz.BackgroundServices;
using MyMobiz.NextIDs;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MyMobiz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly mymobiztestContext _context;
        private readonly QuoteNextId _quoteNextId;
        private IBackgroundQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        public QuotesController(mymobiztestContext context, IBackgroundQueue queue, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _context = context;
            _queue = queue;
            _quoteNextId = new QuoteNextId(_context);
        }
        // GET: api/Quotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quotes>> GetQuotes(string id)
        {
            var quotes = await _context.Quotes.FindAsync(id);

            if (quotes == null)
            {
                return NotFound();
            }

            return quotes;
        }
        //Calculating Qoutes
        /*[HttpPost]
        [Route("calculate")]
        public async Task<ActionResult<DTCalculateQuote>> DTCalculateQuote(DTCalculateQuote dTCalculateQuote)
        {

            //Check http Referer   
            if(_context.Referers.Any(e=>e.Referer== Request.Headers["Referer"].ToString()))
            {

            }
            //Check Services ID and ApiKey
            if (_context.Services.Any(e => e.Id == dTCalculateQuote.ServiceID && e.ApiKey== dTCalculateQuote.ServiceKey))
            {
                //Deserialize places and legs JSON
                /*var places = JsonConvert.DeserializeObject<List<Places>>(dTCalculateQuote.Places);
                var Legs = JsonConvert.DeserializeObject<List<Legs>>(dTCalculateQuote.Legs);
                List<Places> place = new List<Places>();
                List<Legs> legs = new List<Legs>();
                //Inserting Places to database Format: Departure, Wp1, Wp2, Destination
                for (int i=0; i < places.Count; i++)
                {
                    place.Add(places[i]);
                    await _context.Places.AddAsync(place[i]);
                }
                await _context.SaveChangesAsync();
                //Inserting Legs to database Format: Departure->Wp1, Wp1->Wp2, Wp2->Destination
                for (int i = 0; i < Legs.Count; i++)
                {
                    legs.Add(Legs[i]);
                    legs[i].FromPlaceId = place[i].Id;
                   legs[i].ToPlaceId = place[i+1].Id;
                    await _context.Legs.AddAsync(legs[i]);
                }
                //Inserting Rides to database
                Rides rides = new Rides();
                //rides.Id = RidesNextID(); //Getting Rides ID from RidesNextID();
                await _context.Rides.AddAsync(rides);
                await _context.SaveChangesAsync();
                //Inserting Quotes to database
                Quotes quotes = new Quotes();
               // quotes.Id = QuoteNextID();   //Getting Quotes ID from QuotesNextID();
                quotes.RefererId = 5;
                quotes.ServiceId = dTCalculateQuote.ServiceID;
                quotes.RideId = rides.Id;
                
                //Getting VerNum
                Servicerates serviceRates = await _context.Servicerates.FirstOrDefaultAsync(e => e.ServiceId == dTCalculateQuote.ServiceID);
                quotes.VerNum = serviceRates.VerNum;
                //Calculating Price
                quotes.Price = (serviceRates.EurKm * dTCalculateQuote.Kms) + (serviceRates.EurWaitMin * dTCalculateQuote.DriveTime);
                await _context.Quotes.AddAsync(quotes);
                //Inserting RidesLegs to database
                List<Rideslegs> ridesLegsList = new List<Rideslegs>();
                Rideslegs rideslLegs = new Rideslegs();
                for (int i = 0; i < dTCalculateQuote.legs.Count; i++)
                {
                    ridesLegsList.Add(rideslLegs);
                    ridesLegsList[i].LegId = dTCalculateQuote.legs[i].Id;
                    ridesLegsList[i].RideId = rides.Id;
                    ridesLegsList[i].Seqnr = i + 1;
                    await _context.Rideslegs.AddAsync(ridesLegsList[i]);
                }       
                await _context.SaveChangesAsync();
                
                //Returning calculated Quote
                return CreatedAtAction("GetQuotes", new { id = quotes.Id }, quotes);
            }
            return StatusCode(StatusCodes.Status401Unauthorized);        
        }*/
        
        [HttpPost]
        [Route ("c")]
        public async Task<ActionResult<DTCalculateQuote>> CalculateQuoteAsync([FromBody] string data)
        {
            var dtCalculateQuote = JsonConvert.DeserializeObject<DTCalculateQuote>(data);
            Quotes quote = new Quotes();
            quote.Id = _quoteNextId.NextId();
            quote.ServiceId = dtCalculateQuote.ServiceID;
            quote.RideId = "2020R000023";
            quote.RefererId = 5;
            // Not implemented
            // quote.RideJson= data;
            await _context.Quotes.AddAsync(quote);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetQuotes", new { id = quote.Id }, quote);
        }
        // Accepting Post request from Route 'Api/Quotes/ca'
        // Creating and returning a new Quote
        // Initiating the Background Service
        [HttpPost]
        [Route("ca")]
        public async Task<ActionResult<DTCalculateQuote>> CalculateQuoteTasksAsync(DTCalculateQuote dtCalculateQuote)
        {
            //var result=await QuoteAsync(dtCalculateQuote.ServiceID, 5);
            Quotes quote = new Quotes(); //Creating a new Quote
            quote.Id = _quoteNextId.NextId(); //Generating a new Quotes ID from QuotesNextID();
            quote.RefererId = 5;
            quote.ServiceId = dtCalculateQuote.ServiceID;
            quote.RideId = "2020R000023";
            await _context.Quotes.AddAsync(quote); // Adding Quote to COntext
            await _context.SaveChangesAsync(); // Saving Changes
            _queue.QueueTask(async token =>   // Initiating Background Service
            {
                await Calculate(dtCalculateQuote, token);
                
            });
            return CreatedAtAction("GetQuotes", new { id = quote.Id }, quote); //Returning the new Quote
        }
        //Background Service
        //Inserting to Database Places, Legs, Rides and RidesLegs
        public  async Task Calculate(DTCalculateQuote dtCalculateQuote, CancellationToken ct)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                // Creating a new DB Context
                var context = scope.ServiceProvider.GetRequiredService<mymobiztestContext>();
                // Inserting Places to DB and Sorting them
                     int Order = 0;
                List<Places> orderedPlaces = new List<Places>();
                     for(int i = 0; i < dtCalculateQuote.places.Count; i++)
                {
                         for(int j=0; j < dtCalculateQuote.places.Count; j++)
                    {
                        if (dtCalculateQuote.places[j].JsId == "bd-address-from" && Order == 0)
                        {
                            orderedPlaces.Add(new Places()); //Adding Departure to Collection
                            orderedPlaces[Order].Address = dtCalculateQuote.places[j].Address;
                            orderedPlaces[Order].Lat = dtCalculateQuote.places[j].Lat;
                            orderedPlaces[Order].Lng = dtCalculateQuote.places[j].Lng; 
                            await context.Places.AddAsync(orderedPlaces[Order]); //Inserting Places to Context
                            Order = 1;
                        }
                        else if( dtCalculateQuote.places[j].JsId == "bd-address-via" + Order && Order <= dtCalculateQuote.places.Count - 2){
                            orderedPlaces.Add(new Places()); //Adding Waypoints to Collection
                            orderedPlaces[Order].Address = dtCalculateQuote.places[j].Address;
                            orderedPlaces[Order].Lat = dtCalculateQuote.places[j].Lat;
                            orderedPlaces[Order].Lng = dtCalculateQuote.places[j].Lng;            
                            await context.Places.AddAsync(orderedPlaces[Order]); //Inserting Places to Context
                            Order += 1;
                        }
                        else if (Order == dtCalculateQuote.places.Count - 1 && dtCalculateQuote.places[j].JsId == "bd-address-to")
                        {
                            orderedPlaces.Add(new Places()); //Adding Destination to Collection
                            orderedPlaces[Order].Address = dtCalculateQuote.places[j].Address;
                            orderedPlaces[Order].Lat = dtCalculateQuote.places[j].Lat;
                            orderedPlaces[Order].Lng = dtCalculateQuote.places[j].Lng;
                            await context.Places.AddAsync(orderedPlaces[Order]); //Inserting Places to Context
                            j = dtCalculateQuote.places.Count;
                            i = dtCalculateQuote.places.Count;
                        }
                    }
                }
                await context.SaveChangesAsync(); //Saving DB Changes
                /*for (int i = 0; i < dtCalculateQuote.legs.Count; i++)
                {
                    System.Diagnostics.Debug.WriteLine(dtCalculateQuote.legs[i].fromGEO.JsId);
                    System.Diagnostics.Debug.WriteLine(dtCalculateQuote.legs[i].toGEO.JsId);
                }*/
                //Inserting Legs to database. Format: Departure->Wp1, Wp1->Wp2, Wp2->Destination
                
                    List<Legs> legs = new List<Legs>();
                    for (int i = 0; i < dtCalculateQuote.legs.Count; i++)
                    {
                        legs.Add(new Legs()); //Adding a new Leg to Collection
                        legs[i].FromPlaceId = orderedPlaces[i].Id;                      
                        legs[i].ToPlaceId = orderedPlaces[i + 1].Id;                      
                        legs[i].Fare = dtCalculateQuote.legs[i].Fare;                       
                        legs[i].Kms = dtCalculateQuote.legs[i].Kms;                      
                        legs[i].MinutesDrive = dtCalculateQuote.legs[i].MinutesDrive;                       
                        legs[i].MinutesWithTraffic = dtCalculateQuote.legs[i].MinutesWithTraffic;
                        await context.Legs.AddAsync(legs[i]); //Adding Leg to Context
                    }
                RidesNextID _ridesNextID=new RidesNextID(context);
                //Inserting Rides to database
                Rides rides = new Rides();
                rides.Id = _ridesNextID.NextId(); //Generating a new Rides ID from RidesNextID();
                await context.Rides.AddAsync(rides); //Inserting Rides to Context
                await context.SaveChangesAsync(); //Saving DB Changes

                //Inserting RidesLegs to database
                List<Rideslegs> ridesLegs = new List<Rideslegs>();
                for (int i = 0; i < legs.Count; i++)
                {
                    ridesLegs.Add(new Rideslegs()); //Adding a new RideLeg to Collection
                    ridesLegs[i].LegId = legs[i].Id;
                    ridesLegs[i].RideId = rides.Id;
                    ridesLegs[i].Seqnr = i + 1;
                    await context.Rideslegs.AddAsync(ridesLegs[i]);
                }
                await context.SaveChangesAsync(ct);
            }
        }
    }
}
