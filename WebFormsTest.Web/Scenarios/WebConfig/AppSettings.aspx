<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppSettings.aspx.cs" Inherits="Fritz.WebFormsTest.Web.Scenarios.WebConfig.AppSettings" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
      This is my test setting: 
      <asp:Label runat="server" ID="testSetting" Font-Bold="true"></asp:Label>

      <br />
      This is my initial catalog of the default connection string:
      <asp:Label runat="server" ID="testInitialCatalog" Font-Bold="true"></asp:Label>

    </div>
    </form>
</body>
</html>
