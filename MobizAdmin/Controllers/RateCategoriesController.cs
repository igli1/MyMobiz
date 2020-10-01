using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MobizAdmin.Data;

namespace MobizAdmin.Controllers
{
    public class RateCategoriesController : Controller
    {
        private readonly mymobiztestContext _context;
        public RateCategoriesController(mymobiztestContext context)
        {
            _context = context;
        }
        [Authorize] //returns rateCategories list
        public IActionResult RateCategories()
        {
            return View(_context.Ratecategories.ToList());
        }
        [Authorize] //returns create rateCategories view
        public IActionResult CreateRateCategorie()
        {
            ViewBag.RateGrouping = new SelectList(_context.Rategroupings.ToDictionary(e => e.RateGrouping, e => e.RateGrouping), "Key", "Value");
            return View();
        }
        [HttpPost]
        [Authorize] //Create a new rateCategories
        public async Task<IActionResult> CreateRateCategorie(Ratecategories ratecategories)
        {
            await _context.AddAsync(ratecategories);
            await _context.SaveChangesAsync();
            return RedirectToAction("RateCategories");
        }
    }
}