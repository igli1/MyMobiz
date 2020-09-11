using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMobiz.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Text;
using MyMobiz.BackgroundServices;
using MyMobiz.NextIDs;
using MyMobiz.Models.DTOs;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

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
        [HttpPost]
        [Route("calculate")]
        public async Task<ActionResult<DTCalculateQuote>> DTCalculateQuote(DTCalculateQuote dTCalculateQuote)
        {

            //Check http Referer   
            /*if(_context.Referers.Any(e=>e.Referer== Request.Headers["Referer"].ToString()))
            {

            }*/
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
                }*/
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
        }
        
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
        [HttpPost]
        [Route("ca")]
        public async Task<ActionResult<DTCalculateQuote>> CalculateQuoteTasksAsync(DTCalculateQuote dtCalculateQuote)
        {
            //var result=await QuoteAsync(dtCalculateQuote.ServiceID, 5);
            Quotes quote = new Quotes();
            quote.Id = _quoteNextId.NextId();
            quote.RefererId = 5;
            quote.ServiceId = dtCalculateQuote.ServiceID;
            quote.RideId = "2020R000023";
            await _context.Quotes.AddAsync(quote);
            await _context.SaveChangesAsync();
            _queue.QueueTask(async token =>
            {
                await Calculate(dtCalculateQuote, token);
                
            });
            return CreatedAtAction("GetQuotes", new { id = quote.Id }, quote);
        }
        public  async Task <bool>Calculate(DTCalculateQuote dtCalculateQuote, CancellationToken ct)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<mymobiztestContext>();
                try
                {
                    List<Places> places = new List<Places>();
                    Places p = new Places();
                    for (int i = 0; i < dtCalculateQuote.places.Count; i++)
                    {
                        p.Address = dtCalculateQuote.places[i].Address;
                        p.Lat = dtCalculateQuote.places[i].Lat;
                        p.Lng = dtCalculateQuote.places[i].Lng;
                        places.Add(p);
                        System.Diagnostics.Debug.WriteLine("Address: " + places[i].Address);
                        
                    }
                    places.ForEach(n => context.Places.Add(n));
                    // Inserting Places to DB and Sorting them
                    int departure = 0;
                    int waypointOrder = 1;
                    System.Diagnostics.Debug.WriteLine("Places count: "+places.Count);
                    /* for (int i = 0; i < places.Count; i++)
                     {
                         if (dtCalculateQuote.places[i].JsId == "bd-address-from" && departure == 0)
                         {
                             departure = 1;
                             await context.Places.AddAsync(places[i]);
                             System.Diagnostics.Debug.WriteLine("Departure Added");
                         }
                         else if (departure == 1)
                         {
                             for (int j = 0; j < places.Count; j++)
                             {
                                 if (dtCalculateQuote.places[j].JsId == "bd-address-via" + waypointOrder && waypointOrder <= dtCalculateQuote.places.Count - 2)
                                 {
                                     System.Diagnostics.Debug.WriteLine("waypoints Added" + waypointOrder);
                                     waypointOrder += 1;
                                     await context.Places.AddAsync(places[i]);

                                 }
                                 else if (waypointOrder <= dtCalculateQuote.places.Count - 1 && dtCalculateQuote.places[i].JsId == "bd-address-to")
                                 {
                                     System.Diagnostics.Debug.WriteLine("Destination Added");
                                     await context.Places.AddAsync(places[i]);
                                 }
                             }
                         }
                     }*/
                    
                    await context.SaveChangesAsync(ct);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("igli" + e.Message);
                }
                return true;
            }
        }
    }
}
