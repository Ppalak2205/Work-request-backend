using System.ComponentModel.DataAnnotations;

namespace Work_request.DTOs;

public class AddNoteDto
{
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = string.Empty;
}
