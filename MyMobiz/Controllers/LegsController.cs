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
    public class LegsController : ControllerBase
    {
        private readonly mymobiztestContext _context;

        public LegsController(mymobiztestContext context)
        {
            _context = context;
        }

        // GET: api/Legs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Legs>>> GetLegs()
        {
            return await _context.Legs.ToListAsync();
        }

        // GET: api/Legs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Legs>> GetLegs(int id)
        {
            var legs = await _context.Legs.FindAsync(id);

            if (legs == null)
            {
                return NotFound();
            }

            return legs;
        }

        // PUT: api/Legs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLegs(int id, Legs legs)
        {
            if (id != legs.Id)
            {
                return BadRequest();
            }

            _context.Entry(legs).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LegsExists(id))
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

        // POST: api/Legs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Legs>> PostLegs(Legs legs)
        {
            _context.Legs.Add(legs);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLegs", new { id = legs.Id }, legs);
        }

        // DELETE: api/Legs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Legs>> DeleteLegs(int id)
        {
            var legs = await _context.Legs.FindAsync(id);
            if (legs == null)
            {
                return NotFound();
            }

            _context.Legs.Remove(legs);
            await _context.SaveChangesAsync();

            return legs;
        }

        private bool LegsExists(int id)
        {
            return _context.Legs.Any(e => e.Id == id);
        }
    }
}
