namespace Web.Models;

public class CreateWarehouseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int ProvinceId { get; set; }
}

