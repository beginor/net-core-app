(function () {

    var geocodingUtil = window.geocodingUtil = function (map, addrToLocUrl, locToAddrUrl) {
        return new geocodingUtil.fn.init(map, addrToLocUrl, locToAddrUrl);
    };

    geocodingUtil.fn = geocodingUtil.prototype = {

        addrToLocUrl: null,
        locToAddrUrl: null,
        coding: null,
        marker: null,
        myDiv: null,
        map: null,

        init: function (map, addrToLocUrl, locToAddrUrl) {
            this.addrToLocUrl = addrToLocUrl + '?1=1';
            this.locToAddrUrl = (locToAddrUrl ? locToAddrUrl : addrToLocUrl) + '?1=1';
            this.map = map;
            return this;
        },

        //根据地址匹配查询参数查询匹配的地址信息
        addressesToLocationsQuery: function(pageNum){
            var that = this;
            document.getElementById("addrToLocDIVresult").innerHTML = "";
            //var queryText = encodeURIComponent(encodeURIComponent(document.getElementById("queryText").value));
            var queryText = encodeURIComponent(document.getElementById("addrToLocQueryText").value);
            if(queryText === ""){
                return;
            }
            //结果统计信息
            var resultType =  "result";
            var htmlStr = "";
            //根据地址查询
            that.coding = new GeoGlobe.Query.GeoCodingQuery(that.addrToLocUrl, {
                version: '1.1.0'
            });

            that.coding.addressesToLocations({
                address: queryText,
                semanticAnalysis: true,
                resultType: resultType
            }, function(GeoCodingResult){
                if ("" == GeoCodingResult||GeoCodingResult.status == "NO_RESULTS" || GeoCodingResult.status == "INVALID_REQUEST" || GeoCodingResult.status == "UNKNOWN_ERROR") {
                    htmlStr += "没有查询到任何数据。";
                    document.getElementById("addrToLocDIVresult").innerHTML = htmlStr;
                    return;
                }
                var total = GeoCodingResult.features.length;
                //根据地址查询
                that.coding.addressesToLocations({
                    address: queryText,
                    semanticAnalysis: true,
                    resultType: resultType,
                    maxCount: 3,
                    startPosition: (pageNum - 1) * 3 + 1
                }, function(GeoCodingResult){
                    that.addrToLocQuerySuccessFn(GeoCodingResult, total, pageNum);
                }, function(){
                    alert("查询失败！");
                });
            }, function(){
                alert("查询失败！");
            });
        },
        //查询成功回调处理
        addrToLocQuerySuccessFn: function(GeoCodingResult, total, pageNum){
            //判断是否显示地址关联的poi
            var isDisplayPoi = document.getElementById("trd").checked;
            var htmlStr = "";
            var features = [];
            if (GeoCodingResult.status == "NO_RESULTS" || GeoCodingResult.status == "INVALID_REQUEST" || GeoCodingResult.status == "UNKNOWN_ERROR") {
                htmlStr += "没有查询到任何数据。";
                document.getElementById("addrToLocDIVresult").innerHTML = htmlStr;
                return;
            }
            if(typeof GeoCodingResult === "string" || !(GeoGlobe.Util.isArray(GeoCodingResult.features))) {
                alert("请求服务失败！请检查服务是否正常运行！");
                return;
            }
            if (this.marker) {
                this.marker.remove();
            }
            if (this.myDiv) {
                $("myDiv").remove(".marker");
            }
            var results = GeoCodingResult.features;
            for (var i = 0; i < results.length; i++) {
                var result = results[i];
                var attributes = result.attributes;
                //行政区划的第一级-国家
                var country = "";
                //行政区划的第二级-省、直辖市、自治区
                var province = "";
                //行政区划第三级-市
                var city = "";
                //行政区划第四级-区或县
                var district = "";
                //行政区划第五级-乡镇
                var town = "";
                //街道
                var street = "";
                if(null != attributes && undefined != attributes){
                    //行政区划的第一级-国家
                    country = attributes.country;
                    if(null == country || undefined == country){
                        country = "";
                    }
                    //行政区划的第二级-省、直辖市、自治区
                    province = attributes.province;
                    if(null == province || undefined == province){
                        province = "";
                    }
                    //行政区划第三级-市
                    city = attributes.city;
                    if(null == city || undefined == province){
                        city = "";
                    }
                    //行政区划第四级-区或县
                    district = attributes.district;
                    if(null == district || undefined == district){
                        district = "";
                    }
                    //行政区划第五级-乡镇
                    town = attributes.town;
                    town = town ? town : "";
                    if(null == town || undefined == town){
                        town = "";
                    }
                    //街道
                    street = attributes.streetName;
                    var name =attributes.name;
                    if (null == street && undefined == street) {
                        street = "";
                    }
                    var poi = [attributes.lng,attributes.lat];
                    //var p = new Geo.Geometry.Point(lng, lat);
                    var imgPath = TDT.getAppPath("") + "img/" + i + ".png";
                    var streetName = "";

                    if(poi){
                        var bigImg = document.createElement("img");     //创建一个img元素
                        bigImg.src=imgPath;   //给img元素的src属性赋值
                        this.myDiv = document.createElement('myDiv'); //获得dom对象
                        this.myDiv.className = 'marker';
                        this.myDiv.appendChild(bigImg);      //为dom添加子元素img
                        this.marker = new GeoGlobe.Marker(this.myDiv)
                                .setLngLat(poi)
                                .addTo(this.map);
                    }
                    htmlStr +="<img src="+imgPath+" style='width:25px;height:25px;'/> ";
                    var address = city + district + town + street;
                    // htmlStr += "<span><b>" +"地址："+ address + "</b><br/>";
                    var center =poi;
                    if(isDisplayPoi){
                        htmlStr += "<span><b>" + name + "</b><br/>";
                    }
                    htmlStr += "坐标：" + poi[0] + "," + poi[1] + "<br/>";
                }
            }
            htmlStr += this.getPageFooterHTML(total, pageNum);
            this.map.setCenter(new GeoGlobe.LngLat(center[0],center[1]), 14);
            this.map.setZoom(14);
            document.getElementById("addrToLocDIVresult").innerHTML = htmlStr;

            var that = this;
            $('#pageDiv a').click(function(){
                that.addressesToLocationsQuery($(this).text());
            });
        },


        //根据地址匹配查询参数查询匹配的地址信息
        locationToAddressesQuery: function (pageNum) {
            var that = this;
            document.getElementById("locToAddrDIVresult").innerHTML = "";
            var queryText = document.getElementById("locToAddrQueryText").value;
            if (queryText === "") {
                return;
            }
            //结果统计信息
            var resultType = "result";
            var htmlStr = "";
            //根据坐标查询
            var features = [];
            var lonlat = new GeoGlobe.LngLat(queryText.split(",")[0], queryText.split(",")[1]);
            //查询总数
            that.coding = new GeoGlobe.Query.GeoCodingQuery(that.locToAddrUrl, {
                version: '1.1.0'
            });

            that.coding.locationToAddresses({
                lonlat: lonlat,
                // tolerance: 10,
                // unit:"meter",
                resultType: resultType
            }, function (GeoCodingResult) {
                if (typeof (GeoCodingResult.status) == "undefined" || GeoCodingResult.status == "NO_RESULTS" || GeoCodingResult.status == "INVALID_REQUEST" || GeoCodingResult.status == "UNKNOWN_ERROR") {
                    htmlStr += "没有查询到任何数据。";
                    document.getElementById("locToAddrDIVresult").innerHTML = htmlStr;
                    return;
                }
                var total = GeoCodingResult.features.length;
                //查询结果
                that.coding.locationToAddresses({
                    lonlat: lonlat,
                    // tolerance: 100,
                    // unit:"meter",
                    resultType: resultType,
                    maxCount: 3,
                    startPosition: (pageNum - 1) * 3 + 1
                }, function (GeoCodingResult) {
                    that.locToAddrQuerySuccessFn(GeoCodingResult, total, pageNum);
                }, function () {
                    alert("查询失败！");
                });
            }, function () {
                alert("查询失败！");
            });
        },
        //查询成功回调处理
        locToAddrQuerySuccessFn: function (GeoCodingResult, total, pageNum) {
            var htmlStr = "";
            if (GeoCodingResult.status == "NO_RESULTS" || GeoCodingResult.status == "INVALID_REQUEST" || GeoCodingResult.status == "UNKNOWN_ERROR") {
                htmlStr += "没有查询到任何数据。";
                document.getElementById("locToAddrDIVresult").innerHTML = htmlStr;
                return;
            }
            if (typeof GeoCodingResult === "string" || !(GeoGlobe.Util.isArray(GeoCodingResult.features))) {
                alert("请求服务失败！请检查服务是否正常运行！");
                return;
            }
            if (this.marker) {
                this.marker.remove();
            }
            if (this.myDiv) {
                $("myDiv").remove(".marker");
            }
            var results = GeoCodingResult.features;

            //查询总数
            for (var i = 0; i < results.length; i++) {
                var result = results[i];
                var attributes = result.attributes;
                //行政区划的第一级-国家
                var country = "";
                //行政区划的第二级-省、直辖市、自治区
                var province = "";
                //行政区划第三级-市
                var city = "";
                //行政区划第四级-区或县
                var district = "";
                //行政区划第五级-乡镇
                var town = "";
                //街道
                var street = "";
                if (null != attributes && undefined != attributes) {
                    //行政区划的第一级-国家
                    country = attributes.country;
                    if (null == country || undefined == country) {
                        country = "";
                    }
                    //行政区划的第二级-省、直辖市、自治区
                    province = attributes.province;
                    if (null == province || undefined == province) {
                        province = "";
                    }
                    //行政区划第三级-市
                    city = attributes.city;
                    if (null == city || undefined == province) {
                        city = "";
                    }
                    //行政区划第四级-区或县
                    district = attributes.district;
                    if (null == district || undefined == district) {
                        district = "";
                    }
                    //行政区划第五级-乡镇
                    town = attributes.town;
                    town = town ? town : "";
                    if (null == town || undefined == town) {
                        town = "";
                    }
                    //街道
                    street = attributes.streetName;
                    var name = attributes.name;
                    if (null == street && undefined == street) {
                        street = "";
                    }
                    var poi = [attributes.lng, attributes.lat];
                    var categoryName = "";
                    var imgPath = TDT.getAppPath("") + "img/" + i + ".png";

                    htmlStr += "<img id='img" + i + "'' src=" + imgPath + " style='width:25px;height:25px;'/> ";
                    htmlStr += "<span id='span" + i + "' ><b>" + name + "</b><br/>";
                    var streetName = "";

                    var address = city + district + town + street;
                    //	htmlStr += "地址：" + address + "<br/>";
                    if (attributes.subordinate) {
                        htmlStr += "下属行政区划：" + attributes.subordinate + "<br/>";
                    }
                    if (null != categoryName && undefined != categoryName) {
                        htmlStr += "分类名称：" + categoryName + "<br/>";
                    }
                    var bigImg = document.createElement("img");     //创建一个img元素
                    bigImg.src = imgPath;   //给img元素的src属性赋值
                    this.myDiv = document.createElement('myDiv'); //获得dom对象
                    this.myDiv.className = 'marker';
                    this.myDiv.appendChild(bigImg);      //为dom添加子元素img
                    this.marker = new GeoGlobe.Marker(this.myDiv)
                        .setLngLat(poi)
                        .addTo(this.map);
                    var center = poi;
                    htmlStr += "&nbsp;&nbsp;坐标：" + poi[0] + "," + poi[1] + "<br/>";
                }
            }
            htmlStr += this.getPageFooterHTML(total, pageNum);
            this.map.setCenter(new GeoGlobe.LngLat(center[0], center[1]), 14);
            this.map.setZoom(14);
            document.getElementById("locToAddrDIVresult").innerHTML = htmlStr;

            var that = this;
            $('#pageDiv a').click(function(){
                that.locationToAddressesQuery($(this).text());
            });
        },

        //获取查询结果下分页的HTML
        getPageFooterHTML: function (total, pageNum) {
            var html = "<div style='text-align:center;' id='pageDiv'>";
            //分页的页数
            //分页的页数
            if (total % 3 == 0) {
                var pageNums = parseInt(total / 3);
            } else {
                var pageNums = parseInt(total / 3) + 1;
            }
            //实际分页的页数可能大于10页，但本示例分页的页数保持在10页之内。
            pageNums = pageNums > 10 ? 10 : pageNums;
            for (var i = 0; i < pageNums; i++) {
                html += "<span style='margin:0 8px 0 8px;cursor:pointer'>";
                if (pageNum == (i + 1)) {
                    html += (i + 1);
                } else {
                    html += "<a href='#'>" + (i + 1) + "</a>";
                }
                html += "</span>";
            }
            html += "</div>";
            return html;
        }
    };

    geocodingUtil.fn.init.prototype = geocodingUtil.fn;

})();
