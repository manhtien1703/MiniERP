namespace Web.Models.Responses;

public class WarehouseResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int ProvinceId { get; set; }
    public string? ProvinceName { get; set; }
}

