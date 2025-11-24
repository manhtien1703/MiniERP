using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Province
{
	public int Id { get; set; }
	public string Code { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	[JsonIgnore]
	public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
