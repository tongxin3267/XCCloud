<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowTest.aspx.cs" Inherits="XXCloudService.Test.WorkFlowTest.WorkFlowTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>预警监测</title>
    <script type="text/javascript" src="/website/js/jquery-1.8.3-min.js"></script>
    <script type="text/javascript">
        var websocketUrl = "ws://192.168.1.73:12884/websocket";
        var sendToken = '<%=ViewState["warningservertoken"]%>';

        function common(url, parasJson) {
            $.ajax({
                type: "post",
                url: url,
                contentType: "application/json; charset=utf-8",
                data: parasJson,
                success: function (data) {                    
                    console.log(data);
                },
                error: function (error) {
                    alert(error); 
                }
            });
        }

        function fireWarning() {
            var parasObj = { };
            var url = "/Test/WorkFlowTest/WorkFlowTest.aspx/Start";
            var parasJson = JSON.stringify(parasObj);
            common(url, parasJson);
        }

        function getRegisterObj() {
            var SendObject = new Object();
            SendObject.token = sendToken;
            var obj = new Object();
            obj.msgType = 1;
            obj.sObj = SendObject;
            return obj;
        };

        var start = function () {
            var inc = document.getElementById('incomming');
            var form = document.getElementById('sendForm');
            var input = document.getElementById('sendText');            

            inc.innerHTML += "正在连接服务 ..<br/>";

            // create a new websocket and connect
            window.ws = new WebSocket(websocketUrl);

            // when data is comming from the server, this metod is called
            ws.onmessage = function (evt) {
                var msg = JSON.parse(evt.data).answerMsg;
                var time = JSON.parse(evt.data).answerTime;
                inc.innerHTML += (time == undefined ? '' : (time + '<br/>')) + msg + '<br/>';
            };

            // when the connection is established, this method is called
            ws.onopen = function () {
                inc.innerHTML += '.. 连接成功<br/>';
                var businessObj = getRegisterObj();
                var str = JSON.stringify(businessObj);
                ws.send(str);
            };

            // when the connection is closed, this method is called
            ws.onclose = function () {
                inc.innerHTML += '.. 连接关闭<br/>';
            }

            form.addEventListener('submit', function (e) {
                e.preventDefault();
                fireWarning();
            });
        }
        window.onload = start;
    </script>
</head>
<body>
    <form id="sendForm">
        <input id="Submit1" type="submit" value="预警" />
    </form>
    <pre id="incomming"></pre>
</body>
</html>
