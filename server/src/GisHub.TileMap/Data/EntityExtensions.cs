namespace Beginor.GisHub.TileMap.Data {

    public static class EntityExtensions {

        public static double[] GetExtent(this TileMapEntity entity) {
            if (entity.MinLongitude.HasValue && entity.MaxLongitude.HasValue
                && entity.MinLatitude.HasValue && entity.MaxLatitude.HasValue
            ) {
                return new [] {
                    entity.MinLongitude.Value,
                    entity.MinLatitude.Value,
                    entity.MaxLongitude.Value,
                    entity.MaxLatitude.Value
                };
            }
            return null;
        }

    }

}
