using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Devices
{

    [Column("DeviceId")]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Location { get; set; }

    public string Type { get; set; }

    public string? AssetId { get; set; } = null;



}