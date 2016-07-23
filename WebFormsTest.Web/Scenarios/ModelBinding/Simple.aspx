<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Simple.aspx.cs" Inherits="Fritz.WebFormsTest.Web.Scenarios.ModelBinding.Simple" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:GridView runat="server" id="myGrid" SelectMethod="Get">
        </asp:GridView>

    </div>
    </form>
</body>
</html>
