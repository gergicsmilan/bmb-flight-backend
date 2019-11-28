using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightApi.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlightApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightContext _context;

        public FlightsController(FlightContext context)
        {
            _context = context;
        }


        //methord for building the url
        public string urlBuilder(string origin, string destination, string departDate, string returnDate, string currency, string tripClass, string sorting)
        {
            //string result = $"//api.travelpayouts.com/v1/prices/cheap?origin={origin}&destination={destination}&depart_date=2019-12&return_date=2019-12&sorting=price&token=35120b8381d8f9ecea3fbd296b0697c3";           
            StringBuilder UrlSb = new StringBuilder();
            UrlSb.Append("http://api.travelpayouts.com/v2/prices/nearest-places-matrix?");
            UrlSb.Append($"origin={origin}&");
            UrlSb.Append($"destination={destination}&");
            //UrlSb.Append($"depart_date={departDate}&");
            //UrlSb.Append($"return_date={returnDate}&");
            //UrlSb.Append($"trip_class={tripClass}&");
            //UrlSb.Append($"sorting={sorting}&");
            //UrlSb.Append($"currency={currency}&");

            UrlSb.Append("token=35120b8381d8f9ecea3fbd296b0697c3");
            string result = UrlSb.ToString();
            return result;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            //Dictionary<string, string> incomingJsonForFlight = new Dictionary<string, string>();
            
            //string path = urlBuilder(incomingJsonForFlight["direction"],
            //                            incomingJsonForFlight["toDirection"],
            //                            incomingJsonForFlight["departDate"],
            //                            incomingJsonForFlight["returnDate"],
            //                            null,
            //                            null,
            //                            null);


            //string path = urlBuilder("BUD", null, null, null, null,null,null);
            //string testPath = "http://api.travelpayouts.com/v2/prices/nearest-places-matrix?currency=usd&origin=LED&destination=HKT&show_to_affiliates=true&depart_date=2020-12&token=35120b8381d8f9ecea3fbd296b0697c3";
            string testPath = "http://api.travelpayouts.com/v2/prices/nearest-places-matrix?currency=usd&origin=BUD&destination=NYC&show_to_affiliates=true&token=35120b8381d8f9ecea3fbd296b0697c3";
            //string testPath = "http://api.travelpayouts.com/v1/prices/cheap?origin=NYC&destination=LAX&depart_date=2019-11&return_date=2019-12&token=35120b8381d8f9ecea3fbd296b0697c3";
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(testPath);
            var datas = JObject.Parse(response)["data"];
            string debugString = JObject.Parse(response)["data"].ToString();
            foreach (var data in datas)
            {
                Flight flight = new Flight();

                flight.ReturnDate = data["value"].ToString();
                flight.TripClass = data["trip_class"].ToString();
                flight.ShowToAffiliates = data["show_to_affiliates"].ToString();
                flight.ReturnDate = data["return_date"].ToString();
                flight.Origin = data["origin"].ToString();
                flight.NumberOfChanges = data["number_of_changes"].ToString();
                flight.Gate = data["gate"].ToString();
                flight.FoundAt = data["found_at"].ToString();
                flight.Distance = data["distance"].ToString();
                flight.Duration= data["duration"].ToString();
                flight.Destination = data["destination"].ToString();
                flight.DepartDate = data["depart_date"].ToString();
                flight.Actual = data["actual"].ToString();

                _context.Flights.Add(flight);
            }
            await _context.SaveChangesAsync();
            return await _context.Flights.ToListAsync();
            //return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
        }

        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(long id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        // PUT: api/Flights/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(long id, Flight flight)
        {
            if (id != flight.Id)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Flights
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("filter")]
        public async Task<ActionResult<IEnumerable<Flight>>> PostFlight(Flight flight)
        {



            string path = urlBuilder(flight.Origin,
                                        flight.Destination,
                                        flight.DepartDate,
                                        flight.ReturnDate,
                                        null,
                                        null,
                                        null);
            string testPath = "http://api.travelpayouts.com/v2/prices/nearest-places-matrix?currency=usd&origin=BUD&destination=NYC&show_to_affiliates=true&token=35120b8381d8f9ecea3fbd296b0697c3";

            HttpClient client = new HttpClient();

            string response = await client.GetStringAsync(path);
            var datas = JObject.Parse(response)["data"]["prices"];
            string debugString = datas.ToString();
            foreach (var data in datas)
            {
                Flight flightToGiveBack = new Flight();

                flightToGiveBack.Value = data["value"].ToString();
                flightToGiveBack.TripClass = data["trip_class"].ToString();
                flightToGiveBack.ShowToAffiliates = data["show_to_affiliates"].ToString();
                flightToGiveBack.ReturnDate = data["return_date"].ToString();
                flightToGiveBack.Origin = data["origin"].ToString();
                flightToGiveBack.NumberOfChanges = data["number_of_changes"].ToString();
                flightToGiveBack.Gate = data["gate"].ToString();
                flightToGiveBack.FoundAt = data["found_at"].ToString();
                flightToGiveBack.Distance = data["distance"].ToString();
                flightToGiveBack.Duration = data["duration"].ToString();
                flightToGiveBack.Destination = data["destination"].ToString();
                flightToGiveBack.DepartDate = data["depart_date"].ToString();
                flightToGiveBack.Actual = data["actual"].ToString();

                _context.Flights.Add(flightToGiveBack);
            }
            await _context.SaveChangesAsync();
            return await _context.Flights.ToListAsync();
            //return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
        }


        //public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        //{




        //}



        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(long id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return flight;
        }

        private bool FlightExists(long id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
