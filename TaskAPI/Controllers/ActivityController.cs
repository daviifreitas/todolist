using Activity.API.ApiModel;
using Activity.API.Entities;
using Activity.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Activity.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ActivityTicketController : ControllerBase
{
    private readonly IActivityTicketRepository _activityTicketRepository;
    private readonly IConfiguration _config;


    public ActivityTicketController(IActivityTicketRepository ActivityTicketRepository, IConfiguration config)
    {
        _activityTicketRepository = ActivityTicketRepository;
        _config = config;
    }

    [HttpGet]
    [Route("GenerateToken")]
    [AllowAnonymous]
    public async Task<IActionResult> GetToken()
    {
        return Ok(GenerateToken());
    }

    [HttpGet]
    [Route("GetTasks")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<ActivityTicket> activities = await _activityTicketRepository.GetAllAsync();
            return Ok(activities);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            ActivityTicket? taskFromDb = await _activityTicketRepository.GetByIdAsync(id);
            return Ok(taskFromDb);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create([FromBody] ActivityTicketApiModel model)
    {
        try
        {
            ActivityTicket activityTicketForCreate = model;
            bool isTaskCreated = await _activityTicketRepository.AddAsync(activityTicketForCreate);
            string message = isTaskCreated ? "ActivityTicketTicket created successfully" : "ActivityTicketTicket creation failed";
            if (isTaskCreated)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch]
    [Route("Update")]
    public async Task<IActionResult> Update(int id, [FromBody] ActivityTicketApiModel modelForUpdate)
    {
        try
        {
            ActivityTicket? activityTicketForUpdate = await _activityTicketRepository.GetByIdAsync(id);

            if (activityTicketForUpdate is null)
            {
                return BadRequest("ActivityTicketTicket not found");
            }

            modelForUpdate.UpdateEntity(activityTicketForUpdate);
            bool hasActivityTicketUpdated = await _activityTicketRepository.UpdateAsync(activityTicketForUpdate);
            string message = hasActivityTicketUpdated ? "ActivityTicketTicket updated successfully" : "ActivityTicketTicket updation failed";

            return Ok(message);
        }
        catch (Exception exception)
        {
            return BadRequest("Any exception has throws in application");
        }
    }

    [HttpDelete]
    [Route("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            ActivityTicket? taskForDelete = await _activityTicketRepository.GetByIdAsync(id);

            if (taskForDelete == null)
            {
                return BadRequest("ActivityTicketTicket not found");
            }

            bool isTaskDeleted = await _activityTicketRepository.DeleteAsync(id);

            string message = isTaskDeleted ? "ActivityTicketTicket deleted successfully" : "ActivityTicketTicket deletion failed";

            if (isTaskDeleted)
            {
                return Ok(message);
            }
            return BadRequest(message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private string GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(_config["JwtSttings:Key"],
            _config["Jwt:Audience"],
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    private void SetActivityTicketAsDone(ActivityTicket tiket)
    {
        tiket.Status = ActivityStatus.Done;
    }
}