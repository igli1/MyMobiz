using System;
using System.Linq;
namespace MyMobiz.Models
{ 
    public partial class Rides
    {
        private readonly mymobiztestContext _context;
        public Rides(mymobiztestContext context)
        {
            _context = context;
        }
        public string NextId()
        {
            string year = DateTime.Parse(DateTime.Now.ToString()).Year.ToString();
            string maxValue = _context.Rides.Max(e => e.Id);
            if (maxValue != null)
            {
                string[] value = maxValue.Split('R');
                if (year == value[0])
                {
                    int Id;
                    string output = "1";
                    try
                    {
                        Id = (Convert.ToInt32(value[1]) + 1);
                        if (Id <= 999999)
                            output = String.Format("{0}{1}{2:D6}", year, "R", Id);
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
                    return year + "R000001";
            }

            return year + "R000001";
        }
    }
}
