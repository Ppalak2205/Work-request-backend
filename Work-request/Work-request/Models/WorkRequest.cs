using System.ComponentModel.DataAnnotations;

namespace Work_request.Models;

public class WorkRequest
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ClientName { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public WorkRequestPriority Priority { get; set; } = WorkRequestPriority.Medium;

    public WorkRequestStatus Status { get; set; } = WorkRequestStatus.New;

    public DateTime DueDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    public string? CreatedByUserId { get; set; }

    public ApplicationUser? CreatedByUser { get; set; }

    public ICollection<WorkRequestNote> Notes { get; set; } = new List<WorkRequestNote>();
}
