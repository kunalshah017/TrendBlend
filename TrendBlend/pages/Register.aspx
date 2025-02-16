﻿<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TrendBlend.pages.WebForm3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="/styles/Register/styles.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="content">

        <div class="registertext">
            <asp:Label ID="registertext" runat="server" Text="Register" CssClass="registertext"></asp:Label>
        </div>

        <div class="multi-input-container">
            <div class="single-input-container">
                <asp:Label ID="firstname" runat="server" Text="Firstname " CssClass="text"></asp:Label>
                <asp:TextBox ID="fNameInput" runat="server" CssClass="input-feild"></asp:TextBox><asp:RequiredFieldValidator runat="server" ErrorMessage="Enter your name" Display="Dynamic" ControlToValidate="fNameInput" ForeColor="Red"></asp:RequiredFieldValidator>
            </div>
            <div class="single-input-container">
                <asp:Label ID="lastname" runat="server" Text="Lastname " CssClass="text"></asp:Label>
                <asp:TextBox ID="lNameInput" runat="server" CssClass="input-feild"></asp:TextBox><asp:RequiredFieldValidator runat="server" ErrorMessage="Enter your last name" Display="Dynamic" ControlToValidate="lNameInput" ForeColor="Red"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="multi-input-container">
            <div class="single-input-container">
                <asp:Label ID="agetext" runat="server" Text="Age " CssClass="text"></asp:Label>
                <asp:TextBox ID="ageinput" runat="server" CssClass="input-feild"></asp:TextBox><asp:RequiredFieldValidator runat="server" ErrorMessage="Enter your age" Display="Dynamic" ControlToValidate="ageinput" ForeColor="Red"></asp:RequiredFieldValidator>
            </div>
            <div class="single-input-container">
                <asp:Label ID="favcolor" runat="server" Text="Fav Color" CssClass="text"></asp:Label>
                <asp:TextBox ID="colorinput" runat="server" CssClass="input-feild"></asp:TextBox><asp:RequiredFieldValidator runat="server" ErrorMessage="Enter your favourite color" Display="Dynamic" ControlToValidate="colorinput" ForeColor="Red"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="single-input-container">
            <asp:Label ID="emailtext" runat="server" Text="Email " CssClass="text"></asp:Label>
            <asp:TextBox ID="emailinput" runat="server" CssClass="input-feild"></asp:TextBox><asp:RegularExpressionValidator runat="server" ErrorMessage="Enter a valid Email ID" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="emailinput" ForeColor="Red"></asp:RegularExpressionValidator><asp:RequiredFieldValidator runat="server" ErrorMessage="required" Display="Dynamic" ControlToValidate="emailinput" ForeColor="Red"></asp:RequiredFieldValidator>
        </div>

        <div class="single-input-container">
            <asp:Label ID="usernametext" runat="server" Text="Username" CssClass="text"></asp:Label>
            <asp:TextBox ID="usernameinput" runat="server" CssClass="input-feild"></asp:TextBox><asp:RequiredFieldValidator runat="server" ErrorMessage="Enter a valid username" Display="Dynamic" ControlToValidate="usernameinput" ForeColor="Red"></asp:RequiredFieldValidator>
        </div>

        <div class="single-input-container">
            <asp:Label ID="passwordtext" runat="server" Text="Password " CssClass="text"></asp:Label>
            <asp:TextBox ID="passwordinput" runat="server" CssClass="input-feild" TextMode="Password"></asp:TextBox><asp:RequiredFieldValidator runat="server" ErrorMessage="Password required " Display="Dynamic" ControlToValidate="passwordinput" ForeColor="Red"></asp:RequiredFieldValidator>
        </div>

        <asp:Button ID="Button1" runat="server" Text="Register" OnClick="Button1_Click" Height="40px" Width="90px" CssClass="buttonregister" />
        <div>
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/pages/Login.aspx">Already have an account? Login!</asp:HyperLink>
        </div>
        
    </div>
</asp:Content>
