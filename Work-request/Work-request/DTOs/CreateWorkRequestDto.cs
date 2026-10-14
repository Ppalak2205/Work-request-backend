using System.ComponentModel.DataAnnotations;
using Work_request.Models;

namespace Work_request.DTOs;

public class CreateWorkRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ClientName { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    // Nullable so [Required] can detect a missing value; non-nullable value types always bind a default.
    [Required]
    [EnumDataType(typeof(WorkRequestPriority))]
    public WorkRequestPriority? Priority { get; set; }

    [Required]
    [EnumDataType(typeof(WorkRequestStatus))]
    public WorkRequestStatus? Status { get; set; }

    [Required]
    public DateTime? DueDate { get; set; }
}
