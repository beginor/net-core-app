var mapView = Window.mapView = {

	map: null,

	wmtslayersUrl: [
		'http://172.21.11.241:5800/gmap/ebus/zwdsj-dlk/geostar/GD_HLFB/wmts'
		// 'http://172.21.11.241:5800/gmap/ebus/zwdsj-dlk/geostar/GD_2018DLG/wmts',
		// 'http://172.21.11.241:5800/gmap/ebus/zwdsj-dlk/geostar/GD_2018DLGZJ/wmts'
	], //广东2018年7_17级矢量图，广东2018年7_17级矢量注记，示例使用，可替换为需要查看的地图服务地址

	// wfslayerurl: 'http://172.21.11.241:5800/gmap/ebus/zwdsj-dlk/geostar/SLT_DXSZ/wfs', //广东矿产资源分布，示例使用，可替换为需要查看的要素服务地址，也可直接注释不加载显示要素服务

	// wfslayerurl: 'http://172.21.11.241:5800/gmap/ebus/zwdsj-dlk/geostar/GDHP/wfs?output=json', //广东湖泊，示例使用，可替换为需要查看的要素服务地址，也可直接注释不加载显示要素服务

	addrToLocGeocodingUrl: 'http://172.21.11.241:5800/gmap/ebus/zwdsj-dlk/geostar/GDS_DMDZ/geocoding', //广东地名地址查询服务，正向匹配使用
	locToAddrGeocodingUrl: 'http://172.21.11.241:5800/gmap/ebus/zwdsj-dlk/geostar/GDS_DMDZ_esindex/geocoding', //广东坐标匹配查询服务，反向匹配使用

	wmtsUtil: wmtsUtil(),
	wfsUtil: wfsUtil(),
	geocodingUtil: null,

	init: function() {
		this.initMap();   //初始化地图

		//初始化地名地址查询
		this.geocodingUtil = geocodingUtil(this.map, this.addrToLocGeocodingUrl, this.locToAddrGeocodingUrl);
		this.initGeoCoding();

		this.addWfsLayer();  //添加矢量要素图层
	},

	initMap: function() {
		var that = this
        that.createMap('gmap');
	},

	createMap: function(div) {
		var that = this;
		var simple = {
	        "version": 8,
	        "sources": {},
	        "layers": []
	    };
	    var wgs84_wgs84_mapcrs = {
	        topTileExtent: [-180, -270, 180, 90],
	        coordtransform: "none"
	    };
	    var map = new GeoGlobe.Map({
	        mapCRS: wgs84_wgs84_mapcrs,
	        style: simple,
	        container: div,
	        zoom: 4,
	        center: [112.939, 31.377],
	        isIntScrollZoom: true, //缩放级别是否为整数处理模式
	        renderWorldCopies: false,
	        isAttributionControl: false,
	        is3Dpitching: false, //是否到指定层级自动倾斜
	        pitch3Dzoom: 16 //自动倾斜的层级，默认16
	    });
	    this.map = map;
        map.on("style.load",function(){
            that.initLayerGroups(map);//初始化图层组
            that.initControls(map);//初始化控件
    		map.setZoom(9);
    		map.setCenter([113.272753, 23.139257]);
        });
	},

	initControls: function(map) {
        //比例尺控件
        var Sca_control = new GeoGlobe.Control.Scale({
            position: 'bottom-right',
            maxWidth: 80,
            unit: 'm'
        });
        map.addControl(Sca_control, Sca_control.options.position);
        //导航控件
        var Na_control = new GeoGlobe.Control.Navigation();
        map.addControl(Na_control,"bottom-right");
	},

	//初始化图层组
	initLayerGroups: function(map) {
		var that = this;
		var wmtslayersUrl = that.wmtslayersUrl
    	for(var i=0;i<wmtslayersUrl.length;i++) {
    		var wmtsurl = wmtslayersUrl[i];
        	var layerOptions = that.wmtsUtil.getMatchServiceOption(wmtsurl);
    		var layerInf = that.wmtsUtil.createLayer(wmtsurl,layerOptions[0]);
    		map.addSource(layerInf.source.id, layerInf.source);
    		map.addLayer(layerInf.layer);
    	}
	},

	//初始化地名地址查询
	initGeoCoding: function(){
		var that = this;
		$('#geoCodingType').change(function(){
			var value = $(this).find('option:selected').val();
			$('.geocodingDiv').hide();
			$('#' + value + 'Div').show();
		});

		$('#locToAddr').click(function(){
			that.geocodingUtil.locationToAddressesQuery(1);
		});

		$('#addrToLoc').click(function(){
			that.geocodingUtil.addressesToLocationsQuery(1);
		});
	},

	//叠加wfs图层
	addWfsLayer: function(){
		var that = this;
		if(!that.wfslayerurl){
			return;
		}
		var wfslayerurl  =that.wfslayerurl;
		//初始化信息
		that.wfsUtil.getLayer(wfslayerurl,function(layerWfsOpt){
			var featureType = layerWfsOpt[0].name;
			that.wfsUtil.initwfsQueryObj(wfslayerurl,featureType);
		});
		that.wfsUtil.queryWFS(function(result){
			var geojson = result.geojson;
			that.wfsUtil.clearWfsFeature(that.map);
			that.wfsUtil.draWfsFeature(that.map,geojson);
		});
	},

};
mapView.init();


