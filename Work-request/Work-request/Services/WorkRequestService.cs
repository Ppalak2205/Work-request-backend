using Microsoft.EntityFrameworkCore;
using Work_request.Data;
using Work_request.DTOs;
using Work_request.Models;

namespace Work_request.Services;

public class WorkRequestService : IWorkRequestService
{
    private readonly AppDbContext _context;

    public WorkRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<WorkRequestResponseDto>> GetWorkRequestsAsync(
        WorkRequestStatus? status,
        string? search,
        int page,
        int pageSize,
        string? userId = null,
        bool isAdmin = false)
    {
        var query = _context.WorkRequests
            .AsNoTracking()
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(r => r.CreatedByUserId == userId);
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(r => r.Title.Contains(term) || r.ClientName.Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new WorkRequestResponseDto
            {
                Id = r.Id,
                Title = r.Title,
                ClientName = r.ClientName,
                Description = r.Description,
                Priority = r.Priority,
                Status = r.Status,
                DueDate = r.DueDate,
                CreatedDate = r.CreatedDate,
                UpdatedDate = r.UpdatedDate,
                CreatedByUserId = r.CreatedByUserId,
                CreatedByUserName = r.CreatedByUser != null ? (r.CreatedByUser.FullName ?? r.CreatedByUser.UserName) : null,
                NotesCount = r.Notes.Count
            })
            .ToListAsync();

        return new PagedResultDto<WorkRequestResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<WorkRequestResponseDto?> GetWorkRequestByIdAsync(
        int id,
        string? userId = null,
        bool isAdmin = false)
    {
        var query = _context.WorkRequests
            .AsNoTracking()
            .Include(r => r.CreatedByUser)
            .Include(r => r.Notes)
                .ThenInclude(n => n.CreatedByUser)
            .Where(r => r.Id == id);

        if (!isAdmin)
        {
            query = query.Where(r => r.CreatedByUserId == userId);
        }

        var request = await query.FirstOrDefaultAsync();

        if (request is null)
        {
            return null;
        }

        return new WorkRequestResponseDto
        {
            Id = request.Id,
            Title = request.Title,
            ClientName = request.ClientName,
            Description = request.Description,
            Priority = request.Priority,
            Status = request.Status,
            DueDate = request.DueDate,
            CreatedDate = request.CreatedDate,
            UpdatedDate = request.UpdatedDate,
            CreatedByUserId = request.CreatedByUserId,
            CreatedByUserName = request.CreatedByUser != null ? (request.CreatedByUser.FullName ?? request.CreatedByUser.UserName) : null,
            NotesCount = request.Notes.Count,
            Notes = request.Notes
                .OrderBy(n => n.CreatedDate)
                .Select(n => new WorkRequestNoteDto
                {
                    Id = n.Id,
                    WorkRequestId = n.WorkRequestId,
                    Content = n.Content,
                    CreatedByUserId = n.CreatedByUserId,
                    CreatedByUserName = n.CreatedByUser != null ? (n.CreatedByUser.FullName ?? n.CreatedByUser.UserName) : null,
                    CreatedDate = n.CreatedDate
                }).ToList()
        };
    }

    public async Task<WorkRequestResponseDto> CreateWorkRequestAsync(CreateWorkRequestDto dto, string? userId = null)
    {
        var now = DateTime.UtcNow;
        var entity = new WorkRequest
        {
            Title = dto.Title.Trim(),
            ClientName = dto.ClientName.Trim(),
            Description = dto.Description.Trim(),
            Priority = dto.Priority!.Value,
            Status = dto.Status!.Value,
            DueDate = dto.DueDate!.Value,
            CreatedDate = now,
            UpdatedDate = now,
            CreatedByUserId = userId
        };

        _context.WorkRequests.Add(entity);
        await _context.SaveChangesAsync();

        return new WorkRequestResponseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            ClientName = entity.ClientName,
            Description = entity.Description,
            Priority = entity.Priority,
            Status = entity.Status,
            DueDate = entity.DueDate,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate,
            CreatedByUserId = entity.CreatedByUserId,
            NotesCount = 0,
            Notes = new List<WorkRequestNoteDto>()
        };
    }

    public async Task<WorkRequestResponseDto?> UpdateStatusAsync(
        int id,
        WorkRequestStatus status,
        string? userId = null,
        bool isAdmin = false)
    {
        var query = _context.WorkRequests
            .Include(r => r.CreatedByUser)
            .Include(r => r.Notes)
                .ThenInclude(n => n.CreatedByUser)
            .Where(r => r.Id == id);

        if (!isAdmin)
        {
            query = query.Where(r => r.CreatedByUserId == userId);
        }

        var entity = await query.FirstOrDefaultAsync();

        if (entity is null)
        {
            return null;
        }

        entity.Status = status;
        entity.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new WorkRequestResponseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            ClientName = entity.ClientName,
            Description = entity.Description,
            Priority = entity.Priority,
            Status = entity.Status,
            DueDate = entity.DueDate,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate,
            CreatedByUserId = entity.CreatedByUserId,
            CreatedByUserName = entity.CreatedByUser != null ? (entity.CreatedByUser.FullName ?? entity.CreatedByUser.UserName) : null,
            NotesCount = entity.Notes.Count,
            Notes = entity.Notes
                .OrderBy(n => n.CreatedDate)
                .Select(n => new WorkRequestNoteDto
                {
                    Id = n.Id,
                    WorkRequestId = n.WorkRequestId,
                    Content = n.Content,
                    CreatedByUserId = n.CreatedByUserId,
                    CreatedByUserName = n.CreatedByUser != null ? (n.CreatedByUser.FullName ?? n.CreatedByUser.UserName) : null,
                    CreatedDate = n.CreatedDate
                }).ToList()
        };
    }

    public async Task<WorkRequestNoteDto?> AddNoteAsync(
        int id,
        AddNoteDto dto,
        string? userId = null,
        bool isAdmin = false)
    {
        var query = _context.WorkRequests.Where(r => r.Id == id);
        if (!isAdmin)
        {
            query = query.Where(r => r.CreatedByUserId == userId);
        }

        var request = await query.FirstOrDefaultAsync();
        if (request is null)
        {
            return null;
        }

        var note = new WorkRequestNote
        {
            WorkRequestId = id,
            Content = dto.Content.Trim(),
            CreatedByUserId = userId,
            CreatedDate = DateTime.UtcNow
        };

        _context.WorkRequestNotes.Add(note);
        request.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var author = userId != null ? await _context.Users.FindAsync(userId) : null;

        return new WorkRequestNoteDto
        {
            Id = note.Id,
            WorkRequestId = note.WorkRequestId,
            Content = note.Content,
            CreatedByUserId = note.CreatedByUserId,
            CreatedByUserName = author != null ? (author.FullName ?? author.UserName) : null,
            CreatedDate = note.CreatedDate
        };
    }
}
