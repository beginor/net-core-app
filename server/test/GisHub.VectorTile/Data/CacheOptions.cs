namespace GisHub.VectorTile.Data {

    public class CacheOptions {
        public bool Enabled { get; set; } = true;
        public string Directory { get; set; } = "app_cache";
        public int Duration { get; set; } = 60 * 60 * 24;
    }

}
