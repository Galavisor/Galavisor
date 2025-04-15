using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalavisorApi.Services;
using GalavisorApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace GalavisorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivityController : ControllerBase
{
    private readonly ActivityService _activityService;

    public ActivityController(ActivityService activityService)
    {
        _activityService = activityService;
    }

    [Authorize]
    [HttpGet("planet/{planetName}")]
    public async Task<ActionResult<List<ActivityModel>>> GetActivitiesByPlanet(string planetName)
    {
        var activities = await _activityService.GetActivitiesByPlanet(planetName);
        return Ok(activities);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<object>> AddActivity([FromBody] ActivityModel activity)
    {
        var (newActivity, isNewlyCreated) = await _activityService.AddActivity(activity);
        
        // Build a response with status information
        var response = new 
        {
            activity = newActivity,
            status = new
            {
                isNewlyCreated
            }
        };
        
        return StatusCode(201, response);
    }

    [Authorize]
    [HttpPut("{currentName}")]
    public async Task<IActionResult> UpdateActivity(string currentName, [FromBody] string newName)
    {
        var success = await _activityService.UpdateActivity(currentName, newName);
        if (!success)
            return NotFound();
        
        return Ok(new { message = $"Activity '{currentName}' updated to '{newName}' successfully" });
    }

    [Authorize]
    [HttpDelete("{activityName}")]
    public async Task<IActionResult> DeleteActivity(string activityName)
    {
        var success = await _activityService.DeleteActivity(activityName);
        if (!success)
            return NotFound();
        
        return Ok(new { message = $"Activity '{activityName}' deleted successfully" });
    }

    [Authorize]
    [HttpPost("link")]
    public async Task<ActionResult<object>> LinkActivityToPlanet([FromBody] ActivityModel activity)
    {
        if (string.IsNullOrEmpty(activity.Name) || string.IsNullOrEmpty(activity.PlanetName))
        {
            return BadRequest("Activity name and planet name are required");
        }
        
        try
        {
            bool isNewlyLinked = await _activityService.LinkActivityToPlanet(activity.Name, activity.PlanetName);
            
            // Build a response with status information
            var response = new 
            {
                activity = new ActivityModel 
                { 
                    Name = activity.Name,
                    PlanetName = activity.PlanetName
                },
                status = new
                {
                    isNewlyLinked
                }
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<ActivityModel>>> GetAllActivities()
    {
        var activity = await _activityService.GetAllActivities();
        return Ok(activity);
    }
} 