<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageWithUserControl.aspx.cs" Inherits="Fritz.WebFormsTest.Web.Scenarios.ControlTree.PageWithUserControl" %>

<%@ Register Src="~/UserControls/MyLinkControl.ascx" TagPrefix="uc1" TagName="MyLinkControl" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="form1" runat="server">
    <div>

      <p>Here is a link button in a user control:</p>

      <uc1:MyLinkControl runat="server" id="MyLinkControl" LinkButtonText="New Text" />

    </div>
  </form>
</body>
</html>
