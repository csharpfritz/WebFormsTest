<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebFormsTest._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>WebFormsTest.Web</h1>
        <p class="lead">WebFormsTest is a test harness that provides the ability to test your ASP.NET Web Forms application objcets</p>
        <p><a href="http://github.com/csharpfritz/WebFormsTest" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                Instructions on how to get started working with WebFormsTest are available at our project repository on GitHub.
            </p>
            <p>
                <a class="btn btn-default" href="#">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Scenarios</h2>
            <p>
                This is a test web application that hosts sample pages and scenarios that help verify the functionality of the WebFormsTest harness.
            </p>
            <p>
                <a class="btn btn-default" href="Scenarios">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Other Tools</h2>
            <p>
                WebFormsTest is designed to work with as many unit test tools and framework as possible.  Here are a few of the tools that the authors recommend
            </p>
            <p>
                <a class="btn btn-default" href="#">Learn more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
