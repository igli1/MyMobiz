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
using System.Text.Json;
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
        public async Task<IActionResult> ServicesList()
        {
            List<DTServicesList> services = JsonConvert.DeserializeObject<List<DTServicesList>>( JsonConvert.SerializeObject( await _context.Services.Select(s => new
            {
                s.Id,
                s.ServiceName,
                s.Tsd,
                CountRates= s.Servicerates.Count,
                CountCategories = s.Ratecategories.Count,
                CountReferers = s.Webreferers.Count,
                CountLanguages = s.Servicelangs.Count
            }).ToListAsync()));
            return View(services);
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
            await _context.Database.ExecuteSqlRawAsync("INSERT INTO `mymobiztest`.`services`(`ID`,`ServiceName`,`ApiKey`)VALUES(ServicesNextId(),'" + service.ServiceName + "', '" + service.ApiKey + "');");
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
            return View(_context.Servicerates.Where(e => e.ServiceId == ServiceId));
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
        [Authorize] //creates new Rate Categorie
        public IActionResult CreateRateCategorie(string ServiceId, string ServiceName)
        {
            ViewData["ServiceId"] = ServiceId;
            ViewData["ServiceName"] = ServiceName;
            ViewBag.RateGrouping = new SelectList(_context.Rategroupings.Where(e=>e.Tsd==null || e.Tsd>DateTime.Now).ToDictionary(e => e.RateGrouping, e => e.RateGrouping), "Key", "Value");
            return View();
        }
        [Authorize] //return list of services
        public async Task<IActionResult> RateCategories(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            List<Ratecategories> rc = await _context.Ratecategories.Where(e => e.ServiceId == ServiceId).ToListAsync();
            return View(rc);
        }
        [HttpPost]
        [Authorize] //creates new Rate Categorie
        public async Task<IActionResult> CreateRateCategorie(Ratecategories rateCategories,string ServiceName)
        {
            await _context.Ratecategories.AddAsync(rateCategories);
            await _context.SaveChangesAsync();
            return RedirectToAction("RateCategories", new { ServiceId = rateCategories.ServiceId, ServiceName = ServiceName });
        }
        [Authorize] //creates new Rate Categorie
        public async Task<IActionResult> RateCategorieDetails(int Id, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            return View(await _context.Ratecategories.FindAsync(Id));
        }
        [Authorize] //return list of services
        public IActionResult RateGroupings()
        {
            return View(_context.Rategroupings);
        }
        [Authorize] //return create service view
        public IActionResult CreateRateGrouping()
        {
            return View();
        }
        [HttpPost]
        [Authorize] //return create service view
        public async Task<IActionResult> CreateRateGrouping(Rategroupings rg)
        {
            await _context.Rategroupings.AddAsync(rg);
            await _context.SaveChangesAsync();
            return RedirectToAction("RateGroupings");
        }
        [Authorize] //creates a new rate grouping
        public async Task<IActionResult> DeleteRateGrouping(string RateGrouping)
        {
            await _context.Database.ExecuteSqlRawAsync("Update rategroupings set tsd = now() where RateGrouping = '" + RateGrouping + "'");
            return RedirectToAction("RateGroupings");
        }
        [Authorize] //creates a new rate grouping
        public async Task<IActionResult> DeleteServiceRate(int  VerNum, string ServiceId, string ServiceName)
        {
            await _context.Database.ExecuteSqlRawAsync("Update servicerates set tsd = now() where vernum = '" + VerNum + "'");
            return RedirectToAction("ServiceRates", new { ServiceId = ServiceId, ServiceName = ServiceName });
        }
        [Authorize] //Edits Rate Categorie
        public async Task<IActionResult> EditRateCategorie(int Id, string ServiceName)
        {
            Ratecategories rc = await _context.Ratecategories.FindAsync(Id);
            ViewData["ServiceName"] = ServiceName;
            ViewBag.RateGrouping = new SelectList(_context.Rategroupings.Where(e => e.Tsd == null || e.Tsd > DateTime.Now).ToDictionary(e => e.RateGrouping, e => e.RateGrouping), "Key", "Value", _context.Rategroupings.FirstOrDefaultAsync(e=>e.RateGrouping==rc.RateGrouping));
            return View(rc);
        }
        [HttpPost]
        [Authorize] //Edits Rate Categorie
        public async Task<IActionResult> EditRateCategorie(Ratecategories rc, string ServiceName)
        {
            _context.Entry(rc).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("RateCategorieDetails", new { id = rc.Id, ServiceName=ServiceName });
        }
        [Authorize] //creates a new rate grouping
        public async Task<IActionResult> DeleteRateCategorie(int Id, string ServiceId, string ServiceName)
        {
            await _context.Database.ExecuteSqlRawAsync("Update ratecategories set tsd = now() where id = '" + Id + "'");
            return RedirectToAction("RateCategories", new { ServiceId = ServiceId, ServiceName = ServiceName });
        }
        [Authorize] //creates a new rate grouping
        public IActionResult ServiceLanguages(string ServiceId, string ServiceName)
        {
                var langs =  _context.Servicelangs.Where(e => e.ServiceId == ServiceId).Select(e => new
             {
                 Id = e.Id,
                 ServiceId = e.ServiceId,
                 Lang= e.Lang,
                 Word = e.LangNavigation.Word
             });
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(JsonConvert.DeserializeObject<List<DTServiceLanguages>>(JsonConvert.SerializeObject(langs)));
        }
        [Authorize] //creates a new rate grouping
        public IActionResult CreateServiceLanguage(string ServiceId, string ServiceName)
        {
            ViewData["ServiceId"] = ServiceId;
            ViewData["ServiceName"] = ServiceName;
            ViewBag.Languages = new SelectList(_context.Langs.Where(e=>!_context.Servicelangs.Any(sl=> sl.ServiceId == ServiceId && sl.Lang==e.Lang)).ToDictionary(e => e.Lang, e => e.Word), "Key", "Value");
            return View();
        }
        [HttpPost]
        [Authorize] //creates a new rate grouping
        public async Task<JsonResult> CreateServiceLanguage(DTServiceLanguages serviceLangs, string ServiceName)
        {
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(serviceLangs));
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(ServiceName));
            if (serviceLangs.Existing == true)
            {
                await _context.Servicelangs.AddAsync(new Servicelangs(){
                    ServiceId= serviceLangs.ServiceId,
                    Lang= serviceLangs.Lang
                });
                await _context.SaveChangesAsync();
            }
            else if(serviceLangs.Existing == false)
            {
                await _context.Langs.AddAsync(new Langs()
                {
                    Lang = serviceLangs.Lang,
                    Word = serviceLangs.Word
                });
                await _context.Servicelangs.AddAsync(new Servicelangs()
                {
                    ServiceId = serviceLangs.ServiceId,
                    Lang = serviceLangs.Lang
                });
                await _context.SaveChangesAsync();
            }
            return Json(new { serviceLangs.ServiceId, ServiceName });
        }
        [Authorize] //
        public IActionResult WebReferers(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(_context.Webreferers.Where(e=>e.ServiceId==ServiceId));
        }
        [Authorize] //
        public IActionResult CreateWebReferers(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View();
        }
        [HttpPost]
        [Authorize] //
        public async Task<IActionResult> CreateWebReferers(Webreferers webReferer, string ServiceName)
        {
            await _context.Webreferers.AddAsync(webReferer);
            await _context.SaveChangesAsync();
            return RedirectToAction("WebReferers", new { webReferer.ServiceId,ServiceName });
        }
        [Authorize] //
        public async Task<IActionResult> EditWebReferer(int Id, string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            ViewData["ServiceId"] = ServiceId;
            return View(await _context.Webreferers.FirstOrDefaultAsync(e => e.Id == Id));
        }
        [HttpPost]
        [Authorize] //
        public async Task<IActionResult> EditWebReferer(Webreferers webReferer, string ServiceName)
        {
            _context.Webreferers.Update(webReferer);
            await _context.SaveChangesAsync();
            return RedirectToAction("WebReferers", new { webReferer.ServiceId, ServiceName });
        }
    }
}