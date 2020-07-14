using System.Collections.Concurrent;

namespace Beginor.GisHub.Slpk.Cache {

    public class SlpkCache : ConcurrentDictionary<long, SlpkCacheItem> { }

    public class SlpkCacheItem {
        public long Id { get; set; }
        public string Directory { get; set; }
    }

}
