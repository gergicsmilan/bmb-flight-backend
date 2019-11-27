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
        public string urlBuilder(string origin, string destination, string currency, string tripClass, string sorting)
        {
            //string result = $"//api.travelpayouts.com/v1/prices/cheap?origin={origin}&destination={destination}&depart_date=2019-12&return_date=2019-12&sorting=price&token=35120b8381d8f9ecea3fbd296b0697c3";           
            StringBuilder UrlSb = new StringBuilder();
            UrlSb.Append("http://api.travelpayouts.com/v2/prices/nearest-places-matrix?");
            UrlSb.Append($"currency={currency}&");
            UrlSb.Append($"origin={origin}&");
            UrlSb.Append($"destination={destination}&");
            UrlSb.Append($"trip_class={tripClass}&");
            UrlSb.Append($"sorting={sorting}&");
            UrlSb.Append("token=35120b8381d8f9ecea3fbd296b0697c3");
            string result = UrlSb.ToString();
            return result;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<Flight>> GetFlights()
        {

            string path = urlBuilder("BUD", null, null, null, null);
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(path);
            var datas = JObject.Parse(response)["data"]["prices"];
            List
            foreach (JToken i in datas) {
                
                string desti = JObject.Parse(response)["data"]["prices"][i]["dtestination"].ToString();
            }
            //string name = data.Property("airline").Value.ToString
            //Flight flight = new Flight();
            //flight.Destination = data.Property("destination").Value.ToString();
            //_context.Flights.Add(flight);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);

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
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
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

        private bool FlightExists(long id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
