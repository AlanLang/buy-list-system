layui.define(['table'], function (exports) {
    "use strict";
    var table = layui.table;
    var mestable;
    var sliontable = {
        render: function (options) {
            $.ajaxSettings.async = false;
            if (!options.noprm) {
                AddBars();
            }
            $.ajaxSettings.async = true;
            var HasPage = true;
            if (options.page == false) {
                HasPage = false;
            }
             mestable = table.render({
                 elem: options.elem || '#sliontable'
                , height: options.height || GetTableHeight()
                , url: options.url
                , method: 'post'
                , page: HasPage
                , cols: options.cols
                , limit: 24
                , limits: [24, 30, 40, 50, 60, 70]
                //, initSort: (options.where.field)?null: {
                //    field: options.where.field //排序字段，对应 cols 设定的各字段名
                //   , type: options.where.order //排序方式  asc: 升序、desc: 降序、null: 默认排序
                //}
                , where: options.where
            });
            table.on('sort', function (obj) {
                options.where.order = obj.type;
                options.where.field = obj.field;
                mestable.reload({
                    initSort: obj //记录初始排序，如果不设的话，将无法标记表头的排序状态。 layui 2.1.1 新增参数
                    , where: options.where
                });
            });
            function AddBars() {
                var url = window.location.pathname;
                $.getJSON("/Home/ListMenuLimitByUser?url="+url, function (re) {
                    if (re.code == 0) {
                        var HasAdd = false;
                        $.each(re.data, function (i, that) {
                            if (that.MenuLimitCode == "add") {
                                HasAdd = true;
                                if ($("#addOne")) {
                                    $("#addOne").show();
                                }
                            } else {
                                options.cols[0].push({ title: that.MenuLimitName, toolbar: '#' + that.MenuLimitCode, align: 'center', width: 80 });
                            }
                        });
                        if (!HasAdd) {
                            if ($("#addOne")) {
                                $("#addOne").remove();
                            }
                        }
                    } else {
                        console.log("获取页面权限失败：");
                        console.log(re);
                    }
                })
            }
            return mestable;
        }
        , search: function (where, $btn) {
            var title = "";
            if ($btn) {
                title = $btn.html();
                $btn.html(loadingHtml);//改变提交按钮为loading
                setTimeout(function () { $btn.html(title); }, 5000);
            }
            return mestable.reload({
                where: where
                , page: {
                    curr: 1 //重新从第 1 页开始
                }, done: function () {
                    if ($btn) {
                        $btn.html(title);
                    }
                }
            });
        }
        , tool: function (cb) {
            table.on('tool', function (obj) { //注：tool是工具条事件名，test是table原始容器的属性 lay-filter="对应的值"
                return cb(obj);
            });
        }
        , reload: function () {
            table.reload();
        }
    };
    exports('sliontable', sliontable);
});