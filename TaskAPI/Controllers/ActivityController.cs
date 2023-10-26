using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Activity.API.ApiModel;
using Activity.API.Entities;
using Activity.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Activity.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ActivityTicketController : ControllerBase
{
    private readonly IActivityTicketRepository _ActivityTicketRepository;
    private readonly IConfiguration _config;


    public ActivityTicketController(IActivityTicketRepository ActivityTicketRepository, IConfiguration config)
    {
        _ActivityTicketRepository = ActivityTicketRepository;
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
        List<ActivityTicket> activities = await _ActivityTicketRepository.GetAllAsync();
        return Ok(activities);
    }

    [HttpGet]
    [Route("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        ActivityTicket? taskFromDb = await _ActivityTicketRepository.GetByIdAsync(id);
        return Ok(taskFromDb);
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create([FromBody] ActivityTicketApiModel model)
    {
        ActivityTicket activityTicketForCreate = model;
        bool isTaskCreated = await _ActivityTicketRepository.AddAsync(activityTicketForCreate);
        string message = isTaskCreated ? "ActivityTicketTicket created successfully" : "ActivityTicketTicket creation failed";
        if (isTaskCreated)
        {
            return Ok(message);
        }
        return BadRequest(message);
    }


    [HttpGet]
    [Route("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        ActivityTicket? taskForDelete = await _ActivityTicketRepository.GetByIdAsync(id);

        if (taskForDelete == null)
        {
            return BadRequest("ActivityTicketTicket not found");
        }

        bool isTaskDeleted = await _ActivityTicketRepository.DeleteAsync(id);

        string message = isTaskDeleted ? "ActivityTicketTicket deleted successfully" : "ActivityTicketTicket deletion failed";

        if (isTaskDeleted)
        {
            return Ok(message);
        }
        return BadRequest(message);
    }

    private string GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(_config["JwtSettings:Key"],
            _config["Jwt:Audience"],
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}