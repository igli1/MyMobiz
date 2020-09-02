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
    public class ServiceratesController : ControllerBase
    {
        private readonly mymobiztestContext _context;

        public ServiceratesController(mymobiztestContext context)
        {
            _context = context;
        }

        // GET: api/Servicerates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servicerates>>> GetServicerates()
        {
            return await _context.Servicerates.ToListAsync();
        }

        // GET: api/Servicerates/5
        [HttpGet("{vernum}")]
        public async Task<ActionResult<Servicerates>> GetServicerates(int vernum)
        {
            var servicerates = await _context.Servicerates.FindAsync(vernum);

            if (servicerates == null)
            {
                return NotFound();
            }

            return servicerates;
        }

        // PUT: api/Servicerates/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{vernum}")]
        public async Task<IActionResult> PutServicerates(int vernum, Servicerates servicerates)
        {
            if (vernum != servicerates.VerNum)
            {
                return BadRequest();
            }

            _context.Entry(servicerates).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceratesExists(vernum))
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

        // POST: api/Servicerates
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Servicerates>> PostServicerates(Servicerates servicerates)
        {
            _context.Servicerates.Add(servicerates);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ServiceratesExists(servicerates.VerNum))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetServicerates", new { vernum = servicerates.VerNum }, servicerates);
        }

        // DELETE: api/Servicerates/5
        [HttpDelete("{vernum}")]
        public async Task<ActionResult<Servicerates>> DeleteServicerates(string vernum)
        {
            var servicerates = await _context.Servicerates.FindAsync(vernum);
            if (servicerates == null)
            {
                return NotFound();
            }

            _context.Servicerates.Remove(servicerates);
            await _context.SaveChangesAsync();

            return servicerates;
        }

        private bool ServiceratesExists(int vernum)
        {
            return _context.Servicerates.Any(e => e.VerNum == vernum);
        }
    }
}
