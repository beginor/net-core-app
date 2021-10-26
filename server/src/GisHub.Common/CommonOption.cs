using System;

namespace Beginor.GisHub.Common {

    public class CommonOption {
        public CacheOption Cache { get; set; } = new();
        public OutputOption Output { get; set; } = new();
    }

    public class CacheOption {
        public bool Enabled { get; set; } = false;
        public string Directory { get; set; } = "app_cache";
        public TimeSpan MemoryExpiration { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan FileExpiration { get; set; } = TimeSpan.FromDays(1);
    }

    public class OutputOption {
        public bool Compress { get; set; } = false;
        public CoordinateOption Coordinate { get; set; } = new();
    }

    public class CoordinateOption {
        public int Digits { get; set; } = 6;
    }

}
