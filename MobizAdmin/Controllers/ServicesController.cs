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
        public IActionResult ServicesList()
        {
            return View(_context.Services);
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
        public IActionResult RateCategories(string ServiceId, string ServiceName)
        {
            ViewData["ServiceName"] = ServiceName;
            var rc = _context.Ratecategories.Where(e => e.ServiceId == ServiceId).Select(rc => new
            {
                rc.Id,
                rc.Lexo,
                rc.CategoryConditions,
                rc.RateGrouping,
                rc.ServiceId,
                rc.Tsd,
                nOfQuotes = _context.Servicerates.Where(e => e.ServiceId == rc.ServiceId && _context.Ratedetails.Any(rd => rd.Vernum == e.VerNum && rd.CategoryId == rc.Id)).FirstOrDefault(e => e.NQuotes > 0).NQuotes,
                locked = _context.Servicerates.Where(e => e.ServiceId == rc.ServiceId && _context.Ratedetails.Any(rd => rd.Vernum == e.VerNum && rd.CategoryId == rc.Id)).FirstOrDefault(e => e.Locked == true).Locked,
            }).ToArray();
            var rct = new List<RateCategoriesType>();
            for(int i = 0; i < rc.Length; i++)
            {
                rct.Add(new RateCategoriesType
                {
                    Id = rc[i].Id,
                    Lexo = rc[i].Lexo,
                    CategoryConditions = rc[i].CategoryConditions,
                    RateGrouping = rc[i].RateGrouping,
                    ServiceId = rc[i].ServiceId,
                    Tsd = rc[i].Tsd
                });
                if (rc[i].nOfQuotes > 0 || rc[i].locked == true)
                    rct[i].Editable = false;
                else
                    rct[i].Editable = true;
            }
            return View(rct);
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
    }
}