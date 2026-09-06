using Commands;
using Commands.Dto;
using Microsoft.AspNetCore.Mvc;
using Query;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BlastsController : ControllerBase
{
    private readonly ILogger<BlastsController> _logger;
    readonly ICommands _blastCommands;
    readonly IQuery _blastQueries;

    public BlastsController(ILogger<BlastsController> logger, ICommands blastCommands, IQuery blastQueries)
    {
        _logger = logger;
        _blastCommands = blastCommands;
        _blastQueries = blastQueries;
    }

    [HttpPost("blasts")]
    public async Task<IActionResult> CreateBlastAsync(CreateBlastDto blast)
    {
        try
        {
            _logger.LogInformation("Creating a new blast with name: {BlastName}", blast.Name);
            var blastId = await _blastCommands.CreateBlastComand(blast);
            _logger.LogInformation("Blast created successfully with ID: {BlastId}", blastId);
            return Ok(new { blastId });
        }
        catch (Exception)
        {
            _logger.LogError("An unexpected error occurred while creating a new blast with name: {BlastName}", blast.Name);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("blasts/{blastId}/holes")]
    public async Task<IActionResult> AddHoleCommand(Guid blastId, AddHoleDto hole)
    {
        try
        {
            _logger.LogInformation("Adding a new hole to blast with ID: {BlastId}", blastId);
            var holeId = await _blastCommands.AddHole(blastId, hole);
            _logger.LogInformation("Hole added successfully with ID: {HoleId} to blast with ID: {BlastId}", holeId, blastId);
            return Ok(new { holeId });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Failed to add hole to blast with ID: {BlastId}. Reason: {Reason}", blastId, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            _logger.LogError("An unexpected error occurred while adding a new hole to blast with ID: {BlastId}", blastId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPut("blasts/{blastId}/holes/{holeId}/charge")]
    public async Task<IActionResult> ChargeHoleCommand(Guid blastId, Guid holeId)
    {
        try
        {
            _logger.LogInformation("Charging hole with ID: {HoleId} in blast with ID: {BlastId}", holeId, blastId);
            await _blastCommands.ChargeHoleCommand(blastId, holeId);
            _logger.LogInformation("Hole with ID: {HoleId} in blast with ID: {BlastId} charged successfully", holeId, blastId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Failed to charge hole with ID: {HoleId} in blast with ID: {BlastId}. Reason: {Reason}", holeId, blastId, ex.Message);
            return NotFound(ex.Message);
        }
        catch (InvalidHoleStatusException ex)
        {
            _logger.LogWarning("Failed to charge hole with ID: {HoleId} in blast with ID: {BlastId}. Reason: {Reason}", holeId, blastId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            _logger.LogError("An unexpected error occurred while charging hole with ID: {HoleId} in blast with ID: {BlastId}", holeId, blastId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("/blasts/{blastId}/fire")]
    public async Task<IActionResult> FireBlastCommandAsync(Guid blastId)
    {
        try
        {
            _logger.LogInformation("Firing blast with ID: {BlastId}", blastId);
            await _blastCommands.FireBlastCommand(blastId);
            _logger.LogInformation("Blast with ID: {BlastId} fired successfully", blastId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Failed to fire blast with ID: {BlastId}. Reason: {Reason}", blastId, ex.Message);
            return NotFound(ex.Message);
        }
        catch (InvalidHoleStatusException ex)
        {
            _logger.LogWarning("Failed to fire blast with ID: {BlastId}. Reason: {Reason}", blastId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (AlreadyBlastedException ex)
        {
            _logger.LogWarning("Failed to fire blast with ID: {BlastId}. Reason: {Reason}", blastId, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            _logger.LogError("An unexpected error occurred while firing blast with ID: {BlastId}", blastId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("blasts/{blastId}")]
    public async Task<IActionResult> GetBlastQuery(Guid blastId)
    {
        try
        {
            _logger.LogInformation("Retrieving blast with ID: {BlastId}", blastId);
            var blast = await _blastQueries.GetBlastQuery(blastId);
            _logger.LogInformation("Blast with ID: {BlastId} retrieved successfully", blastId);
            return Ok(blast);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Failed to retrieve blast with ID: {BlastId}. Reason: {Reason}", blastId, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            _logger.LogError("An unexpected error occurred while retrieving blast with ID: {BlastId}", blastId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("/blasts/{blastId}/history")]
    public async Task<IActionResult> GetBlastHistoryQuery(Guid blastId)
    {
        try
        {
            _logger.LogInformation("Retrieving history for blast with ID: {BlastId}", blastId);
            var history = await _blastQueries.GetBlastHistoryQuery(blastId);
            _logger.LogInformation("History for blast with ID: {BlastId} retrieved successfully", blastId);
            return Ok(history);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Failed to retrieve history for blast with ID: {BlastId}. Reason: {Reason}", blastId, ex.Message);
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            _logger.LogError("An unexpected error occurred while retrieving history for blast with ID: {BlastId}", blastId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
