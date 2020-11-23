using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobizAdmin.Data;
using Newtonsoft.Json;

namespace MobizAdmin.Controllers
{
    public class ServicesController : Controller
    {
        private readonly mymobiztestContext _context;

        public ServicesController(mymobiztestContext context)
        {
            _context = context;
        }
        [Authorize] //return list of services
        public IActionResult ServicesList()
        {
            return View(_context.Services.ToList());
        }
        [Authorize] //return create service view
        public IActionResult CreateService()
        {
            return View();
        }
        [HttpPost]
        [Authorize] //creates a new service
        public async Task<IActionResult> CreateServiceAsync(Services service)
        {
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return RedirectToAction("ServicesList");
        }
        [Authorize] //return service details
        public IActionResult ServiceDetails(string ServiceId)
        {
            return View(_context.Services.Find(ServiceId));
        }
        [Authorize] //returns edit service view
        public IActionResult EditService(string ServiceId)
        {
            return View(_context.Services.Find(ServiceId));
        }
        [HttpPost]
        [Authorize] //Edits the service
        public async Task<IActionResult> EditServiceAsync(Services services)
        {
            _context.Entry(services).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("ServiceDetails", new { ServiceId = services.Id});
        }
        [Authorize] //return list of service rates
        public IActionResult ServiceRates(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(_context.Servicerates.Where(e => e.ServiceId == ServiceId).ToList());
        }
        [Authorize] //return create service rate view
        public IActionResult CreateServiceRate(string ServiceId, string ServiceName)
        {
            ViewData["ServiceId"] = ServiceId;
            ViewData["ServiceName"] = ServiceName;
            return View();
        }
        [HttpPost]
        [Authorize] //creates new serviceRate
        public async Task<IActionResult> CreateServiceRateAsync(Servicerates serviceRates, string ServiceName)
        {
            
            await _context.Servicerates.AddAsync(serviceRates);
            await _context.SaveChangesAsync();
            return RedirectToAction("ServiceRates", new { ServiceId = serviceRates.ServiceId, ServiceName = ServiceName });
        }
    }
}