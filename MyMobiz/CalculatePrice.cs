using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyMobiz.Models;
namespace MyMobiz
{
    public class CalculatePrice
    {
        private readonly mymobiztestContext _context;
        public CalculatePrice(mymobiztestContext context)
        {
            _context = context;
        }
        public async Task <decimal> FindPriceAsync(string serviceId, int Kms, int DriveTime, string categoryId)
        {
            Servicerates serviceRates = await _context.Servicerates.Where(e => e.AppDate <= DateTime.Today 
            && e.ServiceId == serviceId).OrderByDescending(e => e.AppDate).FirstOrDefaultAsync();
            if (serviceRates != null)
            {
                //Ratesdetails rateDetails = await _context.Ratesdetails.Where()
                decimal price = (serviceRates.EurKm * Kms) + (serviceRates.EurMinDrive * DriveTime);
                return price;
            }
            return 1;
        }
    }
}