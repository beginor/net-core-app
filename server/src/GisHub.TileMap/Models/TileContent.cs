namespace Beginor.GisHub.TileMap.Models {

    public class TileContentModel {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public static TileContentModel Empty { get; set; } = new TileContentModel {
            Content = new byte[0], ContentType = string.Empty
        };
    }

}
