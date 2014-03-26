using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models.Server
{
    public class ServerItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string created_date { get; set; }
        public string modified_date { get; set; }
        public Location location { get; set; }
        public List<ServerMedia> media { get; set; }

        public DateTime CreatedDate()
        {
            DateTime d = DateTime.MinValue;
            DateTime.TryParse(created_date, out d);
            return d;
        }
        public DateTime ModifiedDate()
        {
            DateTime d = DateTime.MinValue;
            DateTime.TryParse(modified_date, out d);
            return d;
        }
    }
}
