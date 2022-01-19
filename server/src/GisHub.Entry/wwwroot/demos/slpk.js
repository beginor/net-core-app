define([
    'esri/Map',
    'esri/views/SceneView',
    'esri/layers/IntegratedMeshLayer'
], function (Map, SceneView, IntegratedMeshLayer) {
  const map = new Map({
    basemap: 'satellite',
    ground: 'world-elevation'
  });
  const view = new SceneView({
    container: 'viewDiv', // Reference to the scene div created in step 5
    map: map, // Reference to the map object created before the scene
    zoom: 7, // Sets zoom level based on level of detail (LOD)
    center: [113, 22] // Sets center point of view using longitude,latitude
  });

  function getHeaders() {
    const headers = { 'X-Requested-With': 'XMLHttpRequest' };
    const jwt = localStorage.getItem('Bearer:/gishub/api');
    if (!!jwt) {
        headers['Authorization'] = `Bearer ${jwt}`;
    }
    return headers;
  }

  view.when().then(async () => {
    const res = await fetch('rest/services/slpks', { headers: getHeaders() });
    const json = await res.json();
    const id = json[0].id;
    const url = `rest/services/slpks/${id}/SceneServer`;
    const meshLayer = new IntegratedMeshLayer({
      url
    });
    await meshLayer.load();
    view.goTo(meshLayer.fullExtent);
    view.map.add(meshLayer);
  });
});
