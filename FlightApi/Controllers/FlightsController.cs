using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightApi.Models;
using FlightApi.Context;
using System.Net.Http;
using System.Net.Http.Headers;
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

        // GET: /api
        [HttpGet]
        public async Task<ActionResult<Dictionary<string, string>>> GetPopularFlights()
        {
            HttpClient httpClient = new HttpClient();
            string response =
                await httpClient.GetStringAsync(
                    "http://api.travelpayouts.com/v1/city-directions?origin=BUD&currency=huf&token=3e08147c7f7449e03258a7b4daa9bdbf");

            List<Flight> flights = GetFlights(response);

            Dictionary<string, string> citiesWithPrices = await GetCitiesWithPrices(flights);

            return citiesWithPrices;
        }

        // GET: api/5
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

        // PUT: api/5
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

        // POST: api/filter
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

            string response;

            try
            {
                response = await client.GetStringAsync(path);
            }
            catch (HttpRequestException e)
            {
                return null;
            }

            JToken datas = JObject.Parse(response)["data"]["prices"];

            List<Flight> flights = new List<Flight>();

            long currentId = 1;

            foreach (JToken data in datas)
            {
                Flight flightToGiveBack = CreateFlight(data, currentId);

                flights.Add(flightToGiveBack);
                currentId++;
            }

            return flights;
        }

        // DELETE: api/5
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
            flight.Price = data["value"].ToString();
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

            UrlSb.Append("token=35120b8381d8f9ecea3fbd296b0697c3");
            string result = UrlSb.ToString();
            return result;
        }

        private async Task<Dictionary<string, string>> GetCitiesWithPrices(List<Flight> flights)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", "033ea2f472msh7c7d1b40c8172acp1c5f99jsn3001eb77120e");

            Dictionary<string, string> citiesWithPrices = new Dictionary<string, string>();

            int amountOfCities = flights.Count;
            int maxAmountOfCities = 6;

            var counter = amountOfCities >= maxAmountOfCities ? 0 : maxAmountOfCities - amountOfCities;

            for (int i = counter; i < maxAmountOfCities; i++)
            {
                (string city, string price) = await CreateCityWithPrice(httpClient, flights[i]);

                citiesWithPrices.Add(city, price);
            }

            return citiesWithPrices;
        }

        private async Task<(string, string)> CreateCityWithPrice(HttpClient httpClient, Flight flight)
        {
            var destination = flight.Destination;

            var resp = await httpClient.GetStringAsync("https://airport-info.p.rapidapi.com/airport?iata=" + destination);
            JObject jsonResp = JObject.Parse(resp);

            var fullCityName = jsonResp["location"].ToString();
            var city = fullCityName.Substring(0, fullCityName.IndexOf(','));
            var price = flight.Price;

            return (city, price);
        }

        private List<Flight> GetFlights(string response)
        {
            List<Flight> flights = new List<Flight>();

            JObject jsonResponse = JObject.Parse(response);
            foreach (JToken jToken in jsonResponse["data"].Children())
            {
                Flight flight = jToken.First.ToObject<Flight>();

                flights.Add(flight);
            }

            return flights;
        }

        private bool FlightExists(long id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
