using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MobizAdmin.Data;
using MobizAdmin.Models;

namespace MobizAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly ILogger<HomeController> _logger;
        private readonly mymobiztestContext _context;

        public HomeController(ILogger<HomeController> logger, mymobiztestContext context, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _context = context;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        
        public IActionResult _ServicesPartial()
        {
            return PartialView(_context.Services.ToList());
        }
               
            public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public IActionResult ServiceRate(IFormCollection collection)
        {
            string Id = collection["dropdownService"];
            if (Id == null)
            {
                return View(_context.Services.ToList());
            }
            else
            {
                IEnumerable<Services> services = _context.Services.ToList();
                services.FirstOrDefault(e => e.Id == Id).Servicerates = _context.Servicerates.Where(e => e.ServiceId == Id).ToList();
                ViewData["ServiceName"] = services.FirstOrDefault(e => e.Id == Id).ServiceName;
                ViewData["ServiceId"] = Id;
                return View(services);
            }
        }

        [Authorize]
        public IActionResult ServiceDetails(string Id)
        {
                return View(_context.Services.FirstOrDefault(e => e.Id==Id));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult LanguageSelect(string lang)
        {
            var culture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            return View();
        }
    }
}
