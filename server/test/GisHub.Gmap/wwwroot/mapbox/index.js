import { Map, NavigationControl } from 'mapbox-gl';

const baseUrl = `${location.protocol}//${location.host}`;

// 卫星图
const satellite = './satellite.json';
// 道路图
const road = './road.json';

const map = new Map({
  container: 'themap',
  accessToken: 'pk.eyJ1IjoiYmVnaW5vciIsImEiOiJja2pmMWNkbXg4azcyMzVwZHFraGc1cjYzIn0.xo0073v7zQfAicAPsr0CHQ',
  style: satellite,
  center: [113.4, 23.3],
  zoom: 6
});

map.addControl(
    new NavigationControl({showCompass: true, showZoom: true}),
    'top-left'
);

document.getElementById('layer-road').addEventListener('click', e => {
  map.setStyle(road, false);
});

document.getElementById('layer-satellite').addEventListener('click', e => {
  map.setStyle(satellite, false);
});
