<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Textbox_StaticId.aspx.cs" Inherits="Fritz.WebFormsTest.Web.Scenarios.Postback.Textbox_StaticId" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">


      <asp:TextBox runat="server" ID="TestTextboxControl" ClientIDMode="Static" />

    </form>
</body>
</html>
