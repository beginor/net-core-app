(function(){

	var wmtsUtil = window.wmtsUtil = function(){
		return new wmtsUtil.fn.init();
	};

	wmtsUtil.fn = wmtsUtil.prototype = {

		init: function(){
			return this;
		},

		getMatchServiceOption: function(serviceUrl){
			var that = this;
			var layerArry;
			var url;
			if(serviceUrl.indexOf("?")>0){
				url = serviceUrl+"&service=wmts";
			}else{
				url = serviceUrl+"?service=wmts";
			}
			var format = new GeoGlobe.Format.WMTSCapabilities();
		  	GeoGlobe.Request.GET({
	            url: url,
	            params: {
	                REQUEST: "GetCapabilities"
	            },
	            async:false,
	            success: function(request){
            		var json = format.read(request.responseText);  // 版本号赋值
                	layerArry = that.WMTSMatchAnalyzer(json);
	            }
	        });
		  	return layerArry;
		},

		//WMTS类型的服务数据 解析.
		WMTSMatchAnalyzer: function(json){
			var layers = json.contents.layers;
			var tileMatrixSets = json.contents.tileMatrixSets;
			var layerArr = [];
			$(layers).each(function(i,data){
				var layerObj = {};
				layerObj["name"]=data.identifier;
				layerObj["type"] = "WMTS";
				layerObj["alias"]= data.identifier;
				layerObj["opacity"]=1;
				layerObj["visibility"]=true;
				layerObj["formats"]=data.formats;
				layerObj["format"]=data.formats[0];//"image/tile";
				layerObj["style"]=data.styles[0].identifier;
				layerObj["tileFullExtent"]=data.bounds.left+","+data.bounds.bottom+","+data.bounds.right+","+data.bounds.top;
				layerObj["matrixSet"]= data.tileMatrixSetLinks[0].tileMatrixSet;
				var matrixIds = [];
				$(tileMatrixSets[data.tileMatrixSetLinks[0].tileMatrixSet].matrixIds).each(function(j,d){
					var matrixid = {};
					matrixid.identifier =d.identifier;
					matrixid.scaleDenominator =d.scaleDenominator;
					matrixid.tileHeight =d.tileHeight;
					matrixid.tileWidth =d.tileWidth;
					matrixIds.push(matrixid);
				});
				layerObj["matrixIds"] = matrixIds;
				layerArr.push(layerObj);
			});
			return layerArr;
		},

		createLayer: function(url,options){
			var layerInf = {};
			var random = TDT.random(4);
			if(url.indexOf('?') > -1){
				url =  url + "&SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER="
					+options.name+"&STYLE=" + options.style + "&TILEMATRIXSET="+ options.matrixSet + "&TILEMATRIX={z}&TILEROW={y}&TILECOL={x}&FORMAT=" + options.formats[0];
			}else{
				url =  url + "?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER="
					+options.name+"&STYLE=" + options.style + "&TILEMATRIXSET="+ options.matrixSet + "&TILEMATRIX={z}&TILEROW={y}&TILECOL={x}&FORMAT=" + options.formats[0];
			}
	        var matrixIds = options.matrixIds;
	        //minzoom
    		if(matrixIds[0].islevelhidden == false){
    			var minZoom = matrixIds[0].identifier;
    		}else if(matrixIds[0].islevelhidden == true){
    			for(i=0;i< matrixIds.length-1;i++){
    				if(matrixIds[i].islevelhidden == true && matrixIds[i+1].islevelhidden == false){
    					var minZoom = matrixIds[i+1].identifier;
    				}
    			}
    		}else{
    			var minZoom = matrixIds[0].identifier
    		};
    		//maxzoom
    		if(matrixIds[matrixIds.length-1].islevelhidden == false){
    			var maxZoom = matrixIds[matrixIds.length-1].identifier;
    		}else if(matrixIds[matrixIds.length-1].islevelhidden == true){
    			for(i= matrixIds.length-1;i>0;i--){
    				if(matrixIds[i].islevelhidden == true && matrixIds[i-1].islevelhidden == false){
    					var maxZoom = matrixIds[i-1].identifier;
    				}
    			}
    		}else{
    			var maxZoom = matrixIds[matrixIds.length-1].identifier
    		};
	        var wmtsSource = new GeoGlobe.Source.RasterSource({
	            "id": "sourceId_"+options.name+"_"+random,
	            "type": "raster",
	            "url": [url],
	            "minzoom": parseInt(minZoom),
	            "maxzoom": parseInt(maxZoom)+1,
	            "tileSize": 256
	        });
	        wmtsSource['minzoom'] = parseInt(minZoom);
	        wmtsSource['maxzoom'] = parseInt(maxZoom)+1;
	        var visibility = options.visibility;
	        if(options.cityName && parseInt(minZoom) > 17){
	        	visibility = "false";
	        }
	        var wmtsLyaer = new GeoGlobe.Layer.RasterLayer({
	            "id": "layerId_"+options.name+"_"+random,
	            "type": "raster",
	            "source": wmtsSource.id,
	            "name": options.name,
	            "metadata": options,
	            "minzoom": parseInt(minZoom),
	            "maxzoom": parseInt(maxZoom)+2,
	            "paint": {
	            	"raster-opacity":1
	            },
	            "layout": {
	            	"visibility":(visibility == true || visibility == "true") ? "visible" : "none"
	            }
	        });
	        layerInf["layer"] = wmtsLyaer;
	        layerInf["source"] = wmtsSource;
	        return layerInf;
		}

	};

	wmtsUtil.fn.init.prototype = wmtsUtil.fn;

})();
