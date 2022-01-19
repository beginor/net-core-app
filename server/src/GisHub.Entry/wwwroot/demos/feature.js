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
        const layer1 = new FeatureLayer({
            // url: 'http://localhost:5000/gishub/rest/services/features/1609887224871030614/MapServer/0',
            url: 'https://it.gdeei.cn/gisserver/rest/services/BaseLayers/gd_sar_labels/MapServer/2',
            renderer: {
                type: 'simple',
                symbol: {
                    type: 'simple-marker',
                    size: 6,
                    color: 'red',
                    outline: {
                        with: 1,
                        color: 'black'
                    }
                }
            }
        });
        map.add(layer1);
        const layer2 = new FeatureLayer({
            url: 'http://localhost:5000/gishub/rest/services/features/1609887224871030614/MapServer/0',
            // url: 'https://it.gdeei.cn/gisserver/rest/services/BaseLayers/gd_sar_labels/MapServer/2',
            renderer: {
                type: 'simple',
                symbol: {
                    type: 'simple-marker',
                    size: 6,
                    color: 'green',
                    outline: {
                        with: 1,
                        color: 'black'
                    }
                }
            }
        });
        map.add(layer2);
    });
});
