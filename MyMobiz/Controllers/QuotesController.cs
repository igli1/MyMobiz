using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMobiz.Models;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

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
        /*[HttpPost("calculate")]
        [Obsolete]
        public ActionResult<CalculatedQuote> CalculateQuote(Places departure, Places destination)
        {

            var sqlCommand = "INSERT INTO places (address, lat, lng) VALUES(@Address, @Lat, @Lng)";
            //departure
            var adressParam = new MySqlParameter("@Address", departure.Address);
            var latParam = new MySqlParameter("@Lat", departure.Lat);
            var lngParam = new MySqlParameter("@Lng", departure.Lng);
            _context.Database.ExecuteSqlCommand(sqlCommand, adressParam, latParam, lngParam);
            //destionation
            var adressParam2 = new MySqlParameter("@Address", destination.Address);
            var latParam2 = new MySqlParameter("@Lat", destination.Lat);
            var lngParam2 = new MySqlParameter("@Lng", destination.Lng);
            _context.Database.ExecuteSqlCommand(sqlCommand, adressParam2, latParam2, lngParam2);


            return Ok(null);
        }*/
        [HttpPost("calculate")]
        [Obsolete]
        public ActionResult<CalculatedQuote> CalculateQuote(CalculatedQuote calculatedQuote)
        {

            var placesSql = "INSERT INTO places (address, lat, lng) VALUES(@Address, @Lat, @Lng)";
            //departure
            var adressParam = new MySqlParameter("@Address", calculatedQuote.departure.Address);
            var latParam = new MySqlParameter("@Lat", calculatedQuote.departure.Lat);
            var lngParam = new MySqlParameter("@Lng", calculatedQuote.departure.Lng);
            _context.Database.ExecuteSqlCommand(placesSql, adressParam, latParam, lngParam);
            //destionation
            var adressParam2 = new MySqlParameter("@Address", calculatedQuote.destination.Address);
            var latParam2 = new MySqlParameter("@Lat", calculatedQuote.destination.Lat);
            var lngParam2 = new MySqlParameter("@Lng", calculatedQuote.destination.Lng);
            _context.Database.ExecuteSqlCommand(placesSql, adressParam2, latParam2, lngParam2);

            //legs
            var legsSql = "INSERT INTO legs(FromPlaceID, ToPlaceID) VALUES(LAST_INSERT_ID()-1, LAST_INSERT_ID())";
            _context.Database.ExecuteSqlCommand(legsSql);

            //rides
            var ridesSql = "INSERT INTO rides(ID) VALUES(@ID)";
            var ridesIDParam2 = new MySqlParameter("@ID", calculatedQuote.rides.Id);
            _context.Database.ExecuteSqlCommand(ridesSql);

            //ridesLegs
            /*var ridesSql = "INSERT INTO rides(ID) VALUES(@ID)";
            var ridesIDParam2 = new MySqlParameter("@ID", calculatedQuote.rides.Id);
            _context.Database.ExecuteSqlCommand(ridesSql);*/

            return Ok(null);
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
    }
}
