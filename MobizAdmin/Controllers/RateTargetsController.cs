using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobizAdmin.Data;

namespace MobizAdmin.Controllers
{
    public class RateTargetsController : Controller
    {
        private readonly mymobiztestContext _context;
        public RateTargetsController(mymobiztestContext context)
        {
            _context = context;
        }
        [Authorize] // returns rateTargets list
        public IActionResult RateTargets()
        {
            return View(_context.Ratetargets.ToList());
        }
        [Authorize] //returns create rateTarget view
        public IActionResult CreateRateTarget()
        {
            return View();
        }
        [HttpPost]
        [Authorize] // creates rateTarget
        public async Task<IActionResult> CreateRateTarget(Ratetargets ratetargets)
        {
            await _context.AddAsync(ratetargets);
            await _context.SaveChangesAsync();
            return RedirectToAction("RateTargets");
        }
    }
}