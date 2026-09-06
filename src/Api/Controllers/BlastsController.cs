using Commands;
using Commands.Dto;
using Microsoft.AspNetCore.Mvc;
using Query;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BlastsController : ControllerBase
{
    readonly ICommands _blastCommands;
    readonly IQuery _blastQueries;

    public BlastsController(ICommands blastCommands, IQuery blastQueries)
    {
        _blastCommands = blastCommands;
        _blastQueries = blastQueries;
    }

    [HttpPost("blasts")]
    public async Task<IActionResult> CreateBlastAsync(CreateBlastDto blast)
    {
        try
        {
            var blastId = await _blastCommands.CreateBlastComand(blast);
            return Ok(new { blastId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("blasts/{blastId}/holes")]
    public async Task<IActionResult> AddHoleCommand(Guid blastId, AddHoleDto hole)
    {
        try
        {
            await _blastCommands.AddHole(blastId, hole);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("blasts/{blastId}/holes/{holeId}/charge")]
    public async Task<IActionResult> ChargeHoleCommand(Guid blastId, Guid holeId)
    {
        try
        {
            await _blastCommands.ChargeHoleCommand(blastId, holeId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("/blasts/{blastId}/fire")]
    public async Task<IActionResult> FireBlastCommandAsync(Guid blastId)
    {
        try
        {
            await _blastCommands.FireBlastCommand(blastId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("blasts/{blastId}")]
    public async Task<IActionResult> GetBlastQuery(Guid blastId)
    {
        try
        {
            var blast = await _blastQueries.GetBlastQuery(blastId);
            return Ok(blast);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("/blasts/{blastId}/history")]
    public async Task<IActionResult> GetBlastHistoryQuery(Guid blastId)
    {
        try
        {
            var history = await _blastQueries.GetBlastHistoryQuery(blastId);
            return Ok(history);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
