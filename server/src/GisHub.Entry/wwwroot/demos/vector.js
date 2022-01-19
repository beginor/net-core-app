
require([
    'esri/config',
    'esri/Map',
    'esri/views/MapView',
    'esri/layers/VectorTileLayer'
], function (Map, MapView, VectorTileLayer) {
    const map = new Map({
        // basemap: 'streets'
    });
    const view = new MapView({
        container: 'viewDiv', // Reference to the scene div created in step 5
        map: map, // Reference to the map object created before the scene
        zoom: 7, // Sets zoom level based on level of detail (LOD)
        center: [113, 22] // Sets center point of view using longitude,latitude
    });
    Object.assign(window, { _mapview: view });
    if (!!style) {
        const vector = new VectorTileLayer({
            style: style
        });
        map.add(vector);
    }
})
