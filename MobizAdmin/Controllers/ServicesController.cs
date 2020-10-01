using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobizAdmin.Data;
using MyMobiz.NextIDs;

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
        public IActionResult Services()
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
            ServiceNextId nextId = new ServiceNextId(_context);
            service.Id = nextId.NextId();
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return RedirectToAction("Services");
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
            //System.Diagnostics.Debug.WriteLine("igli" + serviceRates.ServiceId);
            await _context.Servicerates.AddAsync(serviceRates);
            await _context.SaveChangesAsync();
            return RedirectToAction("ServiceRates", new { Id = serviceRates.ServiceId, ServiceName = ServiceName });
        }
        [Authorize] //return list of rateDetails
        public IActionResult RateDetails(int VerNum, string ServiceId, string ServiceName)
        {
            ViewData["VerNum"] = VerNum;
            ViewData["ServiceId"] = ServiceId;
            ViewData["ServiceName"] = ServiceName;
            return View(_context.Ratesdetails.Where(e => e.Vernum == VerNum).ToList());
        }
        [Authorize] //return create rateDetails view
        public IActionResult CreateRateDetails(int VerNum, string ServiceName, string ServiceId)
        {
            ViewData["VerNum"] = VerNum;
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            //var rateCategories=new SelectList(_context.Ratecategories.ToDictionary(e=>e.CategoryId),"Key","Value");
            ViewBag.Ratecategories = new SelectList(_context.Ratecategories.ToDictionary(e => e.CategoryId, e => e.CategoryId), "Key", "Value");
            ViewBag.Ratetargets = new SelectList(_context.Ratetargets.ToDictionary(e => e.RateTarget, e => e.RateTarget), "Key", "Value");
            return View();
        }
        [HttpPost]
        [Authorize] //creates a new rateDetails
        public async Task<IActionResult> CreateRateDetailsAsync(Ratesdetails ratesdetails, string ServiceName, string ServiceId)
        {
            await _context.AddAsync(ratesdetails);
            await _context.SaveChangesAsync();
            return RedirectToAction("RateDetails", new { VerNum = ratesdetails.Vernum, ServiceName = ServiceName, ServiceId= ServiceId });
        }
        [Authorize] //returns referer details
        public IActionResult RefererDetails(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(_context.Referers.FirstOrDefault(e => e.ServiceId == ServiceId));
        }
        [Authorize] //returns edit referer view
        public IActionResult EditReferer(int id, string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(_context.Referers.FirstOrDefault(e => e.Id == id));
        }
        [HttpPost]
        [Authorize] //Edits the referer
        public async Task<IActionResult> EditRefererAsync(Referers referers, string ServiceId, string ServiceName)
        {
            _context.Entry(referers).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("RefererDetails", new { ServiceId = ServiceId, ServiceName = ServiceName });
        }
        [Authorize] //returns referer create view
        public IActionResult CreateReferer(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View();
        }
        [HttpPost]
        [Authorize] //creates new referer
        public async Task<IActionResult> CreateRefererAsync(Referers referers, string ServiceId, string ServiceName)
        {
            await _context.AddAsync(referers);
            await _context.SaveChangesAsync();
            return RedirectToAction("RefererDetails", new { ServiceId = ServiceId, ServiceName = ServiceName });
        }
    }
}