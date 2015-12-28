<%@ Page Language="C#" AutoEventWireup="true" CodeFile="secureNotAllow.aspx.cs" Inherits="secureNotAllow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%; float: left; vertical-align: top; color: #FF0000; font-size: 20pt; position: relative">
    
        Отсутствует разрешение на просмотр данного отчета!</div>
    
        <asp:Image ID="Image1" runat="server" Height="197px" 
            ImageUrl="~/Images/USERSMODULE.png" Width="192px" />
            <br />
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://www.atlan.com.ua/kbweb1.aspx">Дополнительная информация...</asp:HyperLink><br />
    </form>
</body>
</html>
