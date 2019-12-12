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

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            return await _context.Flights.ToListAsync();
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
            string path = UrlBuilder(flight.Origin,
                                        flight.Destination,
                                        flight.DepartDate,
                                        flight.ReturnDate,
                                        null,
                                        null,
                                        null);
            HttpClient client = new HttpClient();

            string response = await client.GetStringAsync(path);
            JToken datas = JObject.Parse(response)["data"]["prices"];

            _context.Flights.RemoveRange(_context.Flights);

            long currentId = 1;

            foreach (JToken data in datas)
            {
                Flight flightToGiveBack = CreateFlight(data, currentId);

                _context.Flights.Add(flightToGiveBack);
                currentId++;
            }

            await _context.SaveChangesAsync();

            return await _context.Flights.ToListAsync();
        }

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

        private Flight CreateFlight(JToken data, long currentId)
        {
            Flight flight = new Flight();

            flight.FlightId = currentId;
            flight.Value = data["value"].ToString();
            flight.TripClass = data["trip_class"].ToString();
            flight.ShowToAffiliates = data["show_to_affiliates"].ToString();
            flight.ReturnDate = data["return_date"].ToString();
            flight.Origin = data["origin"].ToString();
            flight.NumberOfChanges = data["number_of_changes"].ToString();
            flight.Gate = data["gate"].ToString();
            flight.FoundAt = data["found_at"].ToString();
            flight.Distance = data["distance"].ToString();
            flight.Duration = data["duration"].ToString();
            flight.Destination = data["destination"].ToString();
            flight.DepartDate = data["depart_date"].ToString();
            flight.Actual = data["actual"].ToString();

            return flight;
        }

        //method for building the url
        private string UrlBuilder(string origin, string destination, string departDate, string returnDate, string currency, string tripClass, string sorting)
        {
            StringBuilder UrlSb = new StringBuilder();
            UrlSb.Append("http://api.travelpayouts.com/v2/prices/nearest-places-matrix?");
            UrlSb.Append($"origin={origin}&");
            UrlSb.Append($"destination={destination}&");

            if (departDate != null && departDate != string.Empty)
            {
                UrlSb.Append($"depart_date={departDate}&");
            }

            if (returnDate != null && returnDate != string.Empty)
            {
                UrlSb.Append($"return_date={returnDate}&");
            }
            //UrlSb.Append($"trip_class={tripClass}&");
            //UrlSb.Append($"sorting={sorting}&");
            //UrlSb.Append($"currency={currency}&");

            UrlSb.Append("token=35120b8381d8f9ecea3fbd296b0697c3");
            string result = UrlSb.ToString();
            return result;
        }

        private bool FlightExists(long id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
