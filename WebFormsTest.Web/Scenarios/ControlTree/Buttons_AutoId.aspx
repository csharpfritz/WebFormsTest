<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Buttons_AutoId.aspx.cs" Inherits="Fritz.WebFormsTest.Web.Scenarios.ControlTree.Buttons_AutoId" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="form1" runat="server">

    <p>These are my two buttons.  One is always enabled, one disabled.  When you click on one it should disable itself and enable the other</p>

    <asp:FormView runat="server" ID="mainForm">

      <EmptyDataTemplate>

        <asp:Button runat="server" ID="buttonA" Text="Button A" OnClick="buttonA_Click" OnCommand="buttonA_Command" CommandName="Caption" />

        <asp:Button runat="server" ID="buttonB" Text="Button B" Enabled="false" OnClick="buttonB_Click" />

      </EmptyDataTemplate>

    </asp:FormView>
  </form>
</body>
</html>
