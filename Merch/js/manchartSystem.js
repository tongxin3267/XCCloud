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

function getPara() {
    var url = window.location.href;  //获取网址字符串
    url=decodeURIComponent(url);
    var len = url.length;
    var offset = url.indexOf("?");
    var str = url.substr(offset+1,len);
    var args = str.split("&");
    len = args.length;
    for (var i = 0; i < len; i++) {
        str = args[i];
        var arg = str.split("=");
        if (args.length < 1) {break;}
        else if(paras[i]!=""&&values[i]!=""){
            paras[i] = arg[0];      //参数名
            values[i] = arg[1];     //参数值
        }
    }
}
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
//设置左侧菜单
function setLeftList(id) {
    var obj={'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
    var url="/XCCloud/Main?action=GetMenus";
    var parasJson = JSON.stringify(obj);
    var ulBox=$('#'+id);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: { parasJson: parasJson },
        success: function (data) {
            data=JSON.parse(data);
            if(data.result_code=="1"){
               var data1=data.result_data[0].children;
               // console.log(data1);
               var arr=[];
                   for(var j in data1){
                       if(data1[j].children.length >0){
                           ulBox.append("<li class='layui-nav-item'>" +
                               "<a class='' href='javascript:;'>"+data1[j].name+"</a>"+
                               "<dl class='layui-nav-child'>"+
                               (function(){
                                   var arr="";
                                   for(var i in data1[j].children){
                                       arr+="<dd><a class='pageA' href='javascript:;'>"+data1[j].children[i].name+"</a></dd>";
                                   }
                                   return arr;
                               })() + "</dl></li>")
                       }else {
                           ulBox.append("<li class='layui-nav-item'>" +
                               "<a class='pageA' href='javascript:;'>"+data1[j].name+"</a>"+
                              "</li>")
                       }

                         }
                   var i=1;
                       $('#merchantMenu').find('a[class="pageA"]').each(function () {
                           $(this).attr('id','sum'+i);
                           i++;
                       });
                       addIframePage('a[class="pageA"]');
                layui.use('element', function(){
                    var element = layui.element;
                    element.render('nav');
                });

            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }else {
                // alert("添加失败，请核对后再次提交！");
                // console.log(data.result_msg);
                layui.use('layer', function(){
                    var layer = layui.layer;
                    layer.msg('操作失败');
                });
            }
        }
    });

}
//退出
function escPage() {
    layui.use('layer',function () {
        var layer=layui.layer;
        layer.confirm('确定退出当前系统？', {
            btn: ['确定','点错了，回到本页面'] //按钮
        }, function(){
            var userAgent = navigator.userAgent;
            if (userAgent.indexOf("Firefox") != -1 || userAgent.indexOf("Chrome") !=-1) {
                window.location.href="about:blank";
            } else {
                window.opener = null;
                window.open("", "_self");
                window.close();
            }
            window.localStorage.clear();
        }, function(){
            layer.close();
        });
    })
}
//手动重新登录
function reLogin() {
    layui.use('layer',function () {
        var layer=layui.layer;
        layer.confirm('是否返回登录？', {
            btn: ['确定','点错了，回到本页面'] //按钮
        }, function(){
            var index = layer.load(0, {shade: false});
            setTimeout(function () {
                layer.close(index);
                var parentPage=window.parent;
               // console.log(parentPage);
                    window.location.href='/merch/login.html';

            },2000);
        }, function(){
            layer.close();
        });
    });
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
//初始化表格
function getInitData(parm) {
    var tableData=[],obj=parm.obj,url =parm.url;
    $.ajax({
        type: "post", url: url,
        contentType: "application/json; charset=utf-8",
        data: { parasJson: JSON.stringify(obj) },
        success: function (data) {
            data=JSON.parse(data);
            // console.log(data);
            if(data.result_code=="1"||data.Result_Code=="1"){
                tableData=data.result_data||data.Result_Data;
                layui.use(['table','layer'], function(){
                    var table = layui.table;
                    var layer=layui.layer;
                    var index = layer.load(0, {shade: false});
                    setTimeout(function () {
                        layer.close(index);
                    },1000);
                    table.render({
                        elem:parm.elem
                        ,data:tableData
                        ,cellMinWidth: 120 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
                        ,cols:[parm.cols]
                        ,page:{page:true,limits:[10,15,20,30,50,100]}
                        ,limit:10
                        ,done:parm.done
                    });
                });
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }else {
                // alert("添加失败，请核对后再次提交！");
                layui.use(['layer','jquery'], function () {
                    var layer = layui.layer;
                    var $=layui.jquery;
                    layer.msg(data.result_msg);
                });
                if(data.result_msg=='当前用户无权访问'){
                    $('.addbtn').addClass('layui-btn-disabled').attr('disabled','true');
                }else {
                    $('.addbtn').removeClass('layui-btn-disabled').attr('disabled','false');
                }
            }
        }
    })
}
// 查询模板layui-btn_serchModel
function searchModel() {
    var token=getStorage('token');
    var url='/Query?action=init',
        obj={"sysId": "0", "versionNo": "0.0.0.1",
        'pagename':'storeSearch','processname':'storeSearch',"token": token, "signkey": "1f626576304bf5d95b72ece2222e42c3"
    };
    var  parasJson = JSON.stringify(obj);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        // dataType:'json',
        data:parasJson,
        success: function (data) {
            data=JSON.parse(data);
            // console.log(data);

          if(data.result_code=='1'){
            var arrJson=data.result_data;
            var arr=[];
              for (var i = 0; i < arrJson.length; i++) {arr.push({
                  id: i,
                  name: arrJson[i].title,
                  type: arrJson[i].type,
                  list: arrJson[i].list,
                  iscolume: arrJson[i].iscolume,
                  issearch: arrJson[i].issearch,
                  filed:arrJson[i].filed
              });}
            var arrTitleList=[],arrIsShow=[],content='';
            content+='<div class="layui-form-item"><label class="layui-form-label">查询条件</label><div class="layui-input-block"><div id="ms2"></div></div></div>' +
                '<div class="layui-form-item searchInput"></div>';
              // console.log(content);
              layui.use(['layer'],function () {
                  var layer=layui.layer;
                  layer.open({
                      type: 1,
                      maxmin: true,
                      title:'高级查询',
                      area:['1000px','600px'],
                      content:content,
                      shadeClose:true
                  });
              })
              var ms2 = $('#ms2').magicSuggest({
                  data: arr,
                  maxSelection: 100,
                  editable: false
              });

              for(var i=0;i<arr.length;i++){
                  if(arr[i].issearch==1){
                      if(arr[i].type=='string'||arr[i].type=='number'){
                          $('.searchInput').append('<div class="layui-inline layui-hide" style="width: 480px" name="'+arr[i].name+'"><label class="layui-form-label">'+arr[i].name+'</label>' +
                              '<div class="layui-input-inline"><input type="text" id="'+arr[i].field+'" class="layui-input"></div></div>');
                      }else if(arr[i].type=='numbers'){
                          $('.searchInput').append('<div class="layui-inline layui-hide" style="width: 480px" name="'+arr[i].name+'"><label class="layui-form-label">'+arr[i].name+'</label><div class="layui-input-inline" style="width: 100px;">' +
                              '      <input type="text" name="price_min" placeholder="start" autocomplete="off" class="layui-input">' +
                              '    </div>' +
                              '    <div class="layui-form-mid">-</div>' +
                              '    <div class="layui-input-inline" style="width: 100px;">' +
                              '      <input type="text" name="price_max" placeholder="end" autocomplete="off" class="layui-input">' +
                              '    </div></div>')
                      }else if(arr[i].type=='datetime'){
                          $('.searchInput').append('<div class="layui-inline layui-hide" style="width: 480px" name="'+arr[i].name+'"><label class="layui-form-label">'+arr[i].name+'</label>' +
                              '<div class="layui-input-inline"><input type="text" id="datetime" class="layui-input"></div></div>')

                      }else if(arr[i].type=='datetimes'){
                          $('.searchInput').append('<div class="layui-inline layui-hide" style="width: 480px" name="'+arr[i].name+'"><label class="layui-form-label">'+arr[i].name+'</label>' +
                              '<div class="layui-input-inline"><input type="text" id="datetimes" class="layui-input"></div></div>')
                      }else if(arr[i].type=='date'){
                          $('.searchInput').append('<div class="layui-inline layui-hide" style="width: 480px" name="'+arr[i].name+'"><label class="layui-form-label">'+arr[i].name+'</label>' +
                              '<div class="layui-input-inline"><input type="text" id="date" class="layui-input"></div></div>')
                      }else if(arr[i].type=='dates'){
                          $('.searchInput').append('<div class="layui-inline layui-hide" style="width: 480px" name="'+arr[i].name+'"><label class="layui-form-label">'+arr[i].name+'</label>' +
                              '<div class="layui-input-inline"><input type="text" id="dates" class="layui-input"></div></div>')
                      }else if(arr[i].type=='literals'){
                          $('.searchInput').append('<div class="layui-inline layui-hide" style="width: 480px" name="'+arr[i].name+'"><label class="layui-form-label">'+arr[i].name+'</label>' +
                              '<div class="layui-input-inline"><select id="'+arr[i].field+'" class="layui-input"></select></div></div>')
                      }
                  }
              }
          }
            layui.use(['layer','form','laydate'],function () {
                var layer=layui.layer;
                var laydate=layui.laydate;
                laydate.render({
                    elem: '#datetime',type: 'datetime'
                });
                laydate.render({
                    elem: '#datetimes',type: 'datetime',range: true
                });
                laydate.render({
                    elem: '#date',type: 'date'
                });
                laydate.render({
                    elem: '#dates',type: 'date',range: true
                });
            });
            $('#ms-res-ctn-0 div').on('click',function () {
                // console.log($(this));
              var _text=$(this).text();
              $('.searchInput').find('div[name="'+_text+'"]').removeClass('layui-hide').addClass('needSubmit');
            });
            $('#ms-sel-ctn-0').on('click','div',function () {
                // console.log($(this));
                // console.log($(this).text());
                // console.log($(this).children('span'));
            })
        }
    })
}


//设置地图标记
function setSign(a,b,c,d,id) {
    var map = new BMap.Map(id);
    var point = new BMap.Point(a,b);
    map.centerAndZoom(point, 15);
    var marker = new BMap.Marker(point);  // 创建标注
    map.addOverlay(marker);              // 将标注添加到地图中
    var opts = {
        width : 200,     // 信息窗口宽度
        height: 100,     // 信息窗口高度
        title : "门店地址" , // 信息窗口标题
        enableMessage:true,//设置允许信息窗发送短息
        message:"欢迎到本店体验！"
    };
    var infoWindow = new BMap.InfoWindow("地址："+c+"<br/>"+"电话："+d, opts);  // 创建信息窗口对象
    marker.addEventListener("click", function(){
        map.openInfoWindow(infoWindow,point); //开启信息窗口
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
//时间戳转日期  yy--mm--dd
function dateStamp(time){
    if(time!=null){
        var datetime = new Date();
        datetime.setTime(time.substring(6,19));
        var year = datetime.getFullYear();
        var month = datetime.getMonth() + 1 < 10 ? "0" + (datetime.getMonth() + 1) : datetime.getMonth() + 1;
        var date = datetime.getDate() < 10 ? "0" + datetime.getDate() : datetime.getDate();
        var hour = datetime.getHours()< 10 ? "0" + datetime.getHours() : datetime.getHours();
        var minute = datetime.getMinutes()< 10 ? "0" + datetime.getMinutes() : datetime.getMinutes();
        var second = datetime.getSeconds()< 10 ? "0" + datetime.getSeconds() : datetime.getSeconds();
        return year + "-" + month + "-" + date;
    }else {
        return "";
    }
}
//添加子菜单对应的页面
function addIframePage(node) {

        $("#merchantMenu").find(node).on('click',function() {
            var _text=$(this).text();
            var txt2 = $('<li class="now"><i class="layui-icon" style="color: red;font-size: 16px;' +
                'position: absolute;top: 7px;right:0;margin-left: 3px;cursor: pointer">&#xe640</i><em><a href="#javascript;" name="'+_text+'">'+_text+'</a></em></li>');
                var flag=false;
                var _index="";
                var _length=$(".titleAppend ul").find("li em").length;
            for (var i=0;i<$(".titleAppend ul").find("li em").length;i++){
                if($(".titleAppend ul").find("li em").eq(i).children('a').text()==_text){
                    flag=true;
                    _index=i;
                     }
                  }
               if(flag==true) {
                   $(".titleAppend ul").find("li").eq(_index).addClass("now").siblings().removeClass("now");
               }else {
                   $(".titleAppend ul").append(txt2);
                   $(".titleAppend ul").find("li").eq(_length).addClass("now").siblings().removeClass("now");
               }
            newIframe(_text);
    });
    $(".titleAppend ul").on("click", "li  em", function() {
        $(this).parent("li").addClass("now").siblings().removeClass("now");
        var dd3 = $(this).children('a').text();
        if(dd3=='系统首页'){
            $('iframe[id="iFrame0"]').show().siblings().hide();
        }else {
            $('iframe[name="'+dd3+'"]').show().siblings().hide();
        }
    });
    $(".titleAppend ul").on("click", " li i", function() {
        var _index=$(this).parent('li').index();
        if(_index>1){
        var _text= $('.titleAppend ul').find('li').eq(_index-1).children('em').children('a').text();
            $('iframe[name="'+_text+'"]').show().siblings().hide();
        }else {
            $('iframe[id="iFrame0"]').show()
        }
        var dd3 = $(this).siblings("em").children('a').text();
        $(this).parents("li").remove();
        $('iframe[name="'+dd3+'"]').remove();
    });
    $("#merchantMenu").on("click", "li", function() {
        var liLength = $(".titleAppend li").length;
        if (liLength > 10) {
            $(".titleAppend ul li").eq(1).remove()
        }
    });
}
//登陆页面--login.html
function login_merch(){
    var tokens=getStorage('token');
    var username=$('#username_login').val();
    var password=$('#password_login').val();
    var obj={'userName':username,'password':password,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
    var url='/XCCloud/Login?action=CheckUser';
    var parasJson = JSON.stringify(obj);
    removeStorage('token');
    removeStorage('logType');
    if(username!=''&&password!=''){
        $.ajax({
            type: "post",
            url: url,
            contentType: "application/json; charset=utf-8",
            data: { parasJson: parasJson },
            success: function (data) {
                data=JSON.parse(data);
                console.log(data);
                if(data.result_code==1){
                    setStorage('token',data.result_data.token);
                    setStorage('logType',data.result_data.logType);
                    setStorage('merchTag',data.result_data.merchTag);
                    setStorage('userType',data.result_data.userType);
                    setStorage('usernames',username);
                    if(data.result_data.logType==2){
                        window.location.href='indexStore.html?'+(Date.parse(new Date())/1000);
                    }else {
                        window.location.href='index1.html?'+(Date.parse(new Date())/1000);
                    }
                }
            }
        })
    }
}
function changeLoginWay(){
    if(clickCounts%2==0){
        $('.tips-img img').attr('src','images/password.png');
        $('.login-code').removeClass('layui-hide').siblings('form').addClass('layui-hide');
    }else{
        $('.tips-img img').attr('src','images/code.png');
        $('.login-pwd').removeClass('layui-hide').siblings('form').addClass('layui-hide');
    }
    clickCounts++;
}
function ifShowPassword(){
    if(ifshow%2==1){
        $('#password_login').addClass('layui-hide');
        $('#password_login_hide').removeClass('layui-hide');
    }else{
        $('#password_login_hide').addClass('layui-hide');
        $('#password_login').removeClass('layui-hide');
    }
    ifshow++;
}
//index.html页面
function newIframe(text) {
        if(text=='商户列表'){
            var isTrue=  $('.test2').find('iframe[id="merchPage"]');
            if(isTrue.length>0){
                $("iframe[id='merchPage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="merchPage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="merchantList.html"></iframe>');
                $("iframe[id='merchPage']").show().siblings().hide();
            }
        } else if(text=='商户信息查询'){
        var isTrue=  $('.test2').find('iframe[id="otherMerchPage"]');
        if(isTrue.length>0){
            $("iframe[id='otherMerchPage']").show().siblings().hide();
        }else {
            $('.test2').append('<iframe id="otherMerchPage" name="'+text+'" width="100%" height="100%" ' +
                'frameborder="0" seamless src="otherMerch.html"></iframe>');
            $("iframe[id='otherMerchPage']").show().siblings().hide();
        }
    }else if(text=='门店明细'||text=='门店信息维护'){
            var isTrue=  $('.test2').find('iframe[id="storeListPage"]');
            if(isTrue.length>0){
                $("iframe[id='storeListPage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="storeListPage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="storeList.html"></iframe>');
                $("iframe[id='storeListPage']").show().siblings().hide();
            }
        }else if(text=='待审核门店'){
            var isTrue=  $('.test2').find('iframe[id="auditStorePage"]');
            if(isTrue.length>0){
                $("iframe[id='auditStorePage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="auditStorePage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="auditStore.html"></iframe>');
                $("iframe[id='auditStorePage']").show().siblings().hide();
            }
        }else if(text=='员工用户列表'){
            var isTrue=  $('.test2').find('iframe[id="weChatUserListPage"]');
            if(isTrue.length>0){
                $("iframe[id='weChatUserListPage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="weChatUserListPage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="weChatUserList.html"></iframe>');
                $("iframe[id='weChatUserListPage']").show().siblings().hide();
            }
        }else if(text=='工作组管理'){
            var isTrue=  $('.test2').find('iframe[id="workGroupManagePage"]');
            if(isTrue.length>0){
                $("iframe[id='workGroupManagePage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="workGroupManagePage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="workGroupManage.html"></iframe>');
                $("iframe[id='workGroupManagePage']").show().siblings().hide();
            }
        }else if(text=='系统参数配置'||text=='运营参数设置'){
            var isTrue=  $('.test2').find('iframe[id="parameterSettingPage"]');
            if(isTrue.length>0){
                $("iframe[id='parameterSettingPage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="parameterSettingPage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="parameterSetting.html"></iframe>');
                $("iframe[id='parameterSettingPage']").show().siblings().hide();
            }
        }else if(text=='系统关键字维护'){
            var isTrue=  $('.test2').find('iframe[id="xtreePage"]');
            if(isTrue.length>0){
                $("iframe[id='xtreePage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="xtreePage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="layui-tree2.0/xtree.html"></iframe>');
                $("iframe[id='xtreePage']").show().siblings().hide();
            }
        }else if(text=='员工列表'){
            var isTrue=  $('.test2').find('iframe[id="employerAccountManagePage"]');
            if(isTrue.length>0){
                $("iframe[id='employerAccountManagePage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="employerAccountManagePage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="employerAccountManage.html"></iframe>');
                $("iframe[id='employerAccountManagePage']").show().siblings().hide();
            }
        }else if(text=='账目审核'){
            var isTrue=  $('.test2').find('iframe[id="accountCheckPage"]');
            if(isTrue.length>0){
                $("iframe[id='accountCheckPage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="accountCheckPage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="accountCheck.html"></iframe>');
                $("iframe[id='accountCheckPage']").show().siblings().hide();
            }
        }else if(text=='小程序海报推送'||text=='小程序推广活动设置'){
            var isTrue=  $('.test2').find('iframe[id="weChatActivePage"]');
            if(isTrue.length>0){
                $("iframe[id='weChatActivePage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="weChatActivePage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="weChatActive.html"></iframe>');
                $("iframe[id='weChatActivePage']").show().siblings().hide();
            }
        }else if(text=='互联网支付流水单'){
            var isTrue=  $('.test2').find('iframe[id="threadPayListsPage"]');
            if(isTrue.length>0){
                $("iframe[id='threadPayListsPage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="threadPayListsPage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="threadPayLists.html"></iframe>');
                $("iframe[id='threadPayListsPage']").show().siblings().hide();
            }
        }else if(text=='多门店连锁配置'){
            var isTrue=  $('.test2').find('iframe[id="chainSettingPage"]');
            if(isTrue.length>0){
                $("iframe[id='chainSettingPage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="chainSettingPage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="chainSetting.html"></iframe>');
                $("iframe[id='chainSettingPage']").show().siblings().hide();
            }
        }
        else {
            var isTrue=  $('.test2').find('iframe[id="toFinshPage"]');
            if(isTrue.length>0){
                $("iframe[id='toFinshPage']").show().siblings().hide();
            }else {
                $('.test2').append('<iframe id="toFinshPage" name="'+text+'" width="100%" height="100%" ' +
                    'frameborder="0" seamless src="toFinsh.html"></iframe>');
                $("iframe[id='toFinshPage']").show().siblings().hide();
            }
        }
}
//...............................................小程序活动推送.............................................................
<!--加载所有海报-->
function showAllPoster() {
    let titles=$('#active-title').val();let times=$('#active-time').val();
    var obj={'title':titles,'publishDate':times,'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
    var url='/XCCloud/Bill?action=GetBills';
    var parasJson = JSON.stringify(obj);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: {parasJson: parasJson},
        success: function (data) {
            data = JSON.parse(data);
            if(data.result_code==1){
                $('.active-list').html('');
                let arr=data.result_data;
                for (let i in arr){
                    $('.active-list').append('<li class="active_lists" name="'+arr[i].ID+'"><img src="'+arr[i].PicturePath+'" alt="">' +
                        '<p class="publish_title">'+arr[i].Title+'</p>' +
                        '<div><p class="publish_time">'+timeStamp2String(arr[i].Time)+'</p><i class="layui-icon del_active">&#xe640</i></div></li>')
                }
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }
        }
    })
}
//编辑添加新海报



//单项查询
function searchOne(searchText,searchObj){
    var activeLists=document.getElementsByClassName("active_lists");
    var searchTexts=document.getElementById(searchText).value;
    var textsAll=document.getElementsByClassName(searchObj);
    for(var i=0;i<textsAll.length;i++){
        activeLists[i].style.display="none";
        if(textsAll[i].innerHTML.indexOf(searchTexts)!=-1){
            activeLists[i].style.display="inline-block";
        }
    }
}
//查询 标题和日期
function searchTime_title() {
    var titlesArr=[];
    var activeLists=document.getElementsByClassName("active_lists");
    var searchTitle=document.getElementById("active-title").value;
    var searchTime=document.getElementById("active-time").value;

    var titlesAll=document.getElementsByClassName("publish_title");
    var timesAll=document.getElementsByClassName("publish_time");

    for(var i=0;i<titlesAll.length;i++){
        if(titlesAll[i].innerHTML.indexOf(searchTitle)!=-1){
            titlesArr.push(activeLists[i]);
        }
    }
    for(var i=0;i<titlesArr.length;i++){
        titlesArr[i].style.display="none";
        if(titlesArr[i].getElementsByClassName("publish_time")[0].innerHTML.indexOf(searchTime)!=-1){
            titlesArr[i].style.display="inline-block";
        }
    }
}
// 浏览器 单项 -->删除
function deleteOne(searchText,searchObj){
    var activeLists=document.getElementsByClassName("active_lists");
    var searchTexts=document.getElementById(searchText).value;
    var textsAll=document.getElementsByClassName(searchObj);
    var arrrr=[],arrName=[];

    for(var i=0;i<textsAll.length;i++){
        if(textsAll[i].innerHTML.indexOf(searchTexts)!=-1){
            arrrr.push(activeLists[i]);
            arrName.push(activeLists[i].getAttribute('name'));
        }
    }
    for(var i=0;i<arrrr.length;i++){
        arrrr[i].parentNode.removeChild(arrrr[i]);
    }
}
function deletePosterById(ID) {
    var obj={'ID':ID,'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
    var url='/XCCloud/Bill?action=DeleteBill';
    var parasJson = JSON.stringify(obj);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: {parasJson: parasJson},
        success: function (data) {
            data = JSON.parse(data);
            if(data.result_code==1){
                layui.use('layer', function() {
                    var layer = layui.layer;
                    layer.load(1);
                });
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }
        }
    })
}
//查询 标题和日期 --> 删除
function deleteTime_title() {
    var titlesArr=[];
    var arrrr=[];
    var activeLists=document.getElementsByClassName("active_lists");
    var searchTitle=document.getElementById("active-title").value;
    var searchTime=document.getElementById("active-time").value;
    var titlesAll=document.getElementsByClassName("publish_title");
    var timesAll=document.getElementsByClassName("publish_time");

    for(var i=0;i<titlesAll.length;i++){
        if(titlesAll[i].innerHTML.indexOf(searchTitle)!=-1){
            titlesArr.push(activeLists[i]);
        }
    }
    for(var i=0;i<titlesArr.length;i++){
        if(titlesArr[i].getElementsByClassName("publish_time")[0].innerHTML.indexOf(searchTime)!=-1){
            arrrr.push(titlesArr[i]);
        }
    }
    for(var i=0;i<arrrr.length;i++){
        arrrr[i].parentNode.removeChild(arrrr[i]);
    }
}
function search(){
    var searchTitle=document.getElementById("active-title").value;
    var searchTime=document.getElementById("active-time").value;
    if(searchTitle!=""&&searchTime==""){
        searchOne('active-title','publish_title');
    } else if(searchTitle==""&&searchTime!=""){
        searchOne('active-time','publish_time');
    }else if(searchTitle!=""&&searchTime!=""){
        searchTime_title();
    }
}
function del(){
    var searchTitle=document.getElementById("active-title").value;
    var searchTime=document.getElementById("active-time").value;
    if(searchTitle!=""&&searchTime==""){
        deleteOne('active-title','publish_title');
    } else if(searchTitle==""&&searchTime!=""){
        deleteOne('active-time','publish_time');
    }else if(searchTitle!=""&&searchTime!=""){
        deleteTime_title();
    }
}

function sureDel() {
    $('.delete_tips_box').css("display","none");
    del();
}
function cancelDel() {
    $('.delete_tips_box').css("display","none");
}
//................................................系统关键字请求权限.................................................................
//给select添加option
function setSelect(objVal,id,m) {
   var  obj={"dictKey":objVal,"userToken":token,"signkey":"1f626576304bf5d95b72ece2222e42c3"};
   var url="/XCCloud/Dictionary?action=GetNodes";

   var parasJson = JSON.stringify(obj);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: { parasJson: parasJson },
        success: function (data) {
            data=JSON.parse(data);
            if(data.result_code=="1"){
                var arr=data.result_data;
                $('#'+id).html('<option value="">-请选择-</option>');
                for(i in arr){
                    $('#'+id).append("<option value='"+arr[i].dictValue+"'>"+arr[i].name+"</option>");
                }
                var _text= $('#'+id).find('option[value='+m+']').text();
                $('#'+id).find('option[value='+m+']').remove();
                $('#'+id).append("<option value='"+m+"' selected>"+_text+"</option>");
                layui.use(['form'], function() {
                    var form = layui.form;
                    form.render('select');
                });

            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }else {
                // alert("添加失败，请核对后再次提交！");
                layui.use(['layer'], function() {
                    var layer = layui.layer;
                    layer.msg('数据加载失败！');
                });
            }
        }
    })
}
//获取新增用户 信息列表

//删除表格中一条数据
function deleteTableList(parm) {

    var url = parm.url;
    var parasJson = JSON.stringify(parm.obj);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: {parasJson: parasJson},
        success: function (data) {
            data = JSON.parse(data);
            layui.use('layer',function () {
            var layer=layui.layer;
            var index = layer.load(1, {shade: [0.1,'#fff']});
            if(data.result_code==1){
                    setTimeout(function () {
                        layer.close(index);
                        layer.msg("成功删除！")
                    },1000);

            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }else {
                setTimeout(function () {
                    layer.close(index);
                    layer.msg("操作失败！")
                },1000);
            }
                });
        }
    })
}
//...................................................page门店列表...................................
//添加门店
function addStore() {
       var storeId='';
       var parentId=$('#parentId option:selected').val();
       var storeName=$('#storeName').val();
       var password=$('#lookPassword').val();
       var authorExpireDate=$('#authorExpireDate').val();
        var Address=$('#Address').val();
        var lng=$('#lng').val();
        var lat=$('#lat').val();
        var contracts=$('#contracts').val();

        var idCard=$('#idCard').val();
        var idExpireDate=$('#idExpireDate').val();
        var mobile=$('#mobile').val();
        //门头照片
         var shopSignPhoto=$('.layui-upload-img_md').attr('src');
         //营业执照照片
         var licencePhoto=$('.layui-upload-img_md1').attr('src');
        var licenceId=$('#licenceId').val();
        var licenceExpireDate=$('#licenceExpireDate').val();
        var bankType=bankTypes;
        var bankCode=$('#bankCode').val();
        var bankAccount=$('#bankAccount').val();
        var selttleType=selttleTypes;

        var obj={'storeId':storeId,'parentId':parentId,'storeName':storeName,'password':password,'authorExpireDate':authorExpireDate,
        'Address':Address,'lng':lng,'lat':lat,'contracts':contracts,'idCard':idCard,'idExpireDate':idExpireDate,
        'mobile':mobile,'shopSignPhoto':shopSignPhoto,'licencePhoto':licencePhoto,
        'licenceId':licenceId,'licenceExpireDate':licenceExpireDate,'bankType':bankType,'bankCode':bankCode,
        'bankAccount':bankAccount,'selttleType':selttleType,'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
        var url='/XCCloud/StoreInfo?action=SaveStoreInfo';
        var parasJson = JSON.stringify(obj);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: {parasJson: parasJson},
        success: function (data) {
            data = JSON.parse(data);
            if(data.result_code==1){
                layui.use('layer', function(){
                    var layer = layui.layer;
                    layer.msg('操作成功');
                });
                closePage();
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }else {
                layui.use(['layer'], function() {
                    var layer = layui.layer;
                    layer.msg(data.result_msg);
                });
            }
            }
        })
    }
//获取支付方式参数
function getSettleFields(selttleType) {
    //若selttleType=1,成员：
    var WXPayOpenID=$('#WXPayOpenID').val();
    var WXName =$('#WXName').val();
    var AliPay =$('#AliPay').val();
    var AliPayName =$('#AliPayName ').val();
    var SettleFee1 =$('#SettleFee1').val();
    var SettleCycle =$('#SettleCycle').val();
    var SettleCount =$('#SettleCount').val();

    //若selttleType=2,成员：
    var MerchNo  =$('#MerchNo').val();
    var TerminalNo  =$('#TerminalNo').val();
    var Token =$('#Token').val();
    var InstNo=$('#InstNo').val();
    var SettleFee2 =$('#SettleFee2').val();

    if(selttleType==1){
        settleFields={'WXPayOpenID':WXPayOpenID,'WXName':WXName,'AliPay':AliPay,'AliPayName':AliPayName,
        'SettleFee':SettleFee1,'SettleCycle':SettleCycle,'SettleCount':SettleCount};
    }else if(selttleType==2){
        settleFields={'MerchNo':MerchNo,'TerminalNo':TerminalNo,'Token':Token,'InstNo':InstNo,
            'SettleFee':SettleFee2};
    }
    else  if(selttleType==0){
        settleFields={};
    }
    return settleFields;
}
//...................................................用户权限设置wechatUserList.html...................................
//获取微信用户列表
function getWXUserList() {
    $('.wchatUser').html("");
    var url='/wx/Users?action=GetWxUserInfoBatch';
    var obj={"sysId": "0", "versionNo": "0.0.0.1"};
    $.ajax({
        type:'post',
        dataType:'json',
        url:url,
        data:obj,
        success:function (data) {
            if(data.result_code==1){
                var arr=data.result_data;
                for(var i in arr){
                    if(arr[i].userId!=null){
                        $('.wchatUser').append('<li class="wchatUserLists" name="'+arr[i].userId+'"><img src="'+arr[i].headimgurl+'" alt="" class="wchatUserImg">' +
                            '<p class="nickName">昵称：'+arr[i].nickname+'</p>' +
                            '<p class="empStatus">状态：'+arr[i].subscribe+'</p>'+
                            '<p class="WXopenId layui-hide">'+arr[i].openid+'</p>'+
                            '<p class="WXunionId layui-hide">'+arr[i].unionid+'</p>'+
                            '<div style="text-align: center"><button type="button" class="layui-btn layui-btn-sm editBtn layui-bg-orange" name="'+arr[i].userId+'">编辑</button>' +
                            '<button type="button" class="layui-btn layui-btn-sm layui-btn-normal">其他</button></div></li>')
                    }else {
                        $('.wchatUser').append('<li class="wchatUserLists" name="'+arr[i].userId+'"><img src="'+arr[i].headimgurl+'" alt="" class="wchatUserImg">' +
                            '<p class="nickName">昵称：'+arr[i].nickname+'</p>' +
                            '<p class="empStatus">状态：'+arr[i].subscribe+'</p>'+
                            '<p class="WXopenId layui-hide">'+arr[i].openid+'</p>'+
                            '<p class="WXunionId layui-hide">'+arr[i].unionid+'</p>'+
                            '<div style="text-align: center"><button type="button" class="layui-btn layui-btn-sm addBtn" name="'+arr[i].userId+'">新增成员</button>' +
                            '<button type="button" class="layui-btn layui-btn-sm layui-btn-normal">其他</button></div></li>')
                    }
                }
                setUserAuth();
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }
            else {
                layui.use('layer',function () {
                   var layer=layui.layer;
                    setTimeout(function () {
                        layer.msg('数据加载失败！');
                    },1000);
                });

            }
        }
    })

}
function setUserAuth() {
    $('.wchatUserLists').on('mouseenter',function () {
        $(this).css({'background':'#66ffcc'}).siblings().css({'background':'#f2dede'});
        var userId=$(this).attr('name');
        if(userId!='null'){
            var obj={'userId':userId,'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
            var url='/XCCloud/UserInfo?action=GetXcUserInfo';
            $.ajax({
                type:'post',
                dataType:'json',
                url:url,
                data:obj,
                success:function (data) {
                    if(data.result_code==1){
                        var arr=data.result_data;
                        $('.employeeName').val(arr.realName);
                        $('.username').val(arr.logName);
                        $('.IDCard').val(arr.iCCardId);
                        $('.phone').val(arr.mobile);
                        $('.employerStatus').val(showEmplonerStatus(arr.status));
                        empStatus=arr.status;
                    }else if(data.return_msg=="token无效"){
                        autoReLogin();
                    }else {
                        empStatus="";
                    }
                }
            });
            setStorage('userID',userId);
        }else {
            $('.employeeName').val("");
            $('.username').val("");
            $('.IDCard').val("");
            $('.phone').val("");
            $('.employerStatus').val("");
        }
    });
    $('.wchatUserLists>div .editBtn').on('click',function () {
        getWorkGroup();
        var userID=$(this).attr('name');
            var obj={'userId':userID,'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
            getUserAuthList(obj);
            var url='/XCCloud/UserInfo?action=GetXcUserInfo';
            $.ajax({
                type:'post',
                dataType:'json',
                url:url,
                data:obj,
                success:function (data) {
                    if(data.result_code==1){
                        var arr=data.result_data;
                        $('#employeeName').val(arr.realName);
                        $('#logName').val(arr.logName);
                        $('#idCard').val(arr.iCCardId);
                        $('#phone').val(arr.mobile);
                        empStatus=arr.status;
                        setSelect("员工状态","empStatus",arr.status);
                    }else if(data.return_msg=="token无效"){
                        autoReLogin();
                    }else {
                        empStatus="";
                    }
                }
            });
            setStorage('userID',userID);
        var nickName=$(this).find('.nickName').text();
        var img=$(this).find('.wchatUserImg').attr('src');
        var openid=$(this).parent().siblings('p[class="WXopenId layui-hide"]').text();
        var unionId=$(this).parent().siblings('p[class="WXunionId layui-hide"]').text();
        setStorage('openid',openid); setStorage('unionId',unionId);
        $('.setBoxImg').attr('src',img);
        $('.setBoxNick').val(nickName);

        showLayerBox("员工权限设置",'setAuthBox','1200px','560px');
    });
    $('.wchatUserLists>div .addBtn').on('click',function () {
        getWorkGroup();
        setSelect("员工状态","empStatus");
        var obj={'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
        getUserAuthList(obj);
        setStorage('userID',"");
        $('#employeeName').val("");
        $('#logName').val("");
        $('#idCard').val("");
        $('#phone').val("");

        var nickName=$(this).find('.nickName').text();
        var img=$(this).find('.wchatUserImg').attr('src');
        var openid=$(this).parent().siblings('p[class="WXopenId layui-hide"]').text();
        var unionId=$(this).parent().siblings('p[class="WXunionId layui-hide"]').text();
        setStorage('openid',openid); setStorage('unionId',unionId);
        $('.setBoxImg').attr('src',img);
        $('.setBoxNick').val(nickName);
        showLayerBox("员工权限设置",'setAuthBox','1200px','560px');
    });
}
function getWorkGroup() {
    var userId=getStorage('userID');
    var obj={'userId':userId,'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
    var url='/XCCloud/UserGroup?action=GetUserGroupList';
    $('.workGroupList').html("");
    $.ajax({
        type:'post',
        dataType:'json',
        url:url,
        data:obj,
        success:function (data) {
            radioVal="";
            if(data.result_code==1){
                var arr=data.result_data;
                for(var  i in arr){
                    if(arr[i].checked==1){
                        radioVal=arr[i].groupId;
                        $('.workGroupList').append('<input type="radio" checked="true" name="workGroup"  value="'+arr[i].groupId+'" title="'+arr[i].groupName+'"><br>')
                    }else {
                        $('.workGroupList').append('<input type="radio" name="workGroup"  value="'+arr[i].groupId+'" title="'+arr[i].groupName+'"><br>')
                    }

                }
                layui.use('form',function () {
                    var form=layui.form;
                    form.render();
                })
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }
        }
    })
}
function getUserAuthList(objects) {
    var obj=objects;
    var url='/XCCloud/UserInfo?action=GetXcUserGrant';
    $('.authSetting').html("");
    $.ajax({
        type:'post',
        dataType:'json',
        url:url,
        data:obj,
        success:function (data) {
            if(data.result_code==1){
                var arr=data.result_data;
                checkBoxArr=[];
                for(var i in arr){
                    if(arr[i].grantEn=='1'){
                        $('.authSetting').append(' <input type="checkbox" checked="true" value="'+arr[i].id+'" title="'+arr[i].dictKey+'"><br>');
                        checkBoxArr.push({'id':arr[i].id,'grantEn':1});
                    }else {
                        $('.authSetting').append(' <input type="checkbox"  value="'+arr[i].id+'" title="'+arr[i].dictKey+'"><br>');
                    }
                }
                layui.use('form',function () {
                    var form=layui.form;
                    form.render();
                })
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }
        }
    })
}
function showLayerBox(title,content,areaX,areaY) {//标题：‘www’,弹出层容器id:divid, area:'1200px'
    layui.use('layer', function() {
        var layer = layui.layer;
        layer.open({
            type: 1,
            title:title,
            content: $('#'+content),
            shadeClose:true,
            area: [areaX,areaY]
        })
    });
}
function saveSubmit() {
    var userId=getStorage('userID');
    var openId=getStorage('openid');
    var unionId=getStorage('unionId');
    var logName=$('#logName').val();
    var realName=$('#employeeName').val();
    var mobile=$('#phone').val();
    var iCCardId=$('#idCard').val();
    var obj={'userId':userId,'logName':logName,'openId':openId,'realName':realName,'mobile':mobile,
        'iCCardId':iCCardId,'status':empStatus, 'unionId':unionId,'userGroupId':radioVal,
        'userGrants':checkBoxArr,'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};

    var url='/XCCloud/UserInfo?action=SaveXcUserInfo';
    var parasJson = JSON.stringify(obj);
    $.ajax({
        type:'post',
        url:url,
        data:{ parasJson: parasJson },
        success:function (data) {
            data=JSON.parse(data);
            if(data.result_code==1){
                checkBoxArr=[];
                radioVal="";

                closePage();
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }else {
                layui.use('layer',function () {
                    var layer=layui.layer;
                    layer.msg('操作失败，请检查后重新保存！')
                });
            }
        }
    });
}
function resettingPwd() {
    var openId=getStorage('openid');
    var obj={'openId':openId,'userToken':token,'signkey':'1f626576304bf5d95b72ece2222e42c3'};
    var url='/XCCloud/UserInfo?action=ResetPassword';
    $.ajax({
        type:'post',
        dataType:'json',
        url:url,
        data:obj,
        success:function (data) {
            if(data.result_code==1){
                layui.use('layer',function () {
                    var layer=layui.layer;
                    layer.msg('重置密码成功！')
                });
            }else if(data.return_msg=="token无效"){
                autoReLogin();
            }else {
                layui.use('layer',function () {
                    var layer=layui.layer;
                    layer.msg('密码重置失败，请尝试重新操作或者联系管理员！')
                });
            }
        }
    })
}
function showEmplonerStatus(i) {
    if(i==0){
        return '在职';
        empStatus=0;
    }else if(i==1){
        return '离职';
        empStatus=1;
    }else if(i==2){
        return '锁定';
        empStatus=2;
    }else if(i==3){
        return '休假';
        empStatus=3;
    }
}

function renderDiv(arr) {
    for(var i=0;i<arr.length;i++){
        if(arr[i].issearch==1){
            if(arr[i].type=='string'||arr[i].type=='number'){
                $('#searchbox').append('<div class="layui-inline layui-hide" style="width: 410px" name="'+arr[i].title+'"><label class="layui-form-label">'+arr[i].title+'</label>' +
                    '<div class="layui-input-inline"><input type="text" name="'+arr[i].field+'" class="layui-input" placeholder="请填写查询条件"></div></div>');
            }else if(arr[i].type=='numbers'){
                $('#searchbox').append('<div class="layui-inline layui-hide" style="width: 410px" name="'+arr[i].title+'"><label class="layui-form-label">'+arr[i].title+'</label><div class="layui-input-inline" style="width: 100px;">' +
                    '      <input type="text" name="'+arr[i].field+'1" placeholder="start" autocomplete="off" class="layui-input" placeholder="请填写查询条件">' +
                    '    </div>' +
                    '    <div class="layui-form-mid">-</div>' +
                    '    <div class="layui-input-inline" style="width: 100px;">' +
                    '      <input type="text" name="'+arr[i].field+'2" placeholder="end" autocomplete="off" class="layui-input" placeholder="请填写查询条件">' +
                    '    </div></div>')
            }else if(arr[i].type=='datetime'){
                $('#searchbox').append('<div class="layui-inline layui-hide" style="width: 410px" name="'+arr[i].title+'"><label class="layui-form-label">'+arr[i].title+'</label>' +
                    '<div class="layui-input-inline"><input type="text" id="datetime" class="layui-input" name="'+arr[i].field+'" placeholder="点击选择日期"></div></div>')

            }else if(arr[i].type=='datetimes'){
                $('#searchbox').append('<div class="layui-inline layui-hide" style="width: 410px" name="'+arr[i].title+'"><label class="layui-form-label">'+arr[i].title+'</label>' +
                    '<div class="layui-input-inline"><input type="text" id="datetimes" class="layui-input" name="'+arr[i].field+'" placeholder="点击选择日期"></div></div>')
            }else if(arr[i].type=='date'){
                $('#searchbox').append('<div class="layui-inline layui-hide" style="width: 410px" name="'+arr[i].title+'"><label class="layui-form-label">'+arr[i].title+'</label>' +
                    '<div class="layui-input-inline"><input type="text" id="date" class="layui-input" name="'+arr[i].field+'"placeholder="点击选择日期"></div></div>')
            }else if(arr[i].type=='dates'){
                $('#searchbox').append('<div class="layui-inline layui-hide" style="width: 410px" name="'+arr[i].title+'"><label class="layui-form-label">'+arr[i].title+'</label>' +
                    '<div class="layui-input-inline"><input type="text" id="dates" class="layui-input" name="'+arr[i].field+'"placeholder="点击选择日期"></div></div>')
            }else if(arr[i].type=='literals'){
                $('#searchbox').append('<div class="layui-inline layui-hide" style="width: 410px" name="'+arr[i].title+'"><label class="layui-form-label">'+arr[i].title+'</label>' +
                    '<div class="layui-input-inline"><select name="'+arr[i].field+'" class="layui-input" lay-ignore style="height: 38px;width: 190px"> <option value="" disabled selected>-请选择-</option>  '+
                    (function (){
                        $('select[name="'+arr[i].field+'"]').html("");
                        var options='';
                        for( j in arr[i].list){
                            options+='<option value="'+arr[i].list[j]+'">'+arr[i].list[j]+'</option>';
                            // $('select[name="'+arr[i].field+'"]').append('<option>'+arr[i].list[j]+'</option>');

                    }
                    return options;
                })()+'</select></div></div>')
            }
        }
    }
}
function addCheckbox(id,arr) {
    layui.use(['form','jquery'],function () {
        var form=layui.form;var $=layui.jquery;
        $('#'+id).html("");
        for(var i=0;i<arr.length;i++){
            if(arr[i].issearch==1){
                $('#'+id).append(' <input type="checkbox" name="like" value="'+arr[i].title+'" title="'+arr[i].title+'" lay-filter="aaaaa">');
            }
        }
        form.render();
    })
}
function getValues(arrList,arrResult) {
    for(var i=0;i<arrList.length;i++){
        if(arrList[i].type=='string'||arrList[i].type=='number'){
            if($('input[name="'+arrList[i].field+'"]').val().length>0){
                arrResult.push({'id':arrList[i].id,'field':arrList[i].field,'values':$('input[name="'+arrList[i].field+'"]').val()})
            }
        }else  if(arrList[i].type=='numbers'){
            if($('input[name="'+arrList[i].field+'1"]').val().length>0 && $('input[name="'+arrList[i].field+'2"]').val().length>0){
                arrResult.push({'id':arrList[i].id,'field':arrList[i].field,'values':[$('input[name="'+arrList[i].field+'1"]').val(),$('input[name="'+arrList[i].field+'2"]').val()]})
            }
        }else if(arrList[i].type=='datetime'||arrList[i].type=='date'){
            if($('input[name="'+arrList[i].field+'"]').val().length>0){
                arrResult.push({'id':arrList[i].id,'field':arrList[i].field,'values':$('input[name="'+arrList[i].field+'"]').val()})
            }
        }else if(arrList[i].type=='datetimes'||arrList[i].type=='dates'){
            if($('input[name="'+arrList[i].field+'"]').val().length>0){
                arrResult.push({'id':arrList[i].id,'field':arrList[i].field,'values':$('input[name="'+arrList[i].field+'"]').val().split(' - ')})
            }
        }else if(arrList[i].type=='literals'){
            // console.log($('select[name="'+arrList[i].field+'"]').find('option:selected').val())
            if($('select[name="'+arrList[i].field+'"]').find('option:selected').val()!=""){
                arrResult.push({'id':arrList[i].id,'field':arrList[i].field,'values':$('select[name="'+arrList[i].field+'"]').find('option:selected').val()})
            }

        }
    }
    return arrResult;
}