using FlightScheduleApp.Models;

namespace FlightScheduleApp.Services
{
    public interface IAviationStackService
    {
        Task<FlightResponse?> GetFlightsAsync(string depIata, string? arrIata);
    }
}
