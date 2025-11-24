using System.Collections.Generic;

namespace Web.Models.Responses;

public class ErrorResponse
{
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string? Instance { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}

