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
    public class PlacesController : ControllerBase
    {
        private readonly mymobiztestContext _context;

        public PlacesController(mymobiztestContext context)
        {
            _context = context;
        }

        // GET: api/Places
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Placess>>> GetPlaces()
        {
            return await _context.Places.ToListAsync();
        }

        // GET: api/Places/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Placess>> GetPlaces(int id)
        {
            var places = await _context.Places.FindAsync(id);

            if (places == null)
            {
                return NotFound();
            }

            return places;
        }

        // PUT: api/Places/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaces(int id, Placess places)
        {
            if (id != places.Id)
            {
                return BadRequest();
            }

            _context.Entry(places).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlacesExists(id))
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

        // POST: api/Places
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Placess>> PostPlaces(Placess places)
        {
            _context.Places.Add(places);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlaces", new { id = places.Id }, places);
        }

        // DELETE: api/Places/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Placess>> DeletePlaces(int id)
        {
            var places = await _context.Places.FindAsync(id);
            if (places == null)
            {
                return NotFound();
            }

            _context.Places.Remove(places);
            await _context.SaveChangesAsync();

            return places;
        }

        private bool PlacesExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }
    }
}
