<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TrendBlend.pages.WebForm3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="/styles/Register.css" />
    <link href="https://fonts.googleapis.com/css2?family=Satisfy:wght@300;400;600&display=swap" rel="stylesheet">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content">
        <div class="registertext">
            <asp:Label ID="registertext" runat="server" Text="Register" CssClass="registertext"></asp:Label>
            <br />
        </div>

        <div class="inputuser">
            <asp:Label ID="firstname" runat="server" Text="Firstname " CssClass="text"></asp:Label>
            <asp:Label ID="lastname" runat="server" Text="Lastname " CssClass="text"></asp:Label>
            <br />
            <asp:TextBox ID="fNameInput" runat="server" CssClass="finput"></asp:TextBox>
            <asp:TextBox ID="lNameInput" runat="server" CssClass="linput"></asp:TextBox>
        </div>
        <div class="agefavcolorinput">
            <asp:Label ID="agetext" runat="server" Text="Age " CssClass="text"></asp:Label>
            <asp:Label ID="favcolor" runat="server" Text="Fav Color" CssClass="text"></asp:Label>
            <br />
            <asp:TextBox ID="ageinput" runat="server" CssClass="ageinput"></asp:TextBox>
            <asp:TextBox ID="colorinput" runat="server" CssClass="colorinput"></asp:TextBox>
            <br />
        </div>
        <div class="emailinput">
            <asp:Label ID="emailtext" runat="server" Text="Email " CssClass="text"></asp:Label>
            <br />
            <asp:TextBox ID="emailinput" runat="server" CssClass="input"></asp:TextBox>
            <br />
        </div>
        <div class="usernameinput">
            <asp:Label ID="usernametext" runat="server" Text="Username" CssClass="text"></asp:Label>
            <br />
            <asp:TextBox ID="usernameinput" runat="server" CssClass="input"></asp:TextBox>
            <br />
        </div>
        <div class="passinput">
            <asp:Label ID="passwordtext" runat="server" Text="Password " CssClass="text"></asp:Label>
            <br />
            <asp:TextBox ID="passwordinput" runat="server" CssClass="input" TextMode="Password"></asp:TextBox>
            <br />
        </div>
        <div class="button">
            <asp:Button ID="Button1" runat="server" Text="Register" OnClick="Button1_Click" Height="40px" Width="90px" CssClass="buttonregister" />
        </div>
        <asp:Label ID="Label8" runat="server" Text="Label"></asp:Label>

    </div>
</asp:Content>
