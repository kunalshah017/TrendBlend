<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TrendBlend.pages.WebForm3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="Label1" runat="server" Text="enter your Firstname "></asp:Label>
    <asp:TextBox ID="fNameInput" runat="server"></asp:TextBox>
    <br />
     <asp:Label ID="Label2" runat="server" Text="enter your lastname "></asp:Label>
    <asp:TextBox ID="lNameInput" runat="server"></asp:TextBox>
    <br />
     <asp:Label ID="Label3" runat="server" Text="enter your age "></asp:Label>
    <asp:TextBox ID="ageinput" runat="server"></asp:TextBox>
    <br />
     <asp:Label ID="Label4" runat="server" Text="enter your fav color "></asp:Label>
    <asp:TextBox ID="colorinput" runat="server"></asp:TextBox>
    <br />
     <asp:Label ID="Label5" runat="server" Text="enter your email "></asp:Label>
    <asp:TextBox ID="emailinput" runat="server"></asp:TextBox>
    <br />
     <asp:Label ID="Label6" runat="server" Text="enter your username"></asp:Label>
    <asp:TextBox ID="usernameinput" runat="server"></asp:TextBox>
 <br />
     <asp:Label ID="Label7" runat="server" Text="enter your password "></asp:Label>
    <asp:TextBox ID="passwordinput" runat="server"></asp:TextBox>
    <br />
    <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />

    <asp:Label ID="Label8" runat="server" Text="Label"></asp:Label>
</asp:Content>
