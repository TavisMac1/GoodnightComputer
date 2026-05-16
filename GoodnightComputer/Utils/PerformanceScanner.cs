using GoodnightComputer.Models;
using GoodnightComputer.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoodnightComputer.Utils;

public static class PerformanceScanner
{
    /// <summary>
    /// Samples all non-excluded processes over <paramref name="durationSeconds"/> seconds
    /// (polling every <paramref name="intervalMs"/> ms) and returns averaged CPU % and memory (MB)
    /// for each app, sorted by descending average CPU then memory.
    /// </summary>
    public static async Task<List<ApplicationRecord>> ScanAsync(
        int durationSeconds = 5,
        int intervalMs = 500,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        // pid -> (displayName, List<cpuSample>, List<memorySample>)
        var data = new Dictionary<int, (string Name, List<double> Cpu, List<double> Mem)>();

        // First-pass: seed the counters so we can compute CPU delta
        var counters = new Dictionary<int, (TimeSpan PrevTotal, DateTime PrevTime)>();
        foreach (var p in GetEligibleProcesses())
        {
            try
            {
                counters[p.Id] = (p.TotalProcessorTime, DateTime.UtcNow);
                string name = GetDisplayName(p);
                data[p.Id] = (name, new List<double>(), new List<double>());
            }
            catch { }
        }

        int samples = (int)Math.Ceiling((double)(durationSeconds * 1000) / intervalMs);
        int processorCount = Environment.ProcessorCount;

        for (int i = 0; i < samples; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int remaining = durationSeconds - (i * intervalMs / 1000);
            progress?.Report($"Scanning… {Math.Max(remaining, 1)}s remaining");

            await Task.Delay(intervalMs, cancellationToken);

            foreach (var p in GetEligibleProcesses())
            {
                try
                {
                    var now = DateTime.UtcNow;
                    var totalCpu = p.TotalProcessorTime;
                    double memMb = p.WorkingSet64 / (1024.0 * 1024.0);

                    if (counters.TryGetValue(p.Id, out var prev))
                    {
                        double elapsed = (now - prev.PrevTime).TotalSeconds;
                        double cpuDelta = (totalCpu - prev.PrevTotal).TotalSeconds;
                        double cpuPct = elapsed > 0
                            ? (cpuDelta / (processorCount * elapsed)) * 100.0
                            : 0;
                        cpuPct = Math.Clamp(cpuPct, 0, 100);

                        if (!data.ContainsKey(p.Id))
                        {
                            string name = GetDisplayName(p);
                            data[p.Id] = (name, new List<double>(), new List<double>());
                        }

                        data[p.Id].Cpu.Add(cpuPct);
                        data[p.Id].Mem.Add(memMb);
                    }

                    counters[p.Id] = (totalCpu, now);
                }
                catch { }
            }
        }

        var results = new List<ApplicationRecord>();
        foreach (var kvp in data)
        {
            var (name, cpuList, memList) = kvp.Value;
            if (cpuList.Count == 0) continue;

            double avgCpu = cpuList.Average();
            double avgMem = memList.Count > 0 ? memList.Average() : 0;

            results.Add(new ApplicationRecord(name)
            {
                CpuPercent = Math.Round(avgCpu, 2),
                MemoryMB = Math.Round(avgMem, 1),
                CPUUsage = $"{avgCpu:F2}%",
                MemoryUsage = $"{avgMem:F1} MB"
            });
        }

        // Merge duplicate display names (e.g. multiple instances of same app)
        var merged = results
            .GroupBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
            .Select(g => new ApplicationRecord(g.Key)
            {
                CpuPercent = Math.Round(g.Sum(x => x.CpuPercent), 2),
                MemoryMB = Math.Round(g.Sum(x => x.MemoryMB), 1),
                CPUUsage = $"{g.Sum(x => x.CpuPercent):F2}%",
                MemoryUsage = $"{g.Sum(x => x.MemoryMB):F1} MB"
            })
            .OrderByDescending(r => r.CpuPercent)
            .ThenByDescending(r => r.MemoryMB)
            .ToList();

        return merged;
    }

    private static IEnumerable<Process> GetEligibleProcesses()
    {
        return Process.GetProcesses().Where(p =>
        {
            string name = p.ProcessName;
            return !Constants.Constants.EXCLUSION_APPS.Contains(name, StringComparer.OrdinalIgnoreCase)
                && !UserExclusionState.Contains(name.ToLower());
        });
    }

    private static string GetDisplayName(Process p)
    {
        try
        {
            string? desc = p.MainModule?.FileVersionInfo?.FileDescription;
            return !string.IsNullOrWhiteSpace(desc) ? desc : p.ProcessName;
        }
        catch
        {
            return p.ProcessName;
        }
    }
}
