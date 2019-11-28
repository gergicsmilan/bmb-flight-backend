using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightApi.Models
{
    public class Filter
    {
        public string Destination { get; set; }
        public string Origin { get; set; }
        public string DepartDate { get; set; }
        public string ReturnDate { get; set; }
    }
}
