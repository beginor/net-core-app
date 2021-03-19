define([
    'esri/Map',
    'esri/views/MapView',
    'esri/layers/FeatureLayer'
], function(Map, MapView, FeatureLayer) {
    'use strict';
    const map = new Map({
        basemap: 'streets'
    });
    const view = new MapView({
        map,
        zoom: 7,
        center: [113, 22],
        container: 'viewDiv',
    });
    view.when().then(_ => {
        const layer = new FeatureLayer({
            url: 'http://localhost:5000/gishub/rest/services/features/1609887224871030614/MapServer/0',
            // url: 'https://it.gdeei.cn/gisserver/rest/services/BaseLayers/gd_sar_labels/MapServer/2',
        });
        map.add(layer);
    });
});
