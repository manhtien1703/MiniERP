using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Domain.Entities;

public class Warehouse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }

    public int ProvinceId { get; set; }
    public Province Province { get; set; } = null!;
}
