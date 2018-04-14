<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SocketTest.aspx.cs" Inherits="XXCloudService.Test.SocketTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
<script src="/WebSite/js/jquery-1.8.3-min.js"></script>

<script type="text/javascript">

    function common(url, data) {
        $.ajax({
            type: "get",
            url: url,
            contentType: "application/json; charset=utf-8",
            data: data,
            success: function (data) {
                data = JSON.parse(data);
                console.log(data);
            },
            error: function (error) {
                alert(1);  //这个地方也用到了
            }
        });
    }

</script>

<script type="text/javascript">
    function routeRegister() {
        var parasObj = { "storeId": "100027", "segment": "0001","signkey":"" };
        var url = "/Test/SocketTest.ashx?method=routeRegister";
        var parasJson = JSON.stringify(parasObj);
        common(url, parasJson);
    }

    function routeRegisterUDP() {
        var data = { "storeId": "100027", "segment": "0001", "signkey": "aac2cd1eabc5ffcbd205ca45d6c419b0" };
        var url = "/Test/SocketTest.ashx?method=routeRegisterUDP";
        common(url, data);
    }

    function deviceState() {
        var data = { "token": "62fb63fcc33c246999246cb440e24b1b0", "mcuid": "20171014300001", "status": "1", "signkey": "6e68feb310f07d578a18023c4c2c39f0" };
        var url = "/Test/SocketTest.ashx?method=deviceState";
        common(url, data);
    }

    function 雷达心跳() {
        var data = { "token": "62fb63fcc33c246999246cb440e24b1b0", "mcuid": "20171014300001", "status": "1", "signkey": "6e68feb310f07d578a18023c4c2c39f0" };
        var url = "/Test/SocketTest.ashx?method=radarHeartbeat";
        common(url, data);
    }

    function 远程设备控制指令() {
        var data = { "token": "62fb63fcc33c246999246cb440e24b1b0", "mcuid": "20171014300001", "action": "1", "count": "200", "sn": "123", "signkey": "5431635431213654" };
        var url = "/Test/SocketTest.ashx?method=deviceControl";
        common(url, data);
    }

    function 获取雷达客户端参数() {
        var data = { "token": "62fb63fcc33c246999246cb440e24b1b0", "mcuid": "20171014300001", "action": "1", "count": "200", "sn": "123", "signkey": "5431635431213654" };
        var url = "/Test/SocketTest.ashx?method=getRadarToken";
        common(url, data);
    }

    function webClientRegister() {
        var parasObj = { "storeId": "100000", "mobile": "15618920033" };
        var url = "/Test/SocketTest.ashx?method=webClientRegister";
        var parasJson = JSON.stringify(parasObj);
        common(url, parasJson);
    }

    function serverReceiveRadarResponse() {
        var data = { "result_code": "1", "result_msg": "","sn":"23432","signkey":"fewfewfewfew" };
        var url = "/Test/SocketTest.ashx?method=serverReceiveRadarResponse";
        common(url, data);
    }

    function RadarNotify() {
        var data = { "token": "62fb63fcc33c246999246cb440e24b1b0", "action": "1", "result": "成功", "orderid": "20171010500001", "sn": "123", "signkey": "5431635431213654" };
        var url = "/Test/SocketTest.ashx?method=radarNotify";
        common(url, data);
    }

    function getMemberInfo() {
        var result_data = {
            "storeId": 100000,
            "storeName": "武汉莘宸电玩1",
            "icCardID": 90000011,
            "memberName": "李俊杰",
            "gender": "1",
            "birthday": "2017-09-08",
            "certificalID": "420300197610102511",
            "mobile": "15618920029",
            "balance": 0,
            "point": 0,
            "deposit": 0,
            "memberState": "1",
            "lottery": 0,
            "note": "",
            "memberLevelName":"普通会员",
            "endDate":"2027-09-09"
        };

        var data = {
            "result_code":"1",
            "result_msg":"",
            "signkey": "",
            "result_data": ""
        };

        var url = "/Test/SocketTest.ashx?method=getMemberInfo";
        common(url, data);
    }

</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input id="Button101" type="button" value="route" onclick="routeRegister()" />
        <input id="Button102" type="button" value="webClient" onclick="webClientRegister()" />


        <input id="Button11" type="button" value="routeRegisterUDP" onclick="routeRegisterUDP()" />
        <input id="Button12" type="button" value="deviceState" onclick="deviceState()" />
        <input id="Button13" type="button" value="雷达心跳" onclick="雷达心跳()" />
        <input id="Button14" type="button" value="远程设备控制指令" onclick="远程设备控制指令()" />
        <input id="Button15" type="button" value="获取雷达客户端参数" onclick="获取雷达客户端参数()" />

        <input id="Button16" type="button" value="雷达向服务端发送（远程设备控制指令）应答" onclick="serverReceiveRadarResponse()" />
    
        <input id="Button17" type="button" value="雷达向服务端发送出币回复" onclick="RadarNotify()" />

        <br />

        <input id="Button20" type="button" value="雷达向服务端发送会员结果回复" onclick="getMemberInfo()" />
    
    </div>
    </form>
</body>
</html>
