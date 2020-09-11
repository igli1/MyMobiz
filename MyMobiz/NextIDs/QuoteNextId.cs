using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyMobiz.Models;
namespace MyMobiz.NextIDs
{
    public partial class QuoteNextId
    {
        private readonly mymobiztestContext _context;
        public QuoteNextId(mymobiztestContext context)
        {
            _context = context;
        }
        public string NextId()
        {
            string year = DateTime.Parse(DateTime.Now.ToString()).Year.ToString();
            string maxValue = _context.Quotes.Max(e => e.Id);
            if (maxValue != null)
            {

                string[] value = maxValue.Split('Q');
                if (year == value[0])
                {
                    int Id;
                    string output = "1";
                    try
                    {
                        Id = (Convert.ToInt32(value[1]) + 1);
                        if (Id <= 999999)
                            output = String.Format("{0}{1}{2:D6}", year, "Q", Id);
                        else
                            return null;
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                    return output;
                }
                else
                    return year + "Q000001";
            }

            return year + "Q000001";       
        }
    }
}
