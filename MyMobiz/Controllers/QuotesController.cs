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

namespace MyMobiz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly mymobiztestContext _context;

        public QuotesController(mymobiztestContext context)
        {
            _context = context;
        }

        // GET: api/Quotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quotes>>> GetQuotes()
        {
            return await _context.Quotes.ToListAsync();
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


        // PUT: api/Quotes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuotes(string id, Quotes quotes)
        {
            if (id != quotes.Id)
            {
                return BadRequest();
            }

            _context.Entry(quotes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuotesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Quotes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Quotes>> PostQuotes(Quotes quotes)
        {
            _context.Quotes.Add(quotes);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (QuotesExists(quotes.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetQuotes", new { id = quotes.Id }, quotes);
        }
        //Calculating Qoutes
        [HttpPost("calculate")]
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
                var places = JsonConvert.DeserializeObject<List<Places>>(dTCalculateQuote.Places);
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
                rides.Id = RidesNextID(); //Getting Rides ID from RidesNextID();
                await _context.Rides.AddAsync(rides);
                await _context.SaveChangesAsync();
                //Inserting Quotes to database
                Quotes quotes = new Quotes();
                quotes.Id = QuoteNextID();   //Getting Quotes ID from QuotesNextID();
                quotes.RefererId = 5;
                quotes.ServiceId = dTCalculateQuote.ServiceID;
                quotes.RideId = rides.Id;
                quotes.Price = 500;
                await _context.Quotes.AddAsync(quotes);
                //Inserting RidesLegs to database
                List<Rideslegs> ridesLegsList = new List<Rideslegs>();
                Rideslegs rideslLegs = new Rideslegs();
                for (int i = 0; i < legs.Count; i++)
                {
                    ridesLegsList.Add(rideslLegs);
                    ridesLegsList[i].LegId = legs[i].Id;
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
        // DELETE: api/Quotes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Quotes>> DeleteQuotes(string id)
        {
            var quotes = await _context.Quotes.FindAsync(id);
            if (quotes == null)
            {
                return NotFound();
            }

            _context.Quotes.Remove(quotes);
            await _context.SaveChangesAsync();

            return quotes;
        }

        private bool QuotesExists(string id)
        {
            return _context.Quotes.Any(e => e.Id == id);
        }
        //Generating Quote ID
        private string QuoteNextID()
        {
            string year = DateTime.Parse(DateTime.Now.ToString()).Year.ToString();
            string maxValue = _context.Quotes.Max(e => e.Id);
            if (maxValue != null)
            {
                
                string[] value = maxValue.Split('Q');
                if (year == value[0])
                {
                    int Id;
                    string output = "1";
                    try
                    {
                        Id = (Convert.ToInt32(value[1]) + 1);
                        if (Id <= 999999)
                            output = String.Format("{0}{1}{2:D6}", year, "Q", Id);
                        else
                            return null;
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                    return output;
                }
                else
                    return year + "Q000001";
            }
                
            return year+"Q000001";
        }
        //Generating Rides ID
        private string RidesNextID()
        {
            string year = DateTime.Parse(DateTime.Now.ToString()).Year.ToString();
            string maxValue = _context.Rides.Max(e => e.Id);
            if (maxValue != null)
            {
                string[] value = maxValue.Split('R');
                if (year == value[0])
                {
                    int Id;
                    string output = "1";
                    try
                    {
                        Id = (Convert.ToInt32(value[1]) + 1);
                        if (Id <= 999999)
                            output = String.Format("{0}{1}{2:D6}", year, "R", Id);
                        else
                            return null;
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                    return output;
                }
                else
                    return year + "R000001";
            }
                    
            return year + "R000001";
        }
    }
}
