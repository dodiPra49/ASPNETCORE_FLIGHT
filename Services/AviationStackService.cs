using System.Text.Json;
using FlightScheduleApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlightScheduleApp.Services
{
    public class AviationStackService : IAviationStackService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AviationStackService> _logger;

        public AviationStackService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache, ILogger<AviationStackService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
        }

        public async Task<FlightResponse?> GetFlightsAsync(string depIata, string? arrIata, string? airlineName = null)
        {
            // Build cache key based on query parameters
            string cacheKey = $"flights_{depIata}_{arrIata ?? "any"}_{airlineName ?? "any"}";

            // Check if we have cached data to save API limits
            if (_cache.TryGetValue(cacheKey, out FlightResponse? cachedResponse))
            {
                _logger.LogInformation("Returning flights from cache for key: {CacheKey}", cacheKey);
                return cachedResponse;
            }

            string apiKey = _configuration["AviationStack:ApiKey"] ?? string.Empty;
            
            // Note: AviationStack free tier uses HTTP, not HTTPS
            string requestUri = $"flights?access_key={apiKey}&dep_iata={depIata.ToLower()}";
            
            if (!string.IsNullOrEmpty(arrIata))
            {
                requestUri += $"&arr_iata={arrIata.ToLower()}";
            }
            
            if (!string.IsNullOrEmpty(airlineName))
            {
                // Note: AviationStack accepts airline_name. For better matching, it might be URL encoded.
                requestUri += $"&airline_name={Uri.EscapeDataString(airlineName)}";
            }

            try
            {
                _logger.LogInformation("Fetching flights from AviationStack API: {RequestUri}", requestUri);
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var flightResponse = JsonSerializer.Deserialize<FlightResponse>(content);

                // Cache the response for 10 minutes if valid
                if (flightResponse != null && flightResponse.Data != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    _cache.Set(cacheKey, flightResponse, cacheEntryOptions);
                }

                return flightResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data from AviationStack");
                return null;
            }
        }
    }
}
