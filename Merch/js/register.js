var mobile="";
var token="";
var state=new Date().getTime();
function checkPhone() {
    var str=$('#phone').val();
    var reg=/^1[3|4|5|8][0-9]\d{8}$/;
    if(str==""){
        $('.phoneTips').html("tips:手机号码不能为空").css({color:"red","z-index":"2"});
        return false;
    }else if(reg.test(str)){
        $('.phoneTips').html("tips:手机号码合法").css({color:"green","z-index":"2"});
        return true;
    }else {
        $('.phoneTips').html("tips:输入11位电话号码").css({color:"red,\"z-index\":\"2\""});
        return false;
    }
}
function checkUsername() {
    var str=$('#username').val();
    var reg=/^[\u4e00-\u9fa5a-zA-Z0-9]{6,16}$/;
    if(str==""){
        $('.nameTips').html("tips:账户名不能为空").css({color:"red","z-index":"2"});
        return false;
    }else if(reg.test(str)){
        $('.nameTips').html("tips:账户名合法").css({color:"green","z-index":"2"});
        return true;
    }else {
        $('.nameTips').html("tips:6-16个字符，汉字、字母数字").css({color:"red","z-index":"2"});
        return false;
    }
}
function checkStoreId() {
    var str=$('#storeId').val();
    var  strUpper=str.toUpperCase();
    console.log(strUpper);
    $('#storeId').val(strUpper);
    var reg=/^(B[0-9]{10})|(S[0-9]{7})$/;
    if(strUpper==""){
        $('.storeIdTips').html("tips:店铺ID不能为空").css({color:"red"});
        return false;
    }else if(reg.test(strUpper)){
        $('.storeIdTips').html("tips:ID合法").css({color:"green","z-index":"2"});
        return true;
    }else {
        $('.storeIdTips').html("tips:ID不合法").css({color:"red","z-index":"2"});
        return false;
    }
}
function checkTrueName() {
    var str=$('#trueName').val();
    var reg=/^[\u4e00-\u9fa5]{2,4}$/;
    if(str==""){
        $('.trueNameTips').html("tips:真实姓名不为空").css({color:"red","z-index":"2"});
        return false;
    }else if(reg.test(str)){
        $('.trueNameTips').html("tips:真实姓名合法").css({color:"green","z-index":"2"});
        return true;
    }else {
        $('.trueNameTips').html("tips:2-4个汉字").css({color:"red","z-index":"2"});
        return false;
    }
}
function checkPassword() {
    var str=$('#password').val();
    var reg=/^[a-zA-Z0-9]{6,12}$/;
    if(str==""){
        $('.passwordTips').html("tips:密码不能为空").css({color:"red","z-index":"2"});
        return false;
    }else if(reg.test(str)){
        $('.passwordTips').html("tips:账密码合法").css({color:"green","z-index":"2"});
        return true;
    }else {
        $('.passwordTips').html("tips:6-12个字符，必须为字母数字").css({color:"red","z-index":"2"});
        return false;
    }
}
// function checkRePassword() {
//     var str=$('#password').val();
//     var str2=$('#rePassword').val();
//     if(checkPassword() && str==str2){
//         $('.rePasswordTips').html("tips:确认密码成功").css({color:"green"});
//         return true;
//     }else {
//         $('.rePasswordTips').html("tips:两次输入不同，重新输入").css({color:"red"});
//         return false;
//     }
// }

function getNewUl(id) {
    var imgUl="/ServicePage/ValidateImg.aspx?";
    var data=new Date().getTime();
    imgUl+=data;
    $(id).attr('src',imgUl);
    $('.returnMsg').css({display:"none"});
}

function toNext(obj) {
    if(checkPhone()){
        mobile=$('#phone').val();
        var thisCode=$('.inputCode').val();
        obj={"sysId": "0", "versionNo": "0.0.0.1", "mobile": mobile, "code":thisCode };
        $.ajax({
            type:"post",
            url:"/xcgamemana/token?action=checkImgCode",
            data:obj,
            dataType:"json",
            success:function (data) {
                if(data.result_code=="1"){
                    token=data.result_data.token;
                    mobile=data.result_data.mobile;
                    $(".codeCheck").css({display:"none"});
                    $(".zhuce").css({display:"block"});
                }else{
                    $(".returnMsg").css({display:"block"}).html(data.result_msg);
                }
            }
        })
    }

}

function getMessage() {
    $.ajax({
        type:"post",
        url:"/xcgamemana/token?action=sendSMSCode",
        data:{sysId: "0", "versionNo": "0.0.0.1", "mobile": mobile, "token":token },
        dataType:"json",
        success:function (data) {
            if(data.result_code=="1"){
                $('.registerCode').attr('disabled','true');
                $(".registerCode").val("剩余60秒");
                var timer=setInterval(fn,1000);
                var i=59;
                function fn(){
                    $(".registerCode").val("剩余"+i+"秒");
                    i--;
                    if(i==-2){
                        window.clearInterval(timer);
                        $(".registerCode").val("重新发送").removeAttr('disabled');
                    }
                }
                $('#sendMsg').fadeIn(2000).fadeOut(1000);

            }else {

            }
        }
    });
}

function registerIn() {
    var username=$('#username').val();
    var password=$('#password').val();
    var storeId=$('#storeId').val();
    var smsCode=$('.registerInputCode').val();

    if($('.checkbox1').attr('checked')){
        $.ajax({
            type:"post",
            dataType:"json",
            url:"/XCGameMana/UserRegister?action=getUserRegister",
            data:{ "sysId": "0", "versionNo": "0.0.0.1", "UserName": username, "PassWord": password, "storeId": storeId, "mobile": mobile,"smsCode":smsCode},
            success:function (data) {
                if(data.result_code=="1"){
                    $(".zhuce").css({display:"none"});
                    $(".toExample").css({display:"block"});
                }else {
                    alert(data.result_msg);
                }
            }
        })
    }
}

// GET方式请求
function contactWeiChat() {
  var username=$('#username').val();
  var storeId=$('#storeId').val();
  var password=$('#password').val();
  var trueName=$('#trueName').val();
  var userNote=$('#userNote').val();
  var scode=$('.registerInputCode').val();


  var href="/connect/oauth2/authorize?appid=wx86275e2035a8089d&redirect_uri=";
    var href2= "/WeiXin/Register.aspx?" +
      "smId="+storeId+
      "&scode=" +scode+
      "&mobile=" +mobile+
      "&username=" +username+
      "&password=" +password+
      "&realname=" +trueName+
      "&message=" +userNote;

    var  newHref=href+encodeURIComponent(href2)+"&response_type=code&scope=snsapi_base&state="+state+"#wechat_redirect";
    console.log(newHref);
    window.location.href=newHref;
    // $(".zhuce").css({display:"none"});
    // $(".toExample").css({display:"block"});

}



//...............................................微信用户.............................................................
//请求权限

//打开工作组
var paras = [];
var values = [];
function openGroup() {
        $('.addArrow1').css("background-image","url(images/arrow-down.svg)");
    $('.addArrow2').css("background-image","url(images/arrow-right.svg)");
        $('.grantBox_group').slideDown();
        $('.grantBox_grant').slideUp();
}
function endGroup() {
        $('.grantBox_group').slideUp();
    $('.addArrow1').css("background-image","url(images/arrow-right.svg)");
}
function openGrant() {
    $('.addArrow2').css("background-image","url(images/arrow-down.svg)");
    $('.addArrow1').css("background-image","url(images/arrow-right.svg)");
    $('.grantBox_grant').slideDown();
    $('.grantBox_group').slideUp();
}
function endGrant() {
    $('.grantBox_grant').slideUp();
    $('.addArrow2').css("background-image","url(images/arrow-right.svg)");
}
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
function do_userGroup() {
    paras = [];
    values = [];
    getPara();
    var workIds="";
    for(var i=0;i<paras.length;i++){
        if(paras[i]=="workId"){
            workIds=values[i];
        }
    }
    var parasObj = { "sysId": "0", "versionNo": "0.0.0.1", "workId": workIds };
    var url = '/wx/Users?action=GetUserGroup';
    var parasJson = JSON.stringify(parasObj);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: { parasJson: parasJson },
        success: function (data) {
            data = JSON.parse(data);
            console.log(data);
            var arr=data.result_data;
            if(arr.length!=0){
                for(var i=0;i<arr.length;i++){
                    if(arr[i].GroupName!=""){
                        $('.authSelectList_group') .append("<li class='authSelectList_li'><span class='selectTexts'>"
                            +arr[i].GroupName+"</span><input type='checkbox'>" + " <div class='isCheck'><span></span> </div></li>"
                        );
                    }
                }
            }

        },
        error: function (error) {
            console.log(1);  //这个地方也用到了
        }
    });
}
//请求授权ze
function do_userGrant() {
    paras = [];
    values = [];
    getPara();
    var workIds="";
    for(var i=0;i<paras.length;i++){
        if(paras[i]=="workId"){
            workIds=values[i];
        }
    }
    var parasObj = { "sysId": "0", "versionNo": "0.0.0.1", "workId": workIds };
    var url = '/wx/Users?action=GetUserGrant';
    var parasJson = JSON.stringify(parasObj);
    $.ajax({
        type: "post",
        url: url,
        contentType: "application/json; charset=utf-8",
        data: { parasJson: parasJson },
        success: function (data) {
            data = JSON.parse(data);
            console.log(data);
            var arr=data.result_data;
            if(arr.length!=0){
                for(var i=0;i<arr.length;i++){
                    if(arr[i].DictValue!=""){
                        $('.authSelectList_grant') .append("<li class='authSelectList_li'><span class='selectTexts'>"
                            +arr[i].DictValue+"</span><input type='checkbox'>" + " <div class='ifCheck'><span></span></div></li>"
                        )
                    }
                }
            }
        },
        error: function (error) {
            console.log(1);  //这个地方也用到了
        }
    });
}

function addMessage() {
    paras = [];
    values = [];
    getPara();
    for(var i=0;i<paras.length;i++){
        if(paras[i]=="userName"){
            $('#username').val(values[i])
        }
    }
    for(var i=0;i<paras.length;i++){
        if(paras[i]=="message"){
            $('#reson').val(values[i])
        }
    }
}

function openAddReason() {
    $('.refuseReason').stop(false, true).animate({left: '+=-100%'}, "quick");
}
function addReason() {
    $('.refuseReason').stop(false, true).animate({left: '+=100%'}, "quick");
}

//订单回执提交

function refused(obj) {
    var username=$('#username').val();
    var message=$('#reson').val();
    var refuseReason=$('#reasons').val();
    paras = [];
    values = [];
    getPara();
    var workIds="";
    for(var i=0;i<paras.length;i++){
        if(paras[i]=="workId"){
            workIds=values[i];
        }
    }
    obj={"groupName":"","userGrant":{},"id":"","pId":"","dictValue":"","grantEn":"",
        "state":"2","reason":refuseReason,"workId":workIds,"sysId":"0","versonNo":"0.0.0.1"};
}
