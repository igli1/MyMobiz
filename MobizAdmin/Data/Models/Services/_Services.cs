using System;
using System.Linq;
namespace MobizAdmin.Data
{
    public partial class Services
    {
        public  mymobiztestContext _context { get; set; }
        public string NextId()
        {
            string year = DateTime.Parse(DateTime.Now.ToString()).Year.ToString();
            string maxValue = _context.Services.Max(e => e.Id);
            if (maxValue != null)
            {

                string[] value = maxValue.Split('S');
                if (year == value[0])
                {
                    int Id;
                    string output = "1";
                    try
                    {
                        Id = (Convert.ToInt32(value[1]) + 1);
                        if (Id <= 99999)
                            output = String.Format("{0}{1}{2:D5}", year, "S", Id);
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
                    return year + "S00001";
            }

            return year + "S00001";       
        }
    }
}
