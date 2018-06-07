layui.define(['layer'], function(exports) {
	"use strict";
	var $ = layui.jquery,
		layer = layui.layer;
	var hasLog = true;
	var common = {
		/**
		 * 抛出一个异常错误信息
		 * @param {String} msg
		 */
		throwError: function(msg) {
			throw new Error(msg);
			return;
		},
		/**
		 * 弹出一个错误提示
		 * @param {String} msg
		 */
		msgError: function(msg) {
			layer.msg(msg, {
			    icon: 2
                ,anim:6
			});
			return;
		}
        , msg: function (msg, cb) {
            layer.msg(msg, { icon: 1, time: 800, anim: 0 }, function () {
                if (cb && cb()) {
                    cb();
                }
            });
        }
        , info: function (msg) {
            layer.msg(msg, { icon: 2, time: 800 });
        }
        , info: function (msg, cb) {
            layer.msg(msg, { icon: 2, time: 800 }, function () {
                if (cb && cb()) {
                    cb();
                }
            });
        }
        , loadingMsg: function (msg) {
            return layer.msg(msg, {
              icon: 16
            , time: -1
            , fixed: false
            , shade: [0.3, '#393D49']
            });
        }
        , loading: function (msg) {
            if (msg) {
                return layer.msg(msg, {
                    icon: 16
                    , time: -1
                    , fixed: false
                    , shade: [0.3, '#393D49']
                });
            }
            return layer.load(1, { time: 20 * 1000 });
        }
        , close: function (index) {
            layer.close(index)
        }
        , deleteConfirm: function (msg,cb) {
            layer.confirm(msg, { icon: 3, title: '删除提示',anim:2 }, function (index) {
                //do something
                cb();
                layer.close(index);
            });
        }
        , conform: function (msg,title, cb) {
            layer.confirm(msg, { icon: 3, title: title, anim: 2 }, function (index) {
                //do something
                cb();
                layer.close(index);
            });
        }
        , open: function (title,content) {
            return layer.open({
                title:title,
                type: 1,
                shade: 0.8,
                area: '900px',
                content: $(content) //这里content是一个DOM，注意：最好该元素要存放在body最外层，否则可能被其它的相对元素所影响
            });
        }
        , iframe: function (title, url) {
            return layer.open({
                type: 2,
                title: title,
                shade: 0.8,
                offset: '200px',
                area: '900px',
                content: url,
                end: function () {
                }})
        }
        , iframeArea: function (title, url,width,height) {
            return layer.open({
                type: 2,
                title: title,
                shade: 0.8,
                offset: '200px',
                area: [width, height],
                content: url,
                end: function () {
                }
            })
        }
        , log: function (msg) {
            if (hasLog) {
                console.log(msg);
            }
        }
        , logErr: function (msg) {
            if (hasLog) {
                console.log('%c' + msg, 'color:red');//格式输出
            }
        }
	};
	exports('common', common);
});