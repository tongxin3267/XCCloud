// 后台主页   动态加载左侧菜单
    var paras = [];
    var values = [];
layui.use('form',function () {
        var form=layui.form;
        form.verify({
            username: function(value, item){ //value：表单的值、item：表单的DOM对象
                if(!new RegExp("^[a-zA-Z0-9_\u4e00-\u9fa5\\s·]+$").test(value)){
                    return '用户名不能有特殊字符';
                }
                if(/(^\_)|(\__)|(\_+$)/.test(value)){
                    return '用户名首尾不能出现下划线\'_\'';
                }
                if(/^\d+\d+\d$/.test(value)){
                    return '用户名不能全为数字';
                }
            }

            //我们既支持上述函数式的方式，也支持下述数组的形式
            //数组的两个值分别代表：[正则匹配、匹配不符时的提示文字]
            ,pass: [
                /^[\S]{6,12}$/
                ,'密码必须6到12位，且不能出现空格'
            ]
            ,required:function (value,item) {
                if(value==""){
                    return '必填项不能为空';
                }
            }
            ,number:function (value,item) {
                if(!new RegExp("^[0-9]+$").test(value)){
                    return '必须为纯数字';
                }
            }
        });
    });


//关闭弹窗
function closePage(msg) {


    layui.use('layer', function() {
        var layer = layui.layer;
        layer.closeAll();
        msg!=null&&msg!=undefined ? layer.msg(msg): '' ;

    });
}
//存储session
function setStorage(key,value) {
    if(!window.localStorage){
        layui.use('layer', function() {
            var layer = layui.layer;
            layer.msg("当前浏览器不支持该网站，为了您的体验请使用最新浏览器！")
        });
    }else {
        var storage=window.localStorage;
        storage.setItem(key,value);
    }
}
//删除session
function removeStorage(key) {
    if(!window.localStorage){
        layui.use('layer', function() {
            var layer = layui.layer;
            layer.msg("当前浏览器不支持该网站，为了您的体验请使用最新浏览器！")
        });
    }else {
        var storage=window.localStorage;
        storage.removeItem(key);
    }
}
//获取session
function getStorage(key,value) {
    if(!window.localStorage){
        layui.use('layer', function() {
            var layer = layui.layer;
            layer.msg("当前浏览器不支持该网站，为了您的体验请使用最新浏览器！")
        });
    }else {
        var storage=window.localStorage;
        value = localStorage.getItem(key);
        return value;
    }
}

//自动跳转登录
function autoReLogin() {
    layui.use('layer',function () {
        var layer=layui.layer;
            var index = layer.load(0, {shade: false});
            setTimeout(function () {
                layer.close(index);
                layer.msg('当前登录无效，将自动跳转到登录页面···')
                var parentPage=window.parent;
                if(parentPage){
                    window.parent.href='/merch/login.html';
                }else {
                    window.location.href='/merch/login.html';
                }
            },2000);
    });
}
//设置弹出框
function showLayerBox(title,content,area) {//标题：‘www’,弹出层容器id:divid, area:'1200px'
    layui.use('layer', function() {
        var layer = layui.layer;
        layer.open({
            type: 1,
            title:title,
            content: $('#'+content),
            shadeClose:true,
            area: [area]
        })
    });
}



//时间戳转日期  yy--mm--dd  hh--mm-ss
function timeStamp2String(time){
    if(time!=null){
        var datetime = new Date();
        datetime.setTime(time.substring(6,19));
        var year = datetime.getFullYear();
        var month = datetime.getMonth() + 1 < 10 ? "0" + (datetime.getMonth() + 1) : datetime.getMonth() + 1;
        var date = datetime.getDate() < 10 ? "0" + datetime.getDate() : datetime.getDate();
        var hour = datetime.getHours()< 10 ? "0" + datetime.getHours() : datetime.getHours();
        var minute = datetime.getMinutes()< 10 ? "0" + datetime.getMinutes() : datetime.getMinutes();
        var second = datetime.getSeconds()< 10 ? "0" + datetime.getSeconds() : datetime.getSeconds();
        return year + "-" + month + "-" + date+" "+hour+":"+minute+":"+second;
    }else {
        return "";
    }

}





