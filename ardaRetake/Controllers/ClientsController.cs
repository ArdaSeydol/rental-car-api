using ardaRetake.Models.DTOs;
using ardaRetake.Services;
using Microsoft.AspNetCore.Mvc;

namespace ardaRetake.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IDbService _dbService;

    public ClientsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> GetAppointmentById(int id)
    {
        try
        {
            var res = await _dbService.GetClientByIdAsync(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return NotFound(new { error = e.Message });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateNewClient([FromBody] CreateClientDto dto)
    {
        try
        {
            await _dbService.CreateNewClientAsync(dto);
            return Created("", new { message = "Client and rental created successfully." });
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Car with the given ID does not exist." =>
                    NotFound(new { error = "Car not found." }),

                "DateTo must be after DateFrom." =>
                    BadRequest(new { error = "Invalid rental dates." }),

                "Car price not found." =>
                    NotFound(new { error = "Car pricing information is missing." }),

                _ => StatusCode(500, new { error = "An unexpected error occurred." })
            };
        }
    }

    
    
}