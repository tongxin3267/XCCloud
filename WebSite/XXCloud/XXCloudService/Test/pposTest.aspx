<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pposTest.aspx.cs" Inherits="XXCloudService.Test.pposTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="微信公众号查询" OnClick="Button1_Click" />
    
    </div>
        <p>
        <asp:Button ID="Button2" runat="server" Text="微信公众号支付" OnClick="Button2_Click" />
    
        </p>
    </form>
</body>
</html>
