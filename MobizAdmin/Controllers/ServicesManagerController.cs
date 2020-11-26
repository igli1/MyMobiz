using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobizAdmin.Data;
using Newtonsoft.Json;

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
            ViewBag.Services = new SelectList(_context.Services.Where(sv=>sv.Tsd>DateTime.Now || sv.Tsd ==null) .ToDictionary(e => e.Id, e => e.ServiceName), "Key", "Value");
            return View();
        }
        [Authorize]
        public IActionResult ManagerServiceRateSelected(int VerNum)
        {
            string ServiceId = _context.Servicerates.Find(VerNum).ServiceId;
            ViewData["ServiceId"] = ServiceId;
            ViewData["VerNum"] = VerNum;
            ViewBag.Services = new SelectList(_context.Services.Where(sv => sv.Tsd > DateTime.Now || sv.Tsd == null).ToDictionary(e => e.Id, e => e.ServiceName), "Key", "Value");
            return View("~/Views/ServicesManager/Manager.cshtml");
        }
        [Authorize]
        public IActionResult ManagerServiceSelected(string ServiceId)
        {
            ViewData["ServiceId"] = ServiceId;
            ViewBag.Services = new SelectList(_context.Services.Where(sv => sv.Tsd > DateTime.Now || sv.Tsd == null).ToDictionary(e => e.Id, e => e.ServiceName), "Key", "Value");
            return View("~/Views/ServicesManager/Manager.cshtml");
        }
        [Authorize]
        public async Task<JsonResult> GetServiceRates(string serviceId)
        {
            var query = await _context.Servicerates.Where(e => e.ServiceId == serviceId && (e.Tsd>DateTime.Now || e.Tsd == null)).Select(e => new
            {
                verNum = e.VerNum,
                lexo = e.Lexo,
            }).AsNoTracking().ToListAsync();
            return Json(query);
        }
        [Authorize]
        public async Task<JsonResult> GetServiceRateSelected(int vernum)
        {
            //int VerNum = Convert.ToInt32(vernum);
            var query = await _context.Servicerates.Where(e => e.VerNum == vernum).Select(e => new
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
        public async Task<JsonResult> GetRateDetailsAndCategories(int vernum, int langId)
        {
            var query = await _context.Ratecategories.Where(rc => _context.Servicerates.Any(sr => sr.VerNum == vernum && rc.ServiceId == sr.ServiceId) && (rc.Tsd>DateTime.Now || rc.Tsd == null)).Select(rc => new
            {
                word =  _context.Lexicons.Where(lc=> _context.Servicelangs.Any(sl=>sl.Id== langId && lc.Lang==sl.Lang)&& lc.Lexo==rc.Lexo && lc.ServiceId==rc.ServiceId).FirstOrDefault().Word ?? rc.Lexo,
                lexo= rc.Lexo,
                id = rc.Id,
                rateGrouping = rc.RateGrouping,
                ratesDetails = rc.Ratedetails.Where(rd => rd.Vernum == vernum && (rd.Tsd > DateTime.Now || rd.Tsd == null)).Select(rd => new
                {

                    id = rd.Id,
                    vernum = rd.Vernum,
                    detailConditions = rd.DetailConditions,
                    rateTargets = rd.Ratetargets.Where(rt=>rt.Tsd > DateTime.Now || rt.Tsd == null).Select(rt => new
                    {
                        id = rt.Id,
                        rateTarget = rt.RateTarget,
                        rateFigure = rt.RateFigure,
                        rateOperator = rt.RateOperator
                    })
                })
            }).AsNoTracking().ToListAsync();
            /*var query = await _context.Ratecategories.Where(rc => _context.Servicerates.Any(sr => sr.VerNum == vernum && rc.ServiceId == sr.ServiceId)).Select(rc => new
            {
                lexo = rc.Service.Servicelangs.FirstOrDefault(sl => sl.Id == langId).LangNavigation.Lexicons.FirstOrDefault(lx => lx.Lexo == rc.Lexo).Word == null ? null :
                "hello",
                id = rc.Id,
                rateGrouping = rc.RateGrouping,
                ratesDetails = rc.Ratedetails.Where(rd => rd.Vernum == vernum).Select(rd => new
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
            }).AsNoTracking().ToListAsync();*/
            return Json(query);
        }
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> CreateRateDetails(Ratedetails ratesDetails, int langId)
        {
            await _context.Ratedetails.AddAsync(ratesDetails);
            await _context.SaveChangesAsync();
            return Json(await GetRateCategorie(ratesDetails.CategoryId, ratesDetails.Vernum, langId));
        }
        [Authorize]
        public async Task<JsonResult> DeleteRateDetails(int rdId, int rcId, int langId)
        {
            Ratedetails rd = _context.Ratedetails.Find(rdId);
            rd.Tsd = DateTime.Now;
            _context.Attach(rd);
            _context.Entry(rd).Property(p => p.Tsd).IsModified = true;
            _context.Ratetargets.RemoveRange(await _context.Ratetargets.Where(e => e.RateDetailId == rdId).AsNoTracking().ToListAsync());
            await _context.SaveChangesAsync();
            return Json(await GetRateCategorie(rcId, rd.Vernum, langId));
        }
        private async Task<dynamic> GetRateCategorie(int id, int verNum, int langId)
        {
            var query = await _context.Ratecategories.Where(rc => rc.Id == id).Select(rc => new
            {
                word = _context.Lexicons.Where(lc => _context.Servicelangs.Any(sl => sl.Id == langId && lc.Lang == sl.Lang) && lc.Lexo == rc.Lexo).FirstOrDefault().Word ?? rc.Lexo,
                lexo = rc.Lexo,
                id = rc.Id,
                rateGrouping = rc.RateGrouping,
                ratesDetails = rc.Ratedetails.Where(rd=>rd.Vernum== verNum) .Select(rd => new
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
        public async Task<JsonResult> CreateRateTarget(Ratetargets rt)
        {
            await _context.Ratetargets.AddAsync(rt);
            await _context.SaveChangesAsync();  
            return Json(rt);
        }
        [Authorize]
        public async Task<JsonResult> DeleteRateTargets(int id)
        {
            //int Id = Convert.ToInt32(id);
            _context.Ratetargets.Remove(_context.Ratetargets.Find(id));
            await _context.SaveChangesAsync();
            return Json("Rate Target Deleted");
        }
        [Authorize]
        public JsonResult UpdateDefaults(int vernum, string property, decimal value)
        {
            _context.Database.ExecuteSqlRaw("UPDATE servicerates SET " + property + " = " + value + " WHERE vernum = " + vernum + ";");
            return Json(property+" Updated");
        }
        [Authorize]
        public JsonResult UpdateRateTarget(Ratetargets rt)
        {
            _context.Attach(rt);
            _context.Entry(rt).Property(p => p.RateOperator).IsModified = true;
            _context.Entry(rt).Property(p => p.RateFigure).IsModified = true;
            _context.SaveChanges();
            //rt.Tsu = null;
            //_context.Database.ExecuteSqlRaw("UPDATE ratetargets SET ratefigure = " + rt.RateFigure + ", rateoperator = '" + rt.RateOperator + "' WHERE id = " + rt.Id + " ;");
            return Json(rt);
        }
        [Authorize]
        public JsonResult UpdateDateTime(DateTime value, string property, int vernum)
        {
            _context.Database.ExecuteSqlRaw("UPDATE servicerates SET " + property + " = '" + value.ToString("yyyy-MM-dd") + "' WHERE vernum = " + vernum + " ;");
            return Json(property + " Updated");
        }
        [Authorize]
        public JsonResult UpdateLocked(int vernum, bool locked)
        {
            _context.Database.ExecuteSqlRaw("UPDATE servicerates SET locked = " + locked + " WHERE vernum = " + vernum + " ;");
            return Json("Locked Updated");
        }
        [Authorize]
        public PartialViewResult CreateServiceModal()
        {
            return PartialView("~/Views/ServicesManager/_CreateService.cshtml");
        }
        [HttpPost]
        [Authorize] //creates a new service
        public JsonResult CreateServiceModal(Services service)
        {
            dynamic query = _context.Services.FromSqlRaw("INSERT INTO `mymobiztest`.`services`(`ID`,`ServiceName`,`ApiKey`)VALUES(ServicesNextId(),'" + service.ServiceName + "', '" + service.ApiKey + "');SELECT * FROM services WHERE services.ID=(SELECT MAX(id) FROM services)");
            service.Id = Enumerable.FirstOrDefault<dynamic>(query).Id;
            return Json(service);
        }  
        [Authorize]
        public PartialViewResult CreateServiceRateModal(string serviceId)
        {
            ViewData["ServiceId"] = serviceId;
            return PartialView("~/Views/ServicesManager/_CreateServiceRate.cshtml");
        }
        [HttpPost]
        [Authorize] //creates a new service rate
        public JsonResult CreateServiceRate(Servicerates sr)
        {
            _context.Servicerates.Add(sr);
            _context.SaveChanges();
            return Json(sr);
        }
        [Authorize]
        public PartialViewResult CreateRateCategorieModal(string serviceId)
        {
            ViewData["ServiceId"] = serviceId;
            ViewBag.RateGrouping = new SelectList(_context.Rategroupings.ToDictionary(e => e.RateGrouping, e => e.RateGrouping), "Key", "Value");
            return PartialView("~/Views/ServicesManager/_CreateRateCategorie.cshtml");
        }
        [HttpPost]
        [Authorize] //creates a new rate categorie
        public JsonResult CreateRateCategorie(Ratecategories rc)
        {
            _context.Ratecategories.Add(rc);
            _context.SaveChanges();
            return Json(rc);
        }
        [HttpPost]
        [Authorize] //Duplicates VerNum with all the Rates Details and Rates Targets
        public JsonResult DuplicateVerNum(Servicerates servicerates)
        {
            _context.Servicerates.Add(servicerates);
            _context.SaveChanges();
            dynamic query = new { verNum = servicerates.VerNum,
                lexo = servicerates.Lexo };
            return Json(query);
        }
        [HttpPost]
        [Authorize] //Updates Detail Conditions
        public JsonResult UpdateDetailConditions(string  condition, int id)
        {
            _context.Database.ExecuteSqlRaw("UPDATE ratedetails SET detailconditions = '" +condition+ "' where id = " +id);
            return Json("Detail Conditions Updated");
        }
        [Authorize]
        public async Task<JsonResult> GetServiceLanguages(string serviceId)
        {
            var query = await _context.Servicelangs.Where(e => e.ServiceId == serviceId).Select(e => new
            {
                id = e.Id,
                word = e.LangNavigation.Word
            }).AsNoTracking().ToListAsync();
            query.Add(new{ id = -1, word = "Lexo"});
            return Json(query);
        }
        [Authorize]
        public async Task<JsonResult> InsertWord(int id,string value,  int langId)
        {
            if(langId != -1)
            {
                Lexicons lexicons = new Lexicons();
                if (await _context.Lexicons.AnyAsync(lc => _context.Ratecategories.Any(rc => rc.Id == id && lc.ServiceId == rc.ServiceId && lc.Lexo == rc.Lexo) && _context.Servicelangs.Any(sl => sl.Id == langId && lc.Lang == sl.Lang)))
                {
                    lexicons = await _context.Lexicons.FirstOrDefaultAsync(lc => _context.Ratecategories.Any(rc => rc.Id == id && lc.ServiceId == rc.ServiceId && lc.Lexo == rc.Lexo) && _context.Servicelangs.Any(sl => sl.Id == langId && lc.Lang == sl.Lang));
                    lexicons.Word = value;
                    _context.Attach(lexicons);
                    _context.Entry(lexicons).Property(p => p.Word).IsModified = true;
                    await _context.SaveChangesAsync();
                    return Json("Category: " + id + " Word Updated");
                }
                else
                {
                    var query = await _context.Ratecategories.Where(rc => rc.Id == id).Select(rc => new
                    {
                        lexo = rc.Lexo,
                        serviceId = rc.Service.Id,
                        lang = _context.Servicelangs.FirstOrDefault(sl => sl.Id == langId).Lang

                    }).AsNoTracking().FirstOrDefaultAsync();
                    lexicons.Lang = query.lang;
                    lexicons.Lexo = query.lexo;
                    lexicons.ServiceId = query.serviceId;
                    lexicons.Word = value;
                    await _context.AddAsync(lexicons);
                    await _context.SaveChangesAsync();
                    return Json("Category: " + id + " Word Inserted");
                }
            }
            return null;
        }
        [Authorize]
        public PartialViewResult DuplicateVerNumModal(int vernum)
        {
            string ServiceId = _context.Servicerates.Find(vernum).ServiceId;
            ViewData["ServiceId"] = ServiceId;
            ViewData["VerNum"] = vernum;
            ViewBag.ServicesList = new SelectList(_context.Services.Where(e=> e.Id != ServiceId).ToDictionary(e => e.Id, e => e.ServiceName), "Key", "Value");
            return PartialView("~/Views/ServicesManager/_DuplicateVerNumOtherService.cshtml");
        }
        [Authorize]
        public async Task<JsonResult> DuplicateVerNumOtherService(DuplicateVernum dv)
        {
            Servicerates rate = await _context.Servicerates.FindAsync(dv.VerNum);
            Servicerates duplicated=new Servicerates();
            duplicated.AppDate = rate.AppDate;
            duplicated.DefDate = rate.DefDate;
            duplicated.EndDate = rate.EndDate;
            duplicated.EurKm = rate.EurKm;
            duplicated.EurMinDrive= rate.EurMinDrive;
            duplicated.EurMinimum = rate.EurMinimum;
            duplicated.EurMinWait = rate.EurMinWait;
            duplicated.Lexo = rate.Lexo;
            duplicated.ServiceId = dv.ServiceToDuplicateId;
            await _context.AddAsync(duplicated);
            await _context.SaveChangesAsync();
            var RateCategories = await _context.Ratecategories.Where(rc =>  _context.Ratedetails.Any(rd => rd.Vernum == dv.VerNum && rc.Id == rd.CategoryId) && !_context.Ratecategories.Any(nrc=> nrc.ServiceId==dv.ServiceToDuplicateId && nrc.Lexo==rc.Lexo))
                .Select(rc=>new
                {
                    rc.Lexo,
                    rc.RateGrouping,
                    rc.CategoryConditions,
                    ServiceId = dv.ServiceToDuplicateId,
                    Ratedetails = rc.Ratedetails.Where(rd=> rd.Vernum == dv.VerNum && rc.Id == rd.CategoryId).Select(rd=> new
                    {
                        rd.DetailConditions,
                        Vernum = duplicated.VerNum,
                        Ratetargets=rd.Ratetargets.Select(rt => new
                        {
                            rt.RateFigure,
                            rt.RateOperator,
                            rt.RateTarget
                        })
                    })
                }).AsNoTracking().ToListAsync();
            var RateDetails = await _context.Ratedetails.Where(rd => rd.Vernum == dv.VerNum && _context.Ratecategories.Any(rc=>rc.Id==rd.CategoryId && _context.Ratecategories.Any(nrc=>nrc.ServiceId==dv.ServiceToDuplicateId && nrc.Lexo==rc.Lexo))).Select(rd => new
            {
                CategoryId = _context.Ratecategories.FirstOrDefault(rc=>_context.Ratecategories.Any(nrc=>nrc.Id==rd.CategoryId && nrc.Lexo==rc.Lexo) && rc.ServiceId==dv.ServiceToDuplicateId).Id,
                rd.DetailConditions,
                Vernum = duplicated.VerNum,
                Ratetargets = rd.Ratetargets.Select(rt => new
                {
                    rt.RateFigure,
                    rt.RateOperator,
                    rt.RateTarget
                })
            }).ToListAsync();

            if (RateCategories != null)
            {
                List<Ratecategories> rc = JsonConvert.DeserializeObject<List<Ratecategories>>(JsonConvert.SerializeObject(RateCategories));
                await _context.AddRangeAsync(rc);
            }
            if (RateDetails != null)
            {
                List<Ratedetails> rd = JsonConvert.DeserializeObject<List<Ratedetails>>(JsonConvert.SerializeObject(RateDetails));
                await _context.AddRangeAsync(rd);
            }
            if (RateDetails != null || RateCategories != null)
            await _context.SaveChangesAsync();
            var dr = new
            {
                serviceId= duplicated.ServiceId,
                verNum= duplicated.VerNum
            };
            return Json(dr);
        }
    }
}