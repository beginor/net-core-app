/**
 * 天地图基线公用类
 * @author wangshoudong
 * @version 1.0 2012/6/5
 */
(function () {
    var Tianditu = window.Tianditu = function () {
        return new Tianditu.fn.init();
    },

        _TDT = window.TDT,

        //应用路径
        appPath = "/gmap/";

    Tianditu.fn = Tianditu.prototype = {//

        /**
         * 初始化
         */
        init: function () {
            return this;
        },

        /**
         * 版本号
         */
        version: "1.0"

    };

    Tianditu.fn.init.prototype = Tianditu.fn;

    Tianditu.extend = Tianditu.fn.extend = function () {
        // copy reference to target object
        var target = arguments[0] || {}, i = 1, length = arguments.length, deep = false, options, name, src, copy;

        // Handle a deep copy situation
        if (typeof target === "boolean") {
            deep = target;
            target = arguments[1] || {};
            // skip the boolean and the target
            i = 2;
        }

        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !Tianditu.isFunction(target)) {
            target = {};
        }

        // extend Tianditu itself if only one argument is passed
        if (length === i) {
            target = this;
            --i;
        }

        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) != null) {
                // Extend the base object
                for (name in options) {
                    src = target[name];
                    copy = options[name];

                    // Prevent never-ending loop
                    if (target === copy) {
                        continue;
                    }

                    // Recurse if we're merging object values
                    if (deep && copy && typeof copy == "object" && !copy.nodeType)
                        target[name] = Tianditu.extend(deep,
                            // Never move original objects, clone them
                            src || (copy.length != null ? [] : {})
                        , copy);

                        // Don't bring in undefined values
                    else if (copy !== undefined)
                        target[name] = copy;
                }
            }
        }

        // Return the modified object
        return target;
    };


    /**
     * 扩展方法
     */
    Tianditu.extend({

        /**
         * 获取应用路径
         */
        getAppPath: function (url) {
            return appPath + url;
        },
        
        /**
		 * 获取引用的网络路径
		 */
		getWebSiteUrl: function(url) {
			//系统主机IP地址
			var host = window.location.host;
			//项目名称
			//Cfg.projectName = window.location.pathname.split("/")[1];
			//网站地址
			var webSiteUrl = "http://" +  host + appPath
			return webSiteUrl;
		},

        /**
         * 判断某个元素是否在数组中
         * @return 若在返回下标，否则返回-1 
         */
        inArray: function (elem, array) {
            for (var i = 0, length = array.length; i < length; i++)
                // Use === because on IE, window == document
                if (array[i] === elem)
                    return i;

            return -1;
        },

        /**
         * 从指定数组中去除指定值
         * @return 新的数组 
         */
        removeValFromArr: function (val, arr) {
            var len = arr.length;
            for (var i = 0; i < len; i++) {
                if (arr[i] == val) {
                    arr.splice(i, 1);
                }
            }
            return arr;
        },

        /**
         * 获取URL问号后面的值
         * @return 返回指定key的value
         */
        getParam: function (param) {
            var local = document.location.search.substring(1);
            var splits = local.split("&");
            for (var i = 0; i < splits.length; i++) {
                var sp = splits[i];
                if (sp.indexOf(param + "=") == 0) {
                    var val = sp.substring(param.length + 1);
                    return decodeURIComponent(val);
                }
            }
        },

        /**
         * 判断对象是否为空
         * return true:为空 false:不为空 
         */
        isEmptyObjec: function (obj) {
            if (!obj) {
                return true;
            }

            for (var o in obj) {
                return false;
            }

            return true;
        },

        isFunction: function (obj) {
            return toString.call(obj) === "[object Function]";
        },

        isArray: function (obj) {
            return toString.call(obj) === "[object Array]";
        },

		
		 //处理图片的访问路劲，为本机的网络路径，防止修改应用部署的域名、IP、端口后会导致图片访问路劲失效
	  processImgSrcToHostUrl:function(htmlTx){
	  	if(htmlTx == null || htmlTx == undefined || htmlTx == ""){
	  		return null;
	  	}
		var $htmlTxDom = $(htmlTx);
		var imgArr = $htmlTxDom.find("img");
		for(var i = 0; i < imgArr.length; i++){
			//src例如"http://localhost:8087//admin/ckeditor/viewFile.do?fileId=3574fc48-3214-4ce3-a727-6412a9b87b2b&amp;${webSiteUrl}=http://localhost:8087//admin/"
			var src = imgArr[i].src;
			//?前的url
			//var reqUrl = src.split("?")[0];
			//?后的参数字符串
			var paramStrArr = null;
			if(src.split("?")[1]==null){
				paramStrArr = src.split("?")[0];
			}else{
				paramStrArr = src.split("?")[1].split("&");
			}
			for(var j = 0; j < paramStrArr.length; j++){
				//是否存在${webSiteUrl}
				if(paramStrArr[j].indexOf("${webSiteUrl}") >= 0){
					var webSiteUrl = paramStrArr[j].split("=")[1];
					//把从后台取出来的图片路径替换为本项目的网络访问路径
					src = src.replace(new RegExp(webSiteUrl, "gm"), TDT.getWebSiteUrl() + "admin/");
					imgArr[i].src = src;
				}
			}
		}
		/*
		var text = "";
		if($htmlTxDom[0].outerHTML.length>100){
			for(var j=0;j<$htmlTxDom[0].outerHTML.length/150;j++){
				text += $htmlTxDom[0].outerHTML.substr(0,100)+"</br>";
			}
		}else{
			text = $htmlTxDom[0].outerHTML;
		}
		*/
		return $htmlTxDom[0].outerHTML;
	  },
			
		
        /**
         * 截取字符串指定长度，超过指定长度则"..."显示
         * @param str-原字符串
         * @param len要截取的字符个数
         * 
         * @return string 截取后的字符串
         */
        strCut: function (str, len) {
            var s = str || "";
            var l = s.length;
            return l > parseInt(len) ? s.substr(0, len) + "..." : s;
        },

        /**
         *检查所传递的参数中是否有undefined或null存在,如果有则改为空
         *可以检查一串查询字符串,也可以检查单个值.
         */
        formatParam: function (param) {
            if (param == null || param == undefined) {
                return "";
            }

            param += "&";
            param = param.replace(/=undefined&/g, "=&");
            param = param.replace(/=null&/g, "=&");
            return param.substring(param, param.lastIndexOf("&"));
        },


        /**
         * 远程调用获取服务器数据
         * @param URL 服务器URL
         * @param params 传递到服务器参数，例如：username=aaa&password=111
         * @param isAsync 是否是异步传输 ,true 表示异步，false表示同步
         * @param callback 回调函数
         */
        getDS: function (URL, params, isAsync, callback) {
            URL = $.trim(URL);
            URL = this.formatParam(URL);
            params = this.formatParam(params);
            var async = (typeof isAsync == "undefined") ? true : isAsync;
            async = async === false ? false : true;
            $.ajax({
                type: "post",
                url: this.getAppPath(URL),
                data: params,
                dataType: "json",
                async: async,
                success: function (json) {
                    if (callback && (typeof callback != "undefined")) {
                        if (json) {
                            callback(json);
                        }
                    } else {
                        return json;
                    }
                }
            });
        },
        
        /**
         * 远程调用获取服务器数据
         * @param URL 服务器URL
         * @param params 传递到服务器参数，例如：username=aaa&password=111
         * @param isAsync 是否是异步传输 ,true 表示异步，false表示同步
         * @param callback 回调函数
         */
        getJSONDS: function (URL, params, isAsync, callback) {
            URL = $.trim(URL);
            URL = this.formatParam(URL);
            params = this.formatParam(params);
            var async = (typeof isAsync == "undefined") ? true : isAsync;
            async = async === false ? false : true;
            $.ajax({
                type: "post",
                url: this.getAppPath(URL),
                data: params,
                contentType:"application/json;charset=UTF-8",
                dataType: "json",
                async: async,
                success: function (json) {
                    if (callback && (typeof callback != "undefined")) {
                        if (json) {
                            callback(json);
                        }
                    } else {
                        return json;
                    }
                }
            });
        },
        
        getArgDS: function(URL,params,isAsync,callback){
		URL = $.trim(URL);
		URL = this.formatParam(URL);
		params = this.formatParam(params);
		var async = (typeof isAsync == "undefined") ? true : isAsync;	
		async = async === false ? false: true;
		$.ajax({
	          type:"POST",
	          url:URL,
	          data:params,
	          dataType:"json",
	          async:async,
	          success:function(json){	          	
          		 if(callback && (typeof callback != "undefined")){
          			if(json){
          			    callback(json); 
          			}
          		 }else{
          		  	return json;
          		 }
	          }
		});
	},
        
        
        /**
         * 远程调用获取服务器数据
         * @param URL 服务器URL
         * @param params 传递到服务器参数，例如：username=aaa&password=111
         * @param isAsync 是否是异步传输 ,true 表示异步，false表示同步
         * @param callback 回调函数
         */
        ajax: function (URL, params, isAsync, callback) {
            URL = $.trim(URL);
            URL = this.formatParam(URL);
            params = this.formatParam(params);
            var async = (typeof isAsync == "undefined") ? true : isAsync;
            async = async === false ? false : true;
            $.ajax({
                type: "post",
                url: URL,
                data: params,
                dataType: "json",
                async: async,
                success: function (json) {
                    if (callback && (typeof callback != "undefined")) {
                        if (json) {
                            callback(json);
                        }
                    } else {
                        return json;
                    }
                }
            });
        },

        /**
         * 表单提交
         * @param id 表单DOM对象ID
         * @param URL 服务器URL
         * @param params 传递到服务器参数，例如：username=aaa&password=111
         * @param isAsync 是否是异步传输
         * @param callback 回调函数
         */
        formSubmit: function (id, URL, params, isAsync, callback) {
            //检查提交的参数值是否合法,不合法则将参数值设为空
            URL = this.formatParam(URL);
            params = this.formatParam(params);
            var async = (typeof isAsync == "undefined") ? true : isAsync;
            async = async === false ? false : true;
            var option = {
                url: this.getAppPath(URL),
                data: params,
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded;charset=UTF-8',
                async: async,
                success: function (json) {
                    if (callback && (typeof callback != "undefined")) {
                        if (json) {
                            callback(json);
                        }
                    } else {
                        return json;
                    }
                }
            };
            $("#" + id).ajaxSubmit(option);
        },

        formSubmit2 : function(id, URL, params, isAsync, callback){
    		//检查提交的参数值是否合法,不合法则将参数值设为空
    		URL = this.formatParam(URL);	
    		params = this.formatParam(params);
    		var async = (typeof isAsync == "undefined") ? true : isAsync;	
    		async = async === false ? false: true;
    	    var option={
    	          url:URL,
    	          data:params,
    	          dataType:'json',
    	          contentType:'application/x-www-form-urlencoded;charset=UTF-8',
    	          async:async,
    	          success:function(json){
    	          		if(callback && (typeof callback != "undefined")){
    		          			if(json){
    		          			    callback(json);     	     			          	     		          	     			          	   			          				          	        			          
    		          			}
    	          		  }else{
    	          		  	return json;
    	          		  }
    	          }
    	    }
    	    $("#"+id).ajaxSubmit(option);
    							   	
    	},

        /**
         * 分页控件
         * @param pageNum 页码
         * @param pageSize 页大小
         * @param totalRecords 总记录数
         * @param container 分页容器
         * @param func 分页函数
         */
        pager: function (pageNum, pageSize, totalRecords, container, func) {
            var showPage = "";
            var currPage = pageNum;//当前页码
            var pageN = totalRecords / pageSize;//分页总数
            if (totalRecords % pageSize > 0) pageN = parseInt(totalRecords / pageSize) + 1;
            if (pageNum > 1) {
                showPage = '<ul class="page-num"><li><a href="javascript:void(0);" class="prev">上一页</a></li>';
            } else {
                showPage = '<ul class="page-num"><li disabled="disabled"><a href="javascript:void(0);" class="prev">上一页</a></li>';
            }

            var showPageNum = 10;//最多显示页码数量
            if (pageN <= showPageNum) {
                for (var i = 1; i <= pageN; i++) {
                    showPage += '<li><a href="javascript:void(0);">' + i + '</a></li>';
                }
            } else {
                if (pageNum < 6) {
                    for (var i = 1; i <= showPageNum; i++) {
                        showPage += '<li><a href="javascript:void(0);">' + i + '</a></li>';
                    }
                } else if (pageNum >= pageN - 5 && pageNum <= pageN) {
                    var start = pageN - showPageNum + 1;
                    for (var i = start; i <= pageN; i++) {
                        showPage += '<li><a href="javascript:void(0);">' + i + '</a></li>';
                    }
                } else {
                    var start = pageNum - 4;
                    for (var i = start; i < pageNum; i++) {
                        showPage += '<li><a href="javascript:void(0);">' + i + '</a></li>';
                    }

                    var end = pageNum + 5;
                    for (var i = pageNum; i < end; i++) {
                        showPage += '<li><a href="javascript:void(0);">' + i + '</a></li>';
                    }

                }
            }

            if (pageNum < pageN) {
                showPage += '<li><a href="javascript:void(0);" class="next">下一页</a></li></ul>';
            } else {
                showPage += '<li disabled="disabled"><a href="javascript:void(0);" class="next">下一页</a></li></ul>';
            }
            var jContainer = $(container);
            jContainer.html(showPage + '<div class="total-num">共有<span class="num">' + totalRecords + '</span>条记录</div></div>');


            //绑定页码事件
            jContainer.find("ul li a").each(function () {
            	if(this.className=="prev"||this.className=="next"){
            		return;
            	}
                var p = parseInt($(this).text());
                if (p == currPage) {
                    $(this).parents("li").addClass("active");
                }
                $(this).click(function () {
                    func(p);
                });
            });

            //绑定首页事件
            //		jContainer.find(".first-page").click(function(){
            //			func(1);
            //		});	

            //绑定上一页事件
            jContainer.find(".prev").click(function () { 
            	if(currPage==1){
            		return;
            	}
                if (!this.disabled) {
                    func(currPage - 1);
                }
            });

            //绑定下一页事件
            jContainer.find(".next").click(function () {
            	if(currPage==pageN){
            		return;
            	}
                if (!this.disabled) {
                    func(currPage + 1);
                }
            });

            //绑定尾页事件
            //		jContainer.find(".last-page").click(function(){
            //			func(pageN);
            //		});	 
        },

        /**
         * 选中checkbox,则加亮显示
         * @param obj:选中的checkbox对象
         * @param tbody_id:实际绑定列表的tbody_id
         * @param cssObj:样式(默认样式为visitChecked)
         */
        brightShow: function (obj, tbody_id, cssObj) {
            if (!cssObj) {
                cssObj = "visitChecked";
            }
            var row;
            if ($(obj).parent().attr("rowid") != undefined) {
                row = $(obj).parent();
            } else if ($(obj).parent().parent().attr("rowid") != undefined) {
                row = $(obj).parent().parent();
            }
            row.find("input[type=checkbox]").each(function () {
                if ($(this).attr("checked")) {
                    row.addClass(cssObj);
                } else {
                    row.removeClass(cssObj);
                }
            });

            //如果列表中所有的行都被选中时，那么全选框也同时被选中 
            var checkObj = "#" + tbody_id + " input[type=checkbox]";
            var chooseAllObj = $("#chooseall");
            $(checkObj).click(function () {
                if (!this.checked) {
                    chooseAllObj.attr("checked", false);
                } else {
                    if ($(checkObj).size() == $(checkObj + ":checked").size()) {
                        chooseAllObj.attr("checked", "checked");
                    }
                }
            });
        },

        /** 
         * 全选、取消所有选择
         * @param obj 全选checkbox对象
         * @param bodyObj 列表主题对象
         * @param cssObj样式字符串(默认visitChecked) 
         */
        chooseall: function (obj, bodyObj, cssObj) {
            if (!cssObj)
                cssObj = "visitChecked";
            var grp_slctr = "#" + bodyObj + " input[type=checkbox]";
            if (obj.checked) {
                $(grp_slctr).attr("checked", "checked");
                //加样式
                $(grp_slctr).each(function () {
                    var row;
                    if ($(this).parent().attr("rowid") != undefined) {
                        row = $(this).parent();
                    } else if ($(this).parent().parent().attr("rowid") != undefined) {
                        row = $(this).parent().parent();
                    }
                    row.find("input[type=checkbox]").each(function () {
                        row.addClass(cssObj);
                    });
                });
            } else {
                $(grp_slctr).attr("checked", false);
                //去除样式
                $(grp_slctr).each(function () {
                    var row;
                    if ($(this).parent().attr("rowid") != undefined) {
                        row = $(this).parent();
                    } else if ($(this).parent().parent().attr("rowid") != undefined) {
                        row = $(this).parent().parent();
                    }
                    row.find("input[type=checkbox]").each(function () {
                        row.removeClass(cssObj);
                    });
                });
            }
            $(grp_slctr).click(function () {
                if (!this.checked) {
                    $(obj).attr('checked', false);
                } else {
                    if ($(grp_slctr).size() == $(grp_slctr + ":checked").size()) {
                        $(obj).attr('checked', 'checked');
                    }
                }
            });
        },
		
		
		chooseallMatch: function(obj,bodyObj,cssObj){
		if(!cssObj)
			cssObj = "visitChecked"
		var grp_slctr = "#"+bodyObj+" input[type=checkbox]";
		if(obj.checked){
			$(grp_slctr).attr("checked","checked");
			//加样式
			$(grp_slctr).each(function(){
					
			});
		}else{
			$(grp_slctr).attr("checked",false);
			//去除样式
			$(grp_slctr).each(function(){
					
			});
		}
	    $(grp_slctr).click(function(){
	        if(!this.checked){
	            $(obj).attr('checked',false);
	        }else{
	            if($(grp_slctr).size()==$(grp_slctr+":checked").size()){
	                $(obj).attr('checked','checked');
	            }
	        }
	     });
	},
		
		
        /**
         * 收集选中的IDS
         * @param jListObj
         * 			绑定数据的列表对象
         * @param attrField
         * 			需要获取属性域（不传默认就取属性为id的域）
         */
        getIds: function (jListObj, attrField) {
            if (!attrField) {
                attrField = "rowid";
            }
            var ids = [];
            jListObj.find("input[type=checkbox]:checked").each(function () {
                var row;
                if ($(this).parent().attr(attrField) != undefined) {
                    row = $(this).parent();
                } else if ($(this).parent().parent().attr(attrField) != undefined) {
                    row = $(this).parent().parent();
                }
                ids.push(row.attr(attrField));
            });
            //把收集的id转换成servlet要的字符串
            return ids.join(",");
        },

        /**
         * @param msg  提示信息
         * @param okVal 默认确定按钮值 
         * @param cancelVal 默认取消按钮值
         * @param okFuc 默认确认按钮回调函数
         * @param cancelFuc 默认取消按钮回调函数
         */
        baseDialog: function (msg, okVal, cancelVal, okFuc, cancelFuc) {
            art.dialog({
                title: '友情提示',
                content: msg,
                lock: true, //锁屏
                background: '#FFFFFF', // 背景色
                icon: 'warning',
                okVal: okVal,
                cancelVal: cancelVal,
                ok: function () {
                    okFuc();
                    return true;
                },
                cancel: function () {
                    cancelFuc();
                    return true;
                }
            });
        },
        /**
         * 点击logo跳转到首页
         */
        toFrontPage: function () {
            window.location = TDT.getAppPath("index.html");
        },
        /**
         * 模仿window.alert();
         */
        alert: function (msg, callback) {
            art.dialog({
                title: '友情提示',
                content: msg,
                icon: 'warning',
                ok: function () {
                    if (callback) {
                        callback();
                        return true;
                    }
                    return true;
                }
            });
        },

        /**
         * 模仿window.confirm();
         */
        confirm: function (msg, fun) {
            art.dialog({
                title: '友情提示',
                content: msg,
                lock: true, //锁屏
                background: '#FFFFFF', // 背景色
                icon: 'warning',
                ok: function () {
                    fun();
                    return true;
                },

                cancel: function () {
                    return true;
                }
            });
        },


        login: function () {
            /**art.dialog({
                id:"login",
                title : "",
                lock : true, //锁屏
                background: '', // 背景色
                content : '<iframe id="ifrLogin" width="392px" height="280px" frameborder="no" border=0  allowTransparency="transparent" src="'+TDT.getAppPath("logindialog.html")+'"></iframe>'
            });*/

            art.dialog({
                title: '友情提示',
                content: "您好，检测到您未登录！立即登录？",
                lock: true, //锁屏
                background: '#FFFFFF', // 背景色
                icon: 'warning',
                ok: function () {
                    var currLoc = encodeURIComponent(window.location);
                    window.location = TDT.getAppPath("index1.html?t=" + Date.parse(new Date) + "&callbackUrl=" + currLoc);
                    return true;
                },
                okVal: "去登录",
                cancel: function () {
                    return true;
                }
            });
        },

        logout: function () {
        	window.location=TDT.getAppPath("user/logout.do?t=" + Date.parse(new Date));
        	//window.location=TDT.getAppPath("j_spring_security_logout");
        	
        },

        /**
         * 平台登录统一入口
         * @param cbUrl 登录后跳转地址，为空时代表回到登录之前的页面 
         */
        ssoLogin: function (cbUrl) {
            var callbackUrl = "";
            if (cbUrl) {
                callbackUrl = encodeURIComponent(cbUrl);
            } else {
                callbackUrl = encodeURIComponent(window.location.href);
            }
            window.location.href = TDT.getAppPath("index1.html?t=" + Date.parse(new Date) + "&callbackUrl=" + callbackUrl);
        },
		
		/**
		 * 页面跳转
		 * @param url 页面的路径 
		 */
		 go:function(url){
		 		var timestr="t="+Date.parse(new Date);
		 		//qindex为查询?索引
		 		var qindex=url.indexOf("?");
		 		if(qindex>0){
		 			url+="&"+timestr
		 		}else{
		 		//jindex为#索引
		 		var jindex=url.indexOf("#");
		 			if(jindex>0){
		 				url=url.substring(0,jindex);
		 			}
		 				url+="?"+timestr;
		 		}
		 		var iframe=$(".main-page iframe");
				if(iframe.length>0){
					$(".main-page iframe").attr("src",url);
				}else{
					location.href=url;
				}
		 },

        /**
         * 菜单栏选中
         */
        menuSelected: function (appId) {
            $("#" + appId).parent().addClass("selected");
        },
        /**
         * 菜单栏选中
         */
        menuSelectedNew: function (appId) {
            $("#" + appId).children().addClass("select");
        },

        /**
         * 菜单权限控制
         * @param userInfo 用户信息（包含基本基线和权限信息）
         */
        menuAuthority: function (userInfo) {
            if (userInfo && userInfo.userName) {
                $("#dislogin").removeClass("f-dn");
                $("#disunlogin").addClass("f-dn");
                $("#dislogin .dropdown-toggle span:eq(0)").text(userInfo.userName);
                $("#userId").val(userInfo.userId);
                $("#userName").val(userInfo.loginName);
                $("#dislogin .dropdown-toggle").dropdown();
            } else {
                $("#dislogin").addClass("f-dn");
                $("#disunlogin").removeClass("f-dn");
            }
            if (userInfo && userInfo.perms.length > 0) {
                template.helper('$handleUrl', function (url, id) {
                    var handleUrl = url;
                    if (userInfo && userInfo.loginName) {
                        handleUrl = handleUrl + "?u=" + userInfo.loginName + "&t=" + Date.parse(new Date) + "&id=" + id;
                    } else {
                        handleUrl = handleUrl + "?t=" + Date.parse(new Date) + "&id=" + id;
                    }
                    return handleUrl;
                });
                var list = [];
            	for(var i=0;i<userInfo.perms.length;i++){
            		if(userInfo.perms[i].permDisplay==1){
            			list.push(userInfo.perms[i]);
            		}
            	}
                var html = template.render("headerTemplete", {
                    list: list
                });
                $("#headerList").html(html);
                $("#10001001").addClass("firstPage");
            }

        },
        /**
         * 菜单权限控制没有导航栏的
         * @param userInfo 用户信息（包含基本基线和权限信息）
         */
        menuAuthorityWithOutDaoHang: function (userInfo) {
            if (userInfo && userInfo.userName) {
                $("#dislogin").removeClass("f-dn");
                $("#disunlogin").addClass("f-dn");
                $("#dislogin .dropdown-toggle span:eq(0)").text(userInfo.userName);
                $("#dislogin .dropdown-toggle").dropdown();
            } else {
                $("#dislogin").addClass("f-dn");
                $("#disunlogin").removeClass("f-dn");
            }
            if (userInfo && userInfo.perms.length > 0) {
                template.helper('$handleUrl', function (url, id) {
                    var handleUrl = url;
                    if (userInfo && userInfo.loginName) {
                        handleUrl = handleUrl + "?u=" + userInfo.loginName + "&t=" + Date.parse(new Date) + "&id=" + id;
                    } else {
                        handleUrl = handleUrl + "?t=" + Date.parse(new Date) + "&id=" + id;
                    }
                    return handleUrl;
                });               
            }
        },
        
        /**
         * 首页菜单
         * @param userInfo 用户信息（只显示用户信息）
         */
        showUserInfo: function (userInfo) {
            if (userInfo && userInfo.userName) {
                $("#dislogin").removeClass("f-dn");
                $("#disunlogin").addClass("f-dn");
                $("#dislogin .dropdown-toggle span:eq(0)").text(userInfo.userName);
                $("#dislogin .dropdown-toggle").dropdown();
            } else {
                $("#dislogin").addClass("f-dn");
                $("#disunlogin").removeClass("f-dn");
            }
        },
        /**
         * 首页菜单权限控制
         * @param userInfo 用户信息（包含基本基线和权限信息）
         */
        menuAuthorityIndex: function (userInfo) {
            if (userInfo && userInfo.userName) {
                $("#dislogin").removeClass("f-dn");
                $("#disunlogin").addClass("f-dn");
                $("#dislogin .dropdown-toggle span:eq(0)").text(userInfo.userName);
                $("#dislogin .dropdown-toggle").dropdown();
            } else {
                $("#dislogin").addClass("f-dn");
                $("#disunlogin").removeClass("f-dn");
            }
            if (userInfo && userInfo.perms.length > 0) {           	
                template.helper('$handleUrl', function (url, id) {
                    var handleUrl = url;
                    if (userInfo && userInfo.loginName) {
                        handleUrl = handleUrl + "?u=" + userInfo.loginName + "&t=" + Date.parse(new Date) + "&id=" + id;
                    } else {
                        handleUrl = handleUrl + "?t=" + Date.parse(new Date) + "&id=" + id;
                    }
                    return handleUrl;
                });
            	var list = [];
            	for(var i=0;i<userInfo.perms.length;i++){
            		if(userInfo.perms[i].permDisplay==1&&userInfo.perms[i].permType=="Head"){
            			list.push(userInfo.perms[i]);
            		}
            	}
                var html = template.render("headerTemplete", {
                    list: list
                });
                $("#headerList").html(html);
            }

        },
		
        /**
         * 载入头尾页面
         * @param currLoc 当前位置，定位作用，如yijianfankui代表“意见反馈”
         * @param callback 回调用户信息和配置参数
         */
        loadHeaderAndFooter: function (currLoc, callBackFn) {
            //载入头部
            $("#header").load("../../inc/header.html", null, function () {
                //用户信息，控制菜单权限
                var user = new User()
                var userInfo = user.getUserInfo();
                TDT.menuAuthority(userInfo);
                //配置信息
                var sysConf = new Sysconf();
                var allConfValue = sysConf.getAllConf();
                //菜单定位
                TDT.menuSelectedNew(currLoc);
                if (typeof callBackFn == "function") {
	                callBackFn(userInfo, allConfValue);
	            }
                //绑定登录之后用户下拉事件
                //$(".action-img").click(function () {
                //    $(".action-login ul").toggle();
                //});
                /*
                $(".search-txt").live({
                    blur: function () {
                        //window.setTimeout(function(){DC.LC.hide('id_geo-search_input');},100);
                    },
                    keyup: function () {
                        //DC.LC.findQueryText(this.id,this.id,null,DC.keywordQuery.queryKey);
                    },
                    keydown: function () {
                        //DC.LC.onkeydownpage(event,this.id,this.id,DC.keywordQuery.queryKey);
                    }
                });
                $(".search-btn").live('click', function () {
                    var keyword = $(".search-txt").val();
                    window.open("../../map/page/index.html?keyword=" + keyword);
                });
                */
            });

            //载入尾部
            $("#footer").load("../../inc/footerNew.html");
            //$("#footer").load("../../inc/footer.html");
            //$("#footerWithOutR").load("../../inc/footerOut.html");
        },
        /**
         * 载入头尾页面
         * @param currLoc 当前位置，定位作用，如yijianfankui代表“意见反馈”
         * @param callback 回调用户信息和配置参数
         */
        loadHeaderAndFooter70: function (currLoc, callBackFn) {
            //用户信息，控制菜单权限
            var user = new User()
            var userInfo = user.getUserInfo();
            //TDT.menuAuthority(userInfo);
            
            if (userInfo && userInfo.userName) {
                //$("#dislogin").removeClass("f-dn");
                //$("#disunlogin").addClass("f-dn");
                //$("#dislogin .dropdown-toggle span:eq(0)").text(userInfo.userName);
                //$("#dislogin .dropdown-toggle").dropdown();
                $("#disunlogin").hide();
                $(".loginShow").show();
                $(".loginShow").css("display", "inline-block");
                $("#userInfoName").html(userInfo.userName);
            } else {
            	 $(".loginShow").hide();
                //$("#dislogin").addClass("f-dn");
                //$("#disunlogin").removeClass("f-dn");
                
            }
            if (userInfo && userInfo.perms.length > 0) {
                template.helper('$handleUrl', function (url, id) {
                    var handleUrl = url;
                    if (userInfo && userInfo.loginName) {
                        handleUrl = handleUrl + "?u=" + userInfo.loginName + "&t=" + Date.parse(new Date) + "&id=" + id;
                    } else {
                        handleUrl = handleUrl + "?t=" + Date.parse(new Date) + "&id=" + id;
                    }
                    return handleUrl;
                });
                var list = [];
            	for(var i=0;i<userInfo.perms.length;i++){
            		if(userInfo.perms[i].permDisplay==1){
            			list.push(userInfo.perms[i]);
            		}
            	}
                var html = template.render("headerTemplete", {
                    list: list
                });
                $("#headerList").html(html);
                $("#10001001").addClass("firstPage");
            }
            
            //配置信息
            var sysConf = new Sysconf();
            var allConfValue = sysConf.getAllConf();
            //菜单定位
            //TDT.menuSelectedNew(currLoc);
            $("#" + currLoc).parent().addClass("active");
            if (typeof callBackFn == "function") {
            	callBackFn(userInfo, allConfValue);
            }
            
            //载入尾部
            //$("#footer").load("../../inc/footerNew.html");
        },
        /**
         * 载入头尾页面
         * @param currLoc 当前位置，定位作用，如yijianfankui代表“意见反馈”
         * @param callback 回调用户信息和配置参数
         */
        loadHeaderAndFooter7C: function (currLoc, callBackFn) {
            //用户信息，控制菜单权限
            var user = new User()
            var userInfo = user.getUserInfo();
            if (userInfo && userInfo.userName) {
                $(".notLoggedIn").hide();
                $(".isLoggedIn").show();
                $("#userInfoName").html("欢迎您！"+userInfo.userName);
            } else {
            	 $(".isLoggedIn").hide();
            }
            if (userInfo && userInfo.perms.length > 0) {
                template.helper('$handleUrl', function (url, id) {
                    var handleUrl = url;
                    if (userInfo && userInfo.loginName) {
                        handleUrl = handleUrl + "?u=" + userInfo.loginName + "&t=" + Date.parse(new Date) + "&id=" + id;
                    } else {
                        handleUrl = handleUrl + "?t=" + Date.parse(new Date) + "&id=" + id;
                    }
                    return handleUrl;
                });
                //处理模板的iconClass
                template.helper('$iconClass', function (url, id) {
                	var iclassName = "app";
                	if(id == "10001001"){
                		iclassName = "home";
                	}else if(url.indexOf("/map") != -1){
                		iclassName = "map";
                	}else if(url.indexOf("ResourcesCenter") != -1){
                		iclassName = "res";
                	}else if(url.indexOf("thematicApp") != -1){
                		iclassName = "app";
                	}else if(url.indexOf("geomap-api") != -1){
                		iclassName = "deve";
                	}else if(url.indexOf("standards") != -1){
                		iclassName = "down";
                	}
                    return iclassName;
                });
                var list = [];
            	for(var i=0;i<userInfo.perms.length;i++){
            		if(userInfo.perms[i].permDisplay==1 && userInfo.perms[i].permType === "Head"){
            			list.push(userInfo.perms[i]);
            		}
            	}
                var html = template.render("headerTemplete", {
                    list: list
                });
                //计算宽度
                var headerListWidth = $("#headerList li").width()*list.length + 2;
                $("#headerList").width(headerListWidth);
                $("#headerList").html(html);
                $("#10001001").addClass("firstPage");
            }
            //配置信息
            var sysConf = new Sysconf();
            var allConfValue = sysConf.getAllConf();
            //菜单定位
            $("#" + currLoc).parent().addClass("select");
            if (typeof callBackFn == "function") {
            	callBackFn(userInfo, allConfValue);
            }
        },
        /**
         * 载入没有导航栏的都不尾部
         * @param currLoc 当前位置，定位作用，如yijianfankui代表“意见反馈”
         * @param callback 回调用户信息和配置参数
         */
        loadHeaderOutDHAndFooter: function (currLoc, callback) {
            //载入头部
            $("#header").load("../../inc/headerOutDH.html", null, function () {
                //用户信息，控制菜单权限
                var user = new User()
                var userInfo = user.getUserInfo();
                TDT.menuAuthorityWithOutDaoHang(userInfo);
                //配置信息
                var sysConf = new Sysconf();
                var allConfValue = sysConf.getAllConf();

                //绑定登录之后用户下拉事件
                $(".action-img").click(function () {
                    $(".action-login ul").toggle();
                });

                //菜单定位
                TDT.menuSelected(currLoc);
                $(".search-txt").live({
                    blur: function () {
                        //window.setTimeout(function(){DC.LC.hide('id_geo-search_input');},100);
                    },
                    keyup: function () {
                        //DC.LC.findQueryText(this.id,this.id,null,DC.keywordQuery.queryKey);
                    },
                    keydown: function () {
                        //DC.LC.onkeydownpage(event,this.id,this.id,DC.keywordQuery.queryKey);
                    }
                });
                $(".search-btn").live('click', function () {
                    var keyword = $(".search-txt").val();
                    window.open("../../map/page/index.html?keyword=" + keyword);
                });
                if (callback && (typeof callback != "undefined")) {
                    callback(userInfo, allConfValue);
                }
            });

            //载入尾部
            $("#footer").load("../../inc/footer.html");
        },
        
        formSubmitArg : function(id, URL, params, isAsync, success, fail){
    		//检查提交的参数值是否合法,不合法则将参数值设为空
    		URL = this.formatParam(URL);	
    		params = params == null ? "" : params;
    		var async = (typeof isAsync == "undefined") ? true : isAsync;	
    		async = async === false ? false: true;
    	    var option={
    	          url:URL,
    	          data:params,
    	          dataType:'json',
    	          contentType:'application/x-www-form-urlencoded;charset=UTF-8',
    	          async:async,
    	          success:function(json){
    	          		if(success && (typeof success != "undefined")){
    		          			if(json){
    		          			    success(json);     	     			          	     		          	     			          	   			          				          	        			          
    		          			}
    	          		  }else{
    	          		  	return json;
    	          		  }
    	          },
    	          error: function(){
    	          	if(fail && (typeof fail != "undefined")){
    	          		fail();
    	          	}
    	          }
    	    };
    	    $("#"+id).ajaxSubmit(option);
    							   	
    	},
        
        loadPage: function(documentId, pageUrl, callBackFn){
			var async = true;
			//无回调函数时，请求为同步请求。
			if(!callBackFn){
				async = false;
		    }
	        $.ajax({
	            type: "GET",
	            url: pageUrl,
	            data: "",
	            dataType: "html",
	            cache: false,
	            async: async, //false为同步请求。同步请求将锁住浏览器，用户其它操作必须等待请求完成才可以执行。
	            success: function(data){
	                $("#" + documentId).html(data);
		            if(typeof callBackFn == "function"){
		            	callBackFn();
		            }
	            },
	            error: function(XMLHttpRequest, textStatus, errorThrown){
	                result = {
	                    XMLHttpRequest: XMLHttpRequest,
	                    textStatus: textStatus,
	                    errorThrown: errorThrown
	                };
	                if(typeof failFn == "function"){
	                	failFn(result);
	                }
	            }
	        });
		},
		
		/**
		 * @method r
		 * @desc 模板渲染
		 * @return {string} 渲染结果
		 */
		r:function(tpl, data){
			var render = template.compile(tpl.replace(/^\s*|\s*$/g, ''));
			if (data) {
				return render(data);
			} else {
				return render();
			}
		},
        
        confirmModal: function(option){
			var contentHTML = option.contentHTML;
			if(contentHTML == undefined || contentHTML == ""){
				contentHTML = "是否删除该项？";
			}
			var submitBtnColor = option.submitBtnColor
			if(submitBtnColor == undefined || submitBtnColor == ""){
				submitBtnColor = "red";
			}
			var actionFn = option.actionFn;
			TDT.loadPage("confirmModalContainer", "confirmModal.htm");
			$('#confirmModal').modal('show');
			//修改确定按钮的颜色
			if(submitBtnColor == "blue"){
				$("#confirmSubmitBtn").removeClass("btn-danger").addClass("btn-primary");
			}
			if(typeof contentHTML == "string"){
				$('#confirmModal .modal-body-contentHTML').html(contentHTML);
			}
			$('#confirmSubmitBtn').off().on("click", function(){
				if(typeof actionFn == "function"){
					actionFn();
				}
				//if(actionFn instanceof Function){
				//	actionFn();
				//}
			});
			if(option.showMsg == true){
				$('#confirmSubmitBtn').html("关闭");
				$('#confirmCancelBtn').hide();
			}
		},
        
        guid: function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            }).toUpperCase();
        },
        
        random: function (num) {
        	var random = "";
        	var randArr = [];
			for(var i=0;i<num;i++){
				var randNum = Math.ceil(Math.random() * 25);
				randArr.push(String.fromCharCode(97+randNum));
			}
			for(var i=0;i<num;i++){
				random += randArr[i];
			}
			return random;
        },
        
        //目前只针对数字数组
        combineArray: function (array1, array2) {
            var array = [];
            var newarray = array1.concat(array2);
            newarray.sort();
            var map = {};
            for (var i = 0; i < newarray.length; i++) {
                if (typeof map[newarray[i]] == "undefined") {
                    map[newarray[i]] = 1;
                    array.push(newarray[i]);
                }
            }
            array.reverse();
            return array;
        },
	  
	    validate: {
	  		email: function(email){
	  			var reg = /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/i
				return reg.test(email);
	  		}
	  	}

    });

    // Expose Tianditu to the global object
    window.Tianditu = window.TDT = Tianditu;

})();

