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

        [HttpPost("calculate")]
        public async Task<ActionResult<DTCalculateQuote>> DTCalculateQuote(DTCalculateQuote dTCalculateQuote)
        {
            //  [FromBody]
            //    dynamic jsonRequest
            // var model = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(jsonRequest));
            //var places = JsonConvert.DeserializeObject<List<Placess>>(jsonRequest["Places"].ToString());
            
            /*if(_context.Referers.Any(e=>e.Referer== Request.Headers["Referer"].ToString()))
            {

            }*/
            if (_context.Services.Any(e => e.Id == dTCalculateQuote.ServiceID && e.ApiKey== dTCalculateQuote.ServiceKey))
            {
                var places = JsonConvert.DeserializeObject<List<Placess>>(dTCalculateQuote.Places);
                var Legs = JsonConvert.DeserializeObject<List<Legs>>(dTCalculateQuote.Legs);
                
                CalculatedQuote calculateQuote = new CalculatedQuote();
                calculateQuote.departure = places[0];
                calculateQuote.destination = places[1];
                await _context.Places.AddAsync(calculateQuote.departure);
                await _context.Places.AddAsync(calculateQuote.destination);
                await _context.SaveChangesAsync();
                calculateQuote.legs = Legs[0];
                calculateQuote.legs.FromPlaceId = calculateQuote.departure.Id;
                calculateQuote.legs.ToPlaceId = calculateQuote.destination.Id;
                await _context.Legs.AddAsync(calculateQuote.legs);
                Rides rides = new Rides();
                rides.Id = RidesNextID();
                await _context.Rides.AddAsync(rides);
                await _context.SaveChangesAsync();    
                Quotes quotes = new Quotes();
                quotes.Id = QuoteNextID();   
                quotes.RefererId = 1;
                quotes.ServiceId = dTCalculateQuote.ServiceID;
               // quotes.VerNum = calculateQuote.quotes.VerNum;
                quotes.RideId = rides.Id;
                /*calculateQuote.quotes.ServiceId = quotes.ServiceId;
                calculateQuote.quotes.RideId = calculateQuote.rides.Id;
                calculateQuote.quotes.RefererId = 1;
                calculateQuote.quotes.Id = quotes.Id;*/
                await _context.Quotes.AddAsync(quotes);
                Rideslegs ridesLegs = new Rideslegs();
                ridesLegs.LegId = calculateQuote.legs.Id;
                ridesLegs.RideId = rides.Id;
                await _context.Rideslegs.AddAsync(ridesLegs);
                await _context.SaveChangesAsync();
                
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
