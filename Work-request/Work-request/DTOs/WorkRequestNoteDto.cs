namespace Work_request.DTOs;

public class WorkRequestNoteDto
{
    public int Id { get; set; }
    public int WorkRequestId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedDate { get; set; }
}
