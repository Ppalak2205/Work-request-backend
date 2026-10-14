using System.ComponentModel.DataAnnotations;
using Work_request.Models;

namespace Work_request.DTOs;

public class UpdateStatusDto
{
    [Required]
    [EnumDataType(typeof(WorkRequestStatus))]
    public WorkRequestStatus? Status { get; set; }
}
