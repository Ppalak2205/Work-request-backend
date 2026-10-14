using Work_request.Models;

namespace Work_request.DTOs;

public class WorkRequestResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkRequestPriority Priority { get; set; }
    public WorkRequestStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public int NotesCount { get; set; }
    public List<WorkRequestNoteDto> Notes { get; set; } = new();
}
