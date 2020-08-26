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
    [Route("api/[controller]")]
    [ApiController]
    public class RidesController : ControllerBase
    {
        private readonly mymobiztestContext _context;

        public RidesController(mymobiztestContext context)
        {
            _context = context;
        }

        // GET: api/Rides
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rides>>> GetRides()
        {
            return await _context.Rides.ToListAsync();
        }

        // GET: api/Rides/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rides>> GetRides(string id)
        {
            var rides = await _context.Rides.FindAsync(id);

            if (rides == null)
            {
                return NotFound();
            }

            return rides;
        }

        // PUT: api/Rides/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRides(string id, Rides rides)
        {
            if (id != rides.Id)
            {
                return BadRequest();
            }

            _context.Entry(rides).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RidesExists(id))
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

        // POST: api/Rides
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Rides>> PostRides(Rides rides)
        {
            _context.Rides.Add(rides);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RidesExists(rides.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRides", new { id = rides.Id }, rides);
        }

        // DELETE: api/Rides/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rides>> DeleteRides(string id)
        {
            var rides = await _context.Rides.FindAsync(id);
            if (rides == null)
            {
                return NotFound();
            }

            _context.Rides.Remove(rides);
            await _context.SaveChangesAsync();

            return rides;
        }

        private bool RidesExists(string id)
        {
            return _context.Rides.Any(e => e.Id == id);
        }
    }
}
