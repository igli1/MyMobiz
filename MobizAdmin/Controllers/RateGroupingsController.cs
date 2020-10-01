using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobizAdmin.Data;

namespace MobizAdmin.Controllers
{
    public class RateGroupingsController : Controller
    {
        private readonly mymobiztestContext _context;
        public RateGroupingsController( mymobiztestContext context)
        {
            _context = context;
        }
        [Authorize] //returns rateGroupings list
        public IActionResult RateGroupings()
        {
            return View(_context.Rategroupings.ToList());
        }
        [Authorize] // returns create rateGroupings view
        public IActionResult CreateRateGrouping()
        {
            return View();
        }
        [HttpPost]
        [Authorize] // creates a new rateGrouping
        public async Task<IActionResult> CreateRateGrouping(Rategroupings rategroupings)
        {
            await _context.AddAsync(rategroupings);
            await _context.SaveChangesAsync();
            return RedirectToAction("RateGroupings");
        }
        [Authorize] //returns rateCategories list filtered by grouping
        public IActionResult RateCategories(string id)
        {
            ViewData["RateGrouping"] = id;
            return View(_context.Ratecategories.Where(e=>e.RateGrouping==id).ToList());
        }
    }
}