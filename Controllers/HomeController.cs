using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FlightScheduleApp.Models;
using FlightScheduleApp.Services;

namespace FlightScheduleApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAviationStackService _aviationStackService;

    public HomeController(ILogger<HomeController> logger, IAviationStackService aviationStackService)
    {
        _logger = logger;
        _aviationStackService = aviationStackService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SearchFlights(string depIata, string? arrIata)
    {
        if (string.IsNullOrWhiteSpace(depIata))
        {
            return BadRequest("Departure IATA code is required.");
        }

        var response = await _aviationStackService.GetFlightsAsync(depIata, arrIata);
        
        if (response == null)
        {
            return StatusCode(500, "Error retrieving data from AviationStack API.");
        }

        return Json(response.Data);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
