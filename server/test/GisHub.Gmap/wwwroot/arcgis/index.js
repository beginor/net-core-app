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
  'esri/layers/support/TileInfo'
], function (Map, MapView, SceneView, WebTileLayer, TileLayer, WMTSLayer,FeatureLayer, BaseTileLayer, SpatialReference, TileInfo) {
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

  var map = new Map({
    spatialReference: SpatialReference.WebMercator,
    basemap: 'streets',
    layers: [
     satellite,
     satelliteLabel
      // yztLayer,
      // featureLayer
      // wmtsLayer,
    ]
  });
  var view = new SceneView({
    container: 'mymap',
    map: map,
    zoom: 7,
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
