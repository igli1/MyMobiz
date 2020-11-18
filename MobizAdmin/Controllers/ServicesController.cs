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
        [Authorize] //return list of rateDetails
        public IActionResult RateDetails(int VerNum, string ServiceId, string ServiceName)
        {
            ViewData["VerNum"] = VerNum;
            ViewData["ServiceId"] = ServiceId;
            ViewData["ServiceName"] = ServiceName;
            return View(_context.Ratedetails.Where(e => e.Vernum == VerNum).ToList());
        }
        [Authorize] //return create rateDetails view
        public IActionResult CreateRateDetails(int VerNum, string ServiceName, string ServiceId)
        {
            ViewData["VerNum"] = VerNum;
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            //var rateCategories=new SelectList(_context.Ratecategories.ToDictionary(e=>e.CategoryId),"Key","Value");
            ViewBag.Ratecategories = new SelectList(_context.Ratecategories.ToDictionary(e => e.Lexo, e => e.Lexo), "Key", "Value");
            ViewBag.Ratetargets = new SelectList(_context.Ratetargets.ToDictionary(e => e.RateTarget, e => e.RateTarget), "Key", "Value");
            return View();
        }
        [HttpPost]
        [Authorize] //creates a new rateDetails
        public async Task<IActionResult> CreateRateDetailsAsync(Ratedetails ratedetails, string ServiceName, string ServiceId)
        {
            await _context.AddAsync(ratedetails);
            await _context.SaveChangesAsync();
            return RedirectToAction("RateDetails", new { VerNum = ratedetails.Vernum, ServiceName = ServiceName, ServiceId= ServiceId });
        }
        [Authorize] //returns referer details
        public IActionResult RefererDetails(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(_context.Webreferers.FirstOrDefault(e => e.ServiceId == ServiceId));
        }
        [Authorize] //returns list of Web Referers for specifik Service Id
        public IActionResult WebReferers(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(_context.Webreferers.Where(e => e.ServiceId == ServiceId).ToList());
        }
        [Authorize] //returns referer create view
        public IActionResult CreateWebReferers(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View();
        }
        [HttpPost]
        [Authorize] //creates new referer
        public async Task<IActionResult> CreateWebReferersAsync(Webreferers webReferers, string ServiceId, string ServiceName)
        {
            await _context.AddAsync(webReferers);
            await _context.SaveChangesAsync();
            return RedirectToAction("WebReferers", new { ServiceId = ServiceId, ServiceName = ServiceName });
        }
        [Authorize] //returns Web Referer delete view
        public IActionResult DeleteWebReferer(int Id, string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(_context.Webreferers.FirstOrDefault(e=>e.Id==Id));
        }
        [HttpPost]
        [Authorize] //Deletes Web Referer
        public async Task<IActionResult> DeleteWebRefererAsync(int Id, string ServiceId, string ServiceName)
        {
            _context.Webreferers.Remove(_context.Webreferers.Find(Id));
            await _context.SaveChangesAsync();
            return RedirectToAction("WebReferers", new { ServiceId = ServiceId, ServiceName = ServiceName });
        }
        [Authorize] //returns edit referer view
        public IActionResult EditWebReferer(int id, string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(_context.Webreferers.FirstOrDefault(e => e.Id == id));
        }
        [HttpPost]
        [Authorize] //Edits the referer
        public async Task<IActionResult> EditWebRefererAsync(Webreferers webReferers, string ServiceId, string ServiceName)
        {
            _context.Entry(webReferers).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("WebReferers", new { ServiceId = ServiceId, ServiceName = ServiceName });
        }
        [Authorize] //return list of service details
        public async Task<IActionResult> CreateServiceDetailsAsync()
        {
            RatesDetailsModel model = new RatesDetailsModel();
            model.Services = new SelectList((await _context.Services.AsNoTracking().ToListAsync()).ToDictionary(e => e.Id, e => e.ServiceName), "Key", "Value");
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateServiceDetailsAsync(string ServiceId, int VerNum)
        {
            RatesDetailsModel model = new RatesDetailsModel();
            model.Services = new SelectList((await _context.Services.AsNoTracking().ToListAsync()).ToDictionary(e => e.Id, e => e.ServiceName), "Key", "Value");
            if (ServiceId!=null)
            {
                model.ServiceRates = new SelectList((await _context.Servicerates.Where(e=>e.ServiceId==ServiceId).AsNoTracking().ToListAsync()).ToDictionary(e => e.VerNum, e => e.Lexo), "Key", "Value");
            }
            return View(model);
        }
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Services = new SelectList(_context.Services.ToDictionary(e => e.Id, e => e.ServiceName), "Key", "Value");

            return View();
        }
        /*
        [Authorize]
        public async Task<IActionResult> GetServices()
        {
            var services = await _context.Services.Select(e=> new
            {
                Id = e.Id,
                ServiceName = e.ServiceName
            }
            ).AsNoTracking().ToListAsync();
            return Json(services);
        }*/
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> GetServiceRates([FromBody] string serviceid)
        {
            var serviceRates = await _context.Servicerates.Where(e => e.ServiceId == serviceid).Select(e => new
            {
                VerNum = e.VerNum,
                Lexo = e.Lexo
            }
            ).AsNoTracking().ToListAsync();
            return Json(serviceRates);
        }
        [HttpPost]
        [Authorize]
        public JsonResult GetServiceRate([FromBody] string vernum)
        {
            int VerNum = Convert.ToInt32(vernum);
            var serviceRate = _context.Servicerates.Where(e=>e.VerNum== VerNum).Select(e => new
            {
                verNum=e.VerNum,
                eurKm=e.EurKm,
                eurMinDrive = e.EurMinDrive,
                eurMinimum = e.EurMinimum,
                eurMinWait = e.EurMinWait,
                defDate =e.DefDate,
                appDate=e.AppDate,
                endDate=e.EndDate,
            });
            return Json(serviceRate);
        }
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> GetRateCategoriesSelected([FromBody] string ServiceId)
        {
            var ratecategories = await _context.Ratecategories.Where(e =>e.ServiceId==ServiceId && _context.Ratedetails.AsNoTracking().Any(x => e.Id == x.CategoryId)).AsNoTracking().Select(e => new
            {
                id = e.Id,
                lexo = e.Lexo,
                grouping = e.RateGrouping,
                conditions = e.CategoryConditions
            }).AsNoTracking().ToListAsync();
            return Json(ratecategories);
        }
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> GetRateCategoriesSelectedNotSelected([FromBody] string ServiceId)
        {
            var ratecategories = await _context.Ratecategories.Where(e => e.ServiceId == ServiceId && !_context.Ratedetails.AsNoTracking().Any(x => e.Id == x.CategoryId)).AsNoTracking().Select(e => new
            {
                id = e.Id,
                lexo = e.Lexo,
                grouping = e.RateGrouping,
                conditions = e.CategoryConditions
            }).AsNoTracking().ToListAsync();
            return Json(ratecategories);
        }
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddRateDetails([FromBody] DTRateDetails a)
        {
            List<Ratedetails> ratedetails = a.ratedetails;
            await _context.Ratedetails.AddRangeAsync(ratedetails);
            await _context.SaveChangesAsync();
            return Json("Ok");
        }
    }
}