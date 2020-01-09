using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightApi.Util
{
    public static class Util
    {
        //method for building the url
        public static string UrlBuilder(string origin, string destination, string departDate, string returnDate, string currency, string tripClass, string sorting)
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
    }
}
