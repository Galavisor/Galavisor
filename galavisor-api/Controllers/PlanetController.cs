using GalavisorApi.Services;
using Microsoft.AspNetCore.Mvc;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("planets")]
public class PlanetController(PlanetService planetService, AuthService authService) : ControllerBase
{
    private readonly PlanetService _planetService = planetService;
    private readonly AuthService _authService = authService;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<PlanetModel>>> getAllPlanets()
    {
        return Ok(await _planetService.GetAllPlanets());
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<PlanetModel>> getPlanetById(int id)
    {
        if (id < 0)
        {
            return StatusCode(500, new { message = "Get Failed", error = "Planet ID cannot be negative" });
        }
        return Ok(await _planetService.GetPlanetById(id));
    }

    [Authorize]
    [HttpGet("name/{planetName}")]
    public async Task<ActionResult<string>> getPlanetByName(string planetName)
    {
        return Ok(new { planet = await _planetService.GetPlanetByName(planetName) });
    }

    [Authorize]
    [HttpPost("weather/{id}")]
    public async Task<ActionResult<string>> getPlanetWeatherById(int id)
    {
        if (id < 0)
        {
            return StatusCode(500, new { message = "Get Failed", error = "Planet ID cannot be negative" });
        }
        return Ok(await _planetService.GetPlanetWeatherById(id));
    }

    [Authorize]
    [HttpPost("add")]
    public async Task<ActionResult<PlanetModel>> addPlanet([FromBody] PlanetModel request)
    {
        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if (await _authService.IsSubAdmin(GoogleSubject))
        {
            return Ok(await _planetService.AddPlanet(request));
        }
        else
        {
            return StatusCode(403, new { message = "Get failed", error = "You cannot access this command, only available to admins" });
        }

    }
    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<bool>> deletePlanet(int id)
    {

        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if (await _authService.IsSubAdmin(GoogleSubject))
        {
            if (id < 0)
            {
                return StatusCode(500, new { message = "Get Failed", error = "Planet ID cannot be negative" });
            }
            return Ok(await _planetService.DeletePlanet(id));
        }
        else
        {
            return StatusCode(403, new { message = "Get failed", error = "You cannot access this command, only available to admins" });
        }

    }

    [Authorize]
    [HttpPatch("update/{id}")]
    public async Task<ActionResult<bool>> updatePlanet([FromBody] PlanetModel planet, int id)
    {

        var GoogleSubject = HttpContext.User.FindFirst("sub")!.Value ?? "";
        if (await _authService.IsSubAdmin(GoogleSubject))
        {
            if (id < 0)
            {
                return StatusCode(500, new { message = "Get Failed", error = "Planet ID cannot be negative" });
            }
            return Ok(await _planetService.UpdatePlanet(planet));
        }
        else
        {
            return StatusCode(403, new { message = "Get failed", error = "You cannot access this command, only available to admins" });
        }

    }
}