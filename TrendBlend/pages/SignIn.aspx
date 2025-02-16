<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="TrendBlend.pages.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/styles/Login/styles.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container">

        <div class="login-text">
            <asp:Label ID="login" runat="server" Text="Login" CssClass="logintext" ForeColor="white"></asp:Label>
        </div>

        <div class="boundaries">
            <div class="username-text">
                <asp:Label ID="username" runat="server" Text="Username" CssClass="usernametext" ForeColor="white"></asp:Label>
            </div>

            <div class="textbox">
                <asp:TextBox ID="usernameTextBox" runat="server" CssClass="usernametextbox"></asp:TextBox>
            </div>

            <div class="password-text">
                <asp:Label ID="password" runat="server" Text="Password" CssClass="passwordtext" ForeColor="white"></asp:Label>
            </div>

            <div class="textbox">
                <asp:TextBox ID="passwordTextBox" runat="server" CssClass="passwordtextbox" TextMode="Password"></asp:TextBox>
                <br />
                <asp:CheckBox ID="CheckBox1" runat="server" Text="show password" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged" />
            </div>

            <div class="button">
                <asp:Button ID="Button1" runat="server" Text="Login" CssClass="loginbutton" OnClick="Button1_Click" />
                <br />

            </div>
            <div class="error">
                <asp:Label ID="Label1" runat="server" Text="Label" ForeColor="Red" Visible="False"></asp:Label>
            </div>
            <div>
                <asp:HyperLink ID="HyperLink1" runat="server">New Here? Register </asp:HyperLink>
            </div>
        </div>

    </div>

</asp:Content>
