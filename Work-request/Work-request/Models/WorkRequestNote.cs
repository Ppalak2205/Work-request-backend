using System.ComponentModel.DataAnnotations;

namespace Work_request.Models;

public class WorkRequestNote
{
    public int Id { get; set; }

    public int WorkRequestId { get; set; }

    public WorkRequest? WorkRequest { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? CreatedByUserId { get; set; }

    public ApplicationUser? CreatedByUser { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
