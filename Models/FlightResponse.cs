using System.Text.Json.Serialization;

namespace FlightScheduleApp.Models
{
    public class FlightResponse
    {
        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }

        [JsonPropertyName("data")]
        public List<FlightData> Data { get; set; }
    }

    public class Pagination
    {
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class FlightData
    {
        [JsonPropertyName("flight_date")]
        public string FlightDate { get; set; }

        [JsonPropertyName("flight_status")]
        public string FlightStatus { get; set; }

        [JsonPropertyName("departure")]
        public Departure Departure { get; set; }

        [JsonPropertyName("arrival")]
        public Arrival Arrival { get; set; }

        [JsonPropertyName("airline")]
        public Airline Airline { get; set; }

        [JsonPropertyName("flight")]
        public Flight Flight { get; set; }
    }

    public class Departure
    {
        [JsonPropertyName("airport")]
        public string Airport { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("iata")]
        public string Iata { get; set; }

        [JsonPropertyName("icao")]
        public string Icao { get; set; }

        [JsonPropertyName("terminal")]
        public string Terminal { get; set; }

        [JsonPropertyName("gate")]
        public string Gate { get; set; }

        [JsonPropertyName("delay")]
        public int? Delay { get; set; }

        [JsonPropertyName("scheduled")]
        public DateTime? Scheduled { get; set; }

        [JsonPropertyName("estimated")]
        public DateTime? Estimated { get; set; }
    }

    public class Arrival
    {
        [JsonPropertyName("airport")]
        public string Airport { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("iata")]
        public string Iata { get; set; }

        [JsonPropertyName("icao")]
        public string Icao { get; set; }

        [JsonPropertyName("terminal")]
        public string Terminal { get; set; }

        [JsonPropertyName("gate")]
        public string Gate { get; set; }

        [JsonPropertyName("baggage")]
        public string Baggage { get; set; }

        [JsonPropertyName("delay")]
        public int? Delay { get; set; }

        [JsonPropertyName("scheduled")]
        public DateTime? Scheduled { get; set; }

        [JsonPropertyName("estimated")]
        public DateTime? Estimated { get; set; }
    }

    public class Airline
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("iata")]
        public string Iata { get; set; }

        [JsonPropertyName("icao")]
        public string Icao { get; set; }
    }

    public class Flight
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("iata")]
        public string Iata { get; set; }

        [JsonPropertyName("icao")]
        public string Icao { get; set; }
    }
}
