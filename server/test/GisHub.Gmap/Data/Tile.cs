namespace Beginor.GisHub.Gmap.Data; 

public class Tile {

    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public byte[] Content { get; set; }
    public string ContentType { get; set; }

    public Tile(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }

    public bool IsEmpty => Content == null || Content.Length == 0;

}