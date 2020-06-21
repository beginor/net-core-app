using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace Beginor.GisHub.TileMap {

    public interface ITileMapRepository {

        IList<string> GetAllTileMapNames();

        JsonElement GetTileMapInfo(string tileName);

        Task<TileContent> GetTileContentAsync(string tileName, int level, int row, int col);

        DateTimeOffset? GetTileModifiedTime(string tileName, int level, int row, int col);

    }

}
