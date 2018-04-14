<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ceshi.aspx.cs" Inherits="XXCloudService.ceshi" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="text1" runat="server" placeholder="加密内容"></asp:TextBox>
               <asp:TextBox ID="TextBox1" runat="server" placeholder="解密密钥"></asp:TextBox>
                 <asp:TextBox ID="TextBox2" runat="server" placeholder="密钥"></asp:TextBox>
            <asp:Button ID="btn1" runat="server" OnClick="btn1_Click" Text="加密" />
              <asp:Button ID="btn2" runat="server" OnClick="btn2_Click" Text="解密" />
            <asp:Label ID="lbl1" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
