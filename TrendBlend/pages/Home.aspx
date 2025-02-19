<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="TrendBlend.pages.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/styles/Home/styles.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main_container">
        <div class="welcome_message">
            Welcome,
            <asp:Label ID="userNameLabel" runat="server" />
        </div>
        <asp:FileUpload ID="FileUpload1" runat="server" />

        <div class="upload-button">
            <asp:Button ID="Upload" runat="server" Text="Upload" OnClick="Upload_Click" />
        </div>
        <div class="label">
            <asp:Label ID="Label1" runat="server" Text="Label" Visible="False"></asp:Label>
        </div>
    </div>
</asp:Content>
