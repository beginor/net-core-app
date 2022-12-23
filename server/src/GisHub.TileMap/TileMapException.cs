using System;
using System.Runtime.Serialization;

namespace Beginor.GisHub.TileMap;

[Serializable]
public class TileNotFoundException : Exception {

    public TileNotFoundException() { }

    public TileNotFoundException(string message) : base(message) { }

    public TileNotFoundException(string message, Exception inner) : base(message, inner) { }

    protected TileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

}
