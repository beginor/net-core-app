using System;

namespace Beginor.NetCoreApp.Common;

public class CommonOption {
    public CacheOption Cache { get; set; } = new();
    public StorageOption Storage { get; set; } = new();
}

public class CacheOption {
    public bool Enabled { get; set; } = false;
    public string Directory { get; set; } = "cache";
    public TimeSpan MemoryExpiration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan FileExpiration { get; set; } = TimeSpan.FromDays(1);
}

public class StorageOption {
    public string Directory { get; set; } = "storage";
    public string TempDirectory { get; set; } = "temp";
}
