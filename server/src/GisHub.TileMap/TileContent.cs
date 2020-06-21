namespace Beginor.GisHub.TileMap {

    public class TileContent {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public static TileContent Empty { get; set; } = new TileContent { Content = new byte[0], ContentType = string.Empty };
    }

}
