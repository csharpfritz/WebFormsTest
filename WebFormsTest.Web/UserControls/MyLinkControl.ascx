<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyLinkControl.ascx.cs" Inherits="Fritz.WebFormsTest.Web.UserControls.MyLinkControl" %>

<p>Outputting an LinkButton with custom text</p>

<asp:LinkButton runat="server" ID="myLink" CommandName="DoStuff" Text="OriginalText"></asp:LinkButton>