require([
    'esri/Map',
    'esri/views/MapView',
    'esri/views/SceneView',
    'esri/layers/VectorTileLayer',
    'esri/geometry/SpatialReference'
], function(Map, MapView, SceneView, VectorTileLayer, SpatialReference) {
    const baseUrl = `${location.protocol}//${location.host}/gmap/api/tms`;
    var map = new Map({
        basemap: 'satellite'
    });
    var mapview = new SceneView({
        map,
        center: {
            x: 113.4,
            y: 23.3
        },
        zoom: 6,
        container: 'mymap'
    });
    Object.assign(window, { _mapview: mapview });

    async function addVectorLayer() {
        const res = await fetch('vectortile.json' + '?_t=' + Date.now());
        let txt = await res.text();
        txt = txt.replace("BASE_URL", baseUrl);
        const json = JSON.parse(txt);
        const layer = new VectorTileLayer({
            style: json,
            visible: true
        });
        mapview.map.layers.add(layer);
    }
    addVectorLayer().catch(ex => { console.error(ex); });
});
