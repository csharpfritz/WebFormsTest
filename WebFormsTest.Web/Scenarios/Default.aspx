<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.cs" Inherits="Fritz.WebFormsTest.Web.Scenarios.Default" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

  <h2>Test Scenarios</h2>

  <div class="row">

    <div class="col-sm-12">

      <h3>Control Tree</h3>

      <ul>
        <li><a href="Postback/Textbox_StaticId.aspx" target="_blank">Textbox with StaticID</a></li>
      </ul>

      <h3>Page Events</h3>

      <ul>
        <li><a href="RunToEvent/VerifyOrder.aspx" target="_blank">Verify Order of Events</a></li>
      </ul>

    </div>

  </div>

</asp:Content>