using System;

namespace Beginor.GisHub.Gmap.Data;

public class Tile {

    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = string.Empty;

    public Tile(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }

    public bool IsEmpty => Content.Length == 0;

}
