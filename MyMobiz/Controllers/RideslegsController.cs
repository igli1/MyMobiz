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
    public class RideslegsController : ControllerBase
    {
        private readonly mymobiztestContext _context;

        public RideslegsController(mymobiztestContext context)
        {
            _context = context;
        }

        // GET: api/Rideslegs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rideslegs>>> GetRideslegs()
        {
            return await _context.Rideslegs.ToListAsync();
        }

        // GET: api/Rideslegs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rideslegs>> GetRideslegs(int id)
        {
            var rideslegs = await _context.Rideslegs.FindAsync(id);

            if (rideslegs == null)
            {
                return NotFound();
            }

            return rideslegs;
        }

        // PUT: api/Rideslegs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRideslegs(int id, Rideslegs rideslegs)
        {
            if (id != rideslegs.Id)
            {
                return BadRequest();
            }

            _context.Entry(rideslegs).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RideslegsExists(id))
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

        // POST: api/Rideslegs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Rideslegs>> PostRideslegs(Rideslegs rideslegs)
        {
            _context.Rideslegs.Add(rideslegs);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRideslegs", new { id = rideslegs.Id }, rideslegs);
        }

        // DELETE: api/Rideslegs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rideslegs>> DeleteRideslegs(int id)
        {
            var rideslegs = await _context.Rideslegs.FindAsync(id);
            if (rideslegs == null)
            {
                return NotFound();
            }

            _context.Rideslegs.Remove(rideslegs);
            await _context.SaveChangesAsync();

            return rideslegs;
        }

        private bool RideslegsExists(int id)
        {
            return _context.Rideslegs.Any(e => e.Id == id);
        }
    }
}
