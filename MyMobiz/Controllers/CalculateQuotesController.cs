using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMobiz.Models;

namespace MyMobiz.Controllers
{
    /*//[Route("api/quotes/calculate")]
    [ApiController]
    public class CalculateQuotesController : ControllerBase
    {
        private readonly mymobiztestContext _context;
        public CalculateQuotesController(mymobiztestContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<Quotes>> PostCalculateQuotes(CalculatedQuote calculateQuotes)
        {
            _context.Places.Add(calculateQuotes.places);
            _context.Legs.Add(calculateQuotes.legs);
            _context.Rides.Add(calculateQuotes.rides);
            _context.Rideslegs.Add(calculateQuotes.ridesLegs);
            _context.Quotes.Add(calculateQuotes.quotes);
            await _context.SaveChangesAsync();

            return Ok(null);
        }
        private bool PlacesExists(string address)
        {
            return _context.Places.Any(e => e.Address == address);
        }
    }*/
}