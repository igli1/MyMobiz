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
    public class ReferersController : ControllerBase
    {
        private readonly mymobiztestContext _context;

        public ReferersController(mymobiztestContext context)
        {
            _context = context;
        }

        // GET: api/Referers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Referers>>> GetReferers()
        {
            return await _context.Referers.ToListAsync();
        }

        // GET: api/Referers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Referers>> GetReferers(int id)
        {
            var referers = await _context.Referers.FindAsync(id);

            if (referers == null)
            {
                return NotFound();
            }

            return referers;
        }

        // PUT: api/Referers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReferers(int id, Referers referers)
        {
            if (id != referers.Id)
            {
                return BadRequest();
            }

            _context.Entry(referers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReferersExists(id))
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

        // POST: api/Referers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Referers>> PostReferers(Referers referers)
        {
            _context.Referers.Add(referers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReferers", new { id = referers.Id }, referers);
        }

        // DELETE: api/Referers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Referers>> DeleteReferers(int id)
        {
            var referers = await _context.Referers.FindAsync(id);
            if (referers == null)
            {
                return NotFound();
            }

            _context.Referers.Remove(referers);
            await _context.SaveChangesAsync();

            return referers;
        }

        private bool ReferersExists(int id)
        {
            return _context.Referers.Any(e => e.Id == id);
        }
    }
}
