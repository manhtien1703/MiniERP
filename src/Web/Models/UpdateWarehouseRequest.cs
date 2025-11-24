namespace Web.Models;

public class UpdateWarehouseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int ProvinceId { get; set; }
}

