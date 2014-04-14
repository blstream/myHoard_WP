using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Models.Server
{
    public class Location
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float lat { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float lng { get; set; }
    }
}
