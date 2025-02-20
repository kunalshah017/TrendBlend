<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="TrendBlend.pages.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/styles/Home/styles.css" rel="stylesheet" />
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

        <div class="container">

            <div class="tops_title">
                <asp:Label ID="tops_label" runat="server" Text="Tops"></asp:Label>
                <asp:HyperLink ID="topviewmore" runat="server" NavigateUrl="/pages/Register.aspx">View More></asp:HyperLink>
            </div>
            <div class="topboxes">
                <div class="box">
                </div>
                <div class="box">
                </div>
                <div class="box">
                </div>
            </div>

            <div class="bottom_title">
                <asp:Label ID="bottom_label" runat="server" Text="Bottom"></asp:Label>
                <asp:HyperLink ID="bottomviewmore" runat="server" NavigateUrl="/pages/Register.aspx">View More></asp:HyperLink>
            </div>
            <div class="bottomboxes">
                <div class="box">
                </div>
                <div class="box">
                </div>
                <div class="box">
                </div>
            </div>

            <div class="accessories_title">
                <asp:Label ID="accessories_label" runat="server" Text="Accessories"></asp:Label>
                <asp:HyperLink ID="accessoriesviewmore" runat="server" NavigateUrl="/pages/Register.aspx">View More></asp:HyperLink>
            </div>
            <div class="accessoriesboxes">
                <div class="box">
                </div>
                <div class="box">
                </div>
                <div class="box">
                </div>
            </div>

        </div>
    </div>
</asp:Content>
