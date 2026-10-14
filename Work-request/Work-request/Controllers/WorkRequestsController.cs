using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Work_request.DTOs;
using Work_request.Models;
using Work_request.Services;

namespace Work_request.Controllers;

[Authorize]
[ApiController]
[Route("api/work-requests")]
public class WorkRequestsController : ControllerBase
{
    private readonly IWorkRequestService _workRequestService;

    public WorkRequestsController(IWorkRequestService workRequestService)
    {
        _workRequestService = workRequestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] WorkRequestStatus? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1)
        {
            ModelState.AddModelError(nameof(page), "Page must be at least 1.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            ModelState.AddModelError(nameof(pageSize), "Page size must be between 1 and 100.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _workRequestService.GetWorkRequestsAsync(status, search, page, pageSize, userId, isAdmin);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _workRequestService.GetWorkRequestByIdAsync(id, userId, isAdmin);
        if (result is null)
        {
            return WorkRequestNotFound(id);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _workRequestService.CreateWorkRequestAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateStatusDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _workRequestService.UpdateStatusAsync(id, dto.Status!.Value, userId, isAdmin);
        if (result is null)
        {
            return WorkRequestNotFound(id);
        }

        return Ok(result);
    }

    [HttpPost("{id:int}/notes")]
    public async Task<IActionResult> AddNote([FromRoute] int id, [FromBody] AddNoteDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _workRequestService.AddNoteAsync(id, dto, userId, isAdmin);
        if (result is null)
        {
            return WorkRequestNotFound(id);
        }

        return Ok(result);
    }

    private ObjectResult WorkRequestNotFound(int id) =>
        Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Not Found",
            detail: $"Work request with ID {id} was not found.");
}
