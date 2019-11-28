using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightApi.Models
{
    public class Flight
    {
        public long Id { get; set; }
        //public string Name { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public string DepartDate { get; set; }
        public string ReturnDate { get; set; }
        public string NumberOfChanges { get; set; }
        public string TripClass { get; set; }
        public string ShowToAffiliates { get; set; }
        public string Actual { get; set; }
        public string Gate { get; set; }
        public string FoundAt { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }


    }
}
