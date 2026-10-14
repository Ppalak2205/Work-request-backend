using Work_request.DTOs;
using Work_request.Models;

namespace Work_request.Services;

public interface IWorkRequestService
{
    Task<PagedResultDto<WorkRequestResponseDto>> GetWorkRequestsAsync(
        WorkRequestStatus? status,
        string? search,
        int page,
        int pageSize,
        string? userId = null,
        bool isAdmin = false);

    Task<WorkRequestResponseDto?> GetWorkRequestByIdAsync(
        int id,
        string? userId = null,
        bool isAdmin = false);

    Task<WorkRequestResponseDto> CreateWorkRequestAsync(
        CreateWorkRequestDto dto,
        string? userId = null);

    Task<WorkRequestResponseDto?> UpdateStatusAsync(
        int id,
        WorkRequestStatus status,
        string? userId = null,
        bool isAdmin = false);

    Task<WorkRequestNoteDto?> AddNoteAsync(
        int id,
        AddNoteDto dto,
        string? userId = null,
        bool isAdmin = false);
}
