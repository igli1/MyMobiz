using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobizAdmin.Data;

namespace MobizAdmin.Controllers
{
    public class ServicesManagerController : Controller
    {
        public readonly mymobiztestContext _context;
        public ServicesManagerController(mymobiztestContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Manager()
        {
            ViewBag.Services = new SelectList(_context.Services.ToDictionary(e => e.Id, e => e.ServiceName), "Key", "Value");
            return View();
        }
        [Authorize]
        public async Task<JsonResult> GetServiceRates([FromBody] string serviceId)
        {
            var query = await _context.Servicerates.Where(e => e.ServiceId == serviceId).Select(e => new
            {
                verNum = e.VerNum,
                lexo = e.Lexo,
            }).AsNoTracking().ToListAsync();
            return Json(query);
        }
        [Authorize]
        public async Task<JsonResult> GetServiceRateSelected([FromBody] string vernum)
        {
            int VerNum = Convert.ToInt32(vernum);
            var query = await _context.Servicerates.Where(e => e.VerNum == VerNum).Select(e => new
            {
                verNum = e.VerNum,
                lexo = e.Lexo,
                locked = e.Locked,
                nQuotes = e.NQuotes,
                appDate = e.AppDate,
                defDate = e.DefDate,
                endDate = e.EndDate,
                eurKm = e.EurKm,
                eurMinDrive = e.EurMinDrive,
                eurMinimum = e.EurMinimum,
                eurMinWait = e.EurMinWait,
                maxPax = e.MaxPax
            }).AsNoTracking().FirstOrDefaultAsync();
            return Json(query);
        }
        [Authorize]
        public async Task<JsonResult> GetRateDetailsAndCategories([FromBody] string vernum)
        {
            int VerNum = Convert.ToInt32(vernum);
            var query = await _context.Ratecategories.Where(rc => _context.Servicerates.Any(sr => sr.VerNum == VerNum && rc.ServiceId == sr.ServiceId)).Select(rc => new
            {
                lexo = rc.Lexo,
                id = rc.Id,
                rateGrouping = rc.RateGrouping,
                ratesDetails = rc.Ratedetails.Select(rd => new
                {
                    id = rd.Id,
                    vernum = rd.Vernum,
                    detailConditions = rd.DetailConditions,
                    rateTargets = rd.Ratetargets.Select(rt => new
                    {
                        id = rt.Id,
                        rateTarget = rt.RateTarget,
                        rateFigure = rt.RateFigure,
                        rateOperator = rt.RateOperator
                    })
                })
            }).AsNoTracking().ToListAsync();
            return Json(query);
        }
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> CreateRateDetails([FromBody] Ratedetails rd)
        {
            await _context.Ratedetails.AddAsync(rd);
            await _context.SaveChangesAsync();
            return Json(await GetRateCategorie(rd.CategoryId));
        }
        [Authorize]
        public async Task<JsonResult> DeleteRateDetails([FromBody] DTIds ids)
        {
            _context.Ratedetails.Remove(_context.Ratedetails.Find(ids.RdId));
            _context.Ratetargets.RemoveRange(await _context.Ratetargets.Where(e => e.RateDetailId == ids.RdId).AsNoTracking().ToListAsync());
            await _context.SaveChangesAsync();
            return Json(await GetRateCategorie(ids.RcId));
        }
        private async Task<dynamic> GetRateCategorie(int id)
        {
            var query = await _context.Ratecategories.Where(rc => rc.Id == id).Select(rc => new
            {
                lexo = rc.Lexo,
                id = rc.Id,
                rateGrouping = rc.RateGrouping,
                ratesDetails = rc.Ratedetails.Select(rd => new
                {
                    id = rd.Id,
                    vernum = rd.Vernum,
                    detailConditions = rd.DetailConditions,
                    rateTargets = rd.Ratetargets.Select(rt => new
                    {
                        id = rt.Id,
                        rateTarget = rt.RateTarget,
                        rateFigure = rt.RateFigure,
                        rateOperator = rt.RateOperator
                    })
                })
            }).AsNoTracking().FirstOrDefaultAsync();
            return query;
        }
        [Authorize]
        public async Task<JsonResult> CreateRateTarget([FromBody] Ratetargets rt)
        {
            await _context.Ratetargets.AddAsync(rt);
            await _context.SaveChangesAsync();
            return Json(await GetRateCategorie(_context.Ratedetails.AsNoTracking().FirstOrDefault(e => e.Id == rt.RateDetailId).CategoryId));
        }
        [Authorize]
        public async Task<JsonResult> DeleteRateTargets([FromBody] string id)
        {
            int Id = Convert.ToInt32(id);
            _context.Ratetargets.Remove(_context.Ratetargets.Find(Id));
            await _context.SaveChangesAsync();
            return Json("");
        }
        [Authorize]
        public JsonResult GetApiKey([FromBody] string id)
        {
            return Json(_context.Services.Find(id).ApiKey);
        }
        [Authorize]
        public JsonResult UpdateDefaults([FromBody] DTDefaults dTDefaults)
        {
            _context.Database.ExecuteSqlRaw("UPDATE servicerates SET " + dTDefaults.property + " = " + dTDefaults.value + " WHERE vernum = " + dTDefaults.vernum + ";");
            return Json("Updated");
        }      
        [Authorize]
        public JsonResult UpdateRateTarget([FromBody] Ratetargets rt)
        {
            _context.Database.ExecuteSqlRaw("UPDATE ratetargets SET ratefigure = " + rt.RateFigure + ", rateoperator = '"+rt.RateOperator+ "' WHERE id = " + rt.Id + " ;");
            return Json("Updated");
        }
        [Authorize]
        public JsonResult UpdateDateTime([FromBody] DTDateTime dt)
        {
            _context.Database.ExecuteSqlRaw("UPDATE servicerates SET "+dt.Property+" = '" +  dt.Value.ToString("yyyy-MM-dd") + "' WHERE vernum = " + dt.VerNum + " ;");
            return Json("Updated");
        }
        [Authorize]
        public JsonResult UpdateLocked([FromBody] DTLocked dt)
        {
            _context.Database.ExecuteSqlRaw("UPDATE servicerates SET locked = " + dt.Locked + " WHERE vernum = " + dt.VerNum + " ;");
            return Json("Updated");
        }
    }
}