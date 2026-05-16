namespace GoodnightComputer.Models;
public class ApplicationRecord
{
    public string Name { get; set; }
    public string? CPUUsage { get; set; }
    public string? MemoryUsage { get; set; }
    public string? DiskUsage { get; set; }

    public double CpuPercent { get; set; }
    public double MemoryMB { get; set; }

    public ApplicationRecord(string name)
    {
        Name = name;
    }
}
