(function(){

	var wfsUtil = window.wfsUtil = function(){
		return new wfsUtil.fn.init();
	};

	wfsUtil.fn = wfsUtil.prototype = {

		geometryName: 'GEOMETRY',
		wfsVersion: "1.0.0",
		wfsQueryObj: null,  //wfs服务查询操作对象

		init: function(){
			return this;
		},

		getLayer : function(serviceUrl, callback){
 			var that=this;
 			var url;
			if(serviceUrl.indexOf("?")>0){
				url = serviceUrl+"&service=wfs";
			}else{
				url = serviceUrl+"?service=wfs";
			}
	        GeoGlobe.Request.GET({
	            url: url,
	            params: {
	                request: "GetCapabilities",
	                version:"1.1.0"
	            },
	            async:false,
	            success: function(request){
	            	if(!request.responseText || request.responseText.indexOf("Exception") != -1 ){
	            		that.failFn();
	            		return;
	            	}
	                var doc = request.responseText;
	                var jsonOnj = new GeoGlobe.Format.WFSCapabilities.v1().read(doc);
	                try {
	                    var array = jsonOnj.featureTypeList.featureTypes;
	                    callback && callback(array);
	                }
	                catch (e) {
	                	that.failFn();
	                }
	            },
	            failure: this.failFn
	        });
		},

		failFn:function(){
			alert("获取服务信息失败!");
			return ;
		},

		initwfsQueryObj:function(wfsUrl,featureType){
			var that = this;

	    	that.wfsQueryObj = new GeoGlobe.Query.WFSQuery(wfsUrl, featureType,{
	        	geometryName: that.geometryName,
				version: that.wfsVersion,
	         	maxFeatures: 99999
	        });
		},

		queryWFS: function(callback){
			var that = this;
			var filter  = null;
			that.wfsQueryObj.query(filter,function(result){
				callback && callback(result);
			},function(){
				console.log("查询失败");
			});
		},

		draWfsFeature:function(map,geojson) {
	        var pointjson = {
	            'type': 'geojson',
	            'data': geojson
	        };
	        map.addLayer({
	            "id": "pointLayer",
	            "type": "circle",
	            "source": pointjson,
	            "paint": {
	                "circle-radius": 5,
	                "circle-color": "red",
	                "circle-opacity": 0.8
	            }
	        });
		},

		clearWfsFeature: function(map){
	        var that = this;
	        if (map.getLayer('pointLayer')) {
	            map.removeLayerAndSource('pointLayer')
	        }
		}
	};

	wfsUtil.fn.init.prototype = wfsUtil.fn;

})();
