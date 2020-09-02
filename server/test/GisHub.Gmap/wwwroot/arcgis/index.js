require([
    'esri/Map',
    'esri/views/MapView',
    'esri/views/SceneView',
    'esri/layers/WebTileLayer',
    'esri/layers/TileLayer',
    'esri/layers/WMTSLayer',
    'esri/layers/FeatureLayer',
    'esri/layers/BaseTileLayer',
    'esri/geometry/SpatialReference',
    'esri/layers/support/TileInfo',
    'esri/layers/VectorTileLayer'
], function (Map, MapView, SceneView, WebTileLayer, TileLayer, WMTSLayer, FeatureLayer, BaseTileLayer, SpatialReference, TileInfo, VectorTileLayer) {
    var baseUrl = `${location.protocol}//${location.host}`;
    // 卫星图
    var satellite = new WebTileLayer({
        urlTemplate: baseUrl + '/gmap/api/wmts/GDDOM/{level}/{row}/{col}',
        opacity: 1
    });
    // 注记
    var satelliteLabel = new WebTileLayer({
        urlTemplate: baseUrl + '/gmap/api/wmts/DOMZJ_2000_2015/{level}/{row}/{col}',
        opacity: 1
    });
    // 矢量切片图层
    var vectorTileLayer = new VectorTileLayer({
        style: {
            glyphs: "https://basemaps.arcgis.com/arcgis/rest/services/World_Basemap_v2/VectorTileServer/resources/fonts/{fontstack}/{range}.pbf",
            version: 8,
            sprite: "https://www.arcgis.com/sharing/rest/content/items/7675d44bb1e4428aa2c30a9b68f97822/resources/sprites/sprite",
            sources: {
                gd_bas_building: {
                    // url: "https://basemaps.arcgis.com/arcgis/rest/services/World_Basemap_v2/VectorTileServer",
                    type: "vector",
                    // scheme: 'tms',
                    tiles: [
                        // 'http://localhost:9080/geoserver/gwc/service/tms/1.0.0/postgis:gd_bas_building@EPSG:3857@pbf/{z}/{x}/{y}.pbf',
                        baseUrl + '/gmap/api/tms/postgis:gd_bas_building@EPSG:3857@pbf/{z}/{x}/{y}'
                    ]
                }
            },
            layers: [
                {
                    "id": "gd_bas_building",
                    "metadata": {
                        "title": "河流",
                        "mapbox:group": "river",
                        "type": "operational"
                    },
                    "type": "fill",
                    "source": "gd_bas_building",
                    "source-layer": "gd_bas_building",
                    "layout": {
                        "visibility": "visible"
                    },
                    "paint": {
                        "fill-antialias": true,
                        "fill-color": "#bae4bc",
                        "fill-outline-color": "#7bccc4",
                        "fill-opacity": 0.8
                    }
                }
            ]
        }
    });
    var map = new Map({
        spatialReference: SpatialReference.WebMercator,
        basemap: 'streets',
        layers: [
            // satellite,
            // satelliteLabel,
            vectorTileLayer
            // yztLayer,
            // featureLayer
            // wmtsLayer,
        ]
    });
    var view = new SceneView({
        container: 'mymap',
        map: map,
        zoom: 10,
        spatialReference: SpatialReference.WebMercator,
        center: {
            x: 113.4,
            y: 23.3
        }
        // center: [113, 22]
    });
    window._mapview = view;
    view.watch('center', (newVal) => {
        // var html = [];
        // html.push(JSON.stringify(newVal, null, '  '));
        // document.getElementById('mapCenter').innerHTML = html.join('<br/>');
    })
});
