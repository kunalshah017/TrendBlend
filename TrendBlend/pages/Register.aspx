<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TrendBlend.pages.WebForm3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="/styles/Register/styles.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="content">
        <img class="content-background" src="~/assets/registration.jpg" />

        <div class="registertext">
            <asp:Label ID="registertext" runat="server" Text="Register" CssClass="registertext"></asp:Label>
        </div>

        <div class="multi-input-container">
            <div class="single-input-container">
                <asp:Label ID="firstname" runat="server" Text="Firstname " CssClass="text"></asp:Label>
                <asp:TextBox ID="fNameInput" runat="server" CssClass="input-feild"></asp:TextBox>
            </div>
            <div class="single-input-container">
                <asp:Label ID="lastname" runat="server" Text="Lastname " CssClass="text"></asp:Label>
                <asp:TextBox ID="lNameInput" runat="server" CssClass="input-feild"></asp:TextBox>
            </div>
        </div>

        <div class="multi-input-container">
            <div class="single-input-container">
                <asp:Label ID="agetext" runat="server" Text="Age " CssClass="text"></asp:Label>
                <asp:TextBox ID="ageinput" runat="server" CssClass="input-feild"></asp:TextBox>
            </div>
            <div class="single-input-container">
                <asp:Label ID="favcolor" runat="server" Text="Fav Color" CssClass="text"></asp:Label>
                <asp:TextBox ID="colorinput" runat="server" CssClass="input-feild"></asp:TextBox>
            </div>
        </div>

        <div class="single-input-container">
            <asp:Label ID="emailtext" runat="server" Text="Email " CssClass="text"></asp:Label>
            <asp:TextBox ID="emailinput" runat="server" CssClass="input-feild"></asp:TextBox>
        </div>

        <div class="single-input-container">
            <asp:Label ID="usernametext" runat="server" Text="Username" CssClass="text"></asp:Label>
            <asp:TextBox ID="usernameinput" runat="server" CssClass="input-feild"></asp:TextBox>
        </div>

        <div class="single-input-container">
            <asp:Label ID="passwordtext" runat="server" Text="Password " CssClass="text"></asp:Label>
            <asp:TextBox ID="passwordinput" runat="server" CssClass="input-feild" TextMode="Password"></asp:TextBox>
        </div>

        <asp:Button ID="Button1" runat="server" Text="Register" OnClick="Button1_Click" Height="40px" Width="90px" CssClass="buttonregister" />

    </div>
</asp:Content>
