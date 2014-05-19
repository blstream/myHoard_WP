using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models.Server
{
    public class ServerCollection
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<string> tags { get; set; }
        public int items_number { get; set; }
        public string created_date { get; set; }
        public string modified_date { get; set; }
        public string owner { get; set; }

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
