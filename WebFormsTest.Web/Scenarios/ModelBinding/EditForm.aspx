<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditForm.aspx.cs" Inherits="Fritz.WebFormsTest.Web.Scenarios.ModelBinding.EditForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        <asp:FormView runat="server" ID="myForm" ItemType="Fritz.WebFormsTest.Web.Scenarios.ModelBinding.Simple+GridItem" 
            DataKeyNames="ID" DefaultMode="Edit"
            SelectMethod="myForm_GetItem" InsertMethod="myForm_InsertItem" UpdateMethod="myForm_UpdateItem">
            <HeaderTemplate>
                <asp:ValidationSummary runat="server" ID="validationSummary" />
            </HeaderTemplate>
            <ItemTemplate>
                <dl>
                    <dt>ID:</dt><dd><%#: Item.ID %></dd>
                    <dt>Name:</dt><dd><%#: Item.Name %></dd>
                </dl>

                <asp:Button runat="server" ID="edit" CommandName="Edit" Text="Edit" />

            </ItemTemplate>
            <EditItemTemplate>
                <dl>
                    <dt>Id:</dt>
                    <dd><%#: Item.ID %></dd>
                    <dt>Name:</dt>
                    <dd><asp:TextBox runat="server" ID="name" TextMode="SingleLine" Text="<%#: BindItem.Name %>"></asp:TextBox></dd>
                </dl>
                <asp:Button runat="server" ID="Save" CommandName="Update" Text="Update" />
            </EditItemTemplate>
        </asp:FormView>
    
    </div>
    </form>
</body>
</html>
