define(['esri/Map', 'esri/views/SceneView', 'esri/layers/TileLayer'], function (Map, SceneView, TileLayer) {
    const map = new Map({
        basemap: 'streets'
    });
    const view = new SceneView({
        container: 'viewDiv', // Reference to the scene div created in step 5
        map: map, // Reference to the map object created before the scene
        zoom: 7, // Sets zoom level based on level of detail (LOD)
        center: [113, 22] // Sets center point of view using longitude,latitude
    });
    const gaofen = new TileLayer({
        url: '/gishub/rest/services/tilemaps/1600409446764030070/MapServer'
    });
    view.map.add(gaofen);
    const label = new TileLayer({
        url: '/gishub/rest/services/tilemaps/1600409603668030071/MapServer'
    });
    view.map.add(label);
})
