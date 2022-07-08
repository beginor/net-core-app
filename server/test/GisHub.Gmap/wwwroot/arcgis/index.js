import Map from '@arcgis/core/Map.js';
import MapView from '@arcgis/core/views/MapView.js';
import WebTileLayer from '@arcgis/core/layers/WebTileLayer.js';
import SpatialReference from '@arcgis/core/geometry/SpatialReference.js';

const baseUrl = `${location.protocol}//${location.host}`;
// 卫星图
const satellite = new WebTileLayer({
  urlTemplate: baseUrl + '/gmap/api/wmts/GDDOM/{level}/{row}/{col}',
  opacity: 1
});
// 卫星图注记
const satelliteLabel = new WebTileLayer({
  urlTemplate: baseUrl + '/gmap/api/wmts/DOMZJ_2000_2015/{level}/{row}/{col}',
  opacity: 1
});
// 道路图
const road = new WebTileLayer({
  urlTemplate: baseUrl + '/gmap/api/wmts/GD_2018DLG/{level}/{row}/{col}',
  opacity: 1
});
// 道路图注记
const roadLabel = new WebTileLayer({
  urlTemplate: baseUrl + '/gmap/api/wmts/GD_2018DLGZJ/{level}/{row}/{col}',
  opacity: 1
});

const map = new Map({
  spatialReference: SpatialReference.WebMercator,
  // basemap: 'satellite',
  layers: [
    satellite,
    satelliteLabel
    // yztLayer,
    // featureLayer
    // wmtsLayer,
  ]
});
const view = new MapView({
  container: 'themap',
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
  // const html = [];
  // html.push(JSON.stringify(newVal, null, '  '));
  // document.getElementById('mapCenter').innerHTML = html.join('<br/>');
});

document.getElementById('layer-road').addEventListener('click', e => {
  map.removeMany([satellite, satelliteLabel]);
  map.addMany([road, roadLabel]);
});

document.getElementById('layer-satellite').addEventListener('click', e => {
  map.removeMany([road, roadLabel]);
  map.addMany([satellite, satelliteLabel]);
});
