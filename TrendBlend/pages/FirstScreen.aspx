<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="FirstScreen.aspx.cs" Inherits="TrendBlend.pages.WebForm4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="/styles/FirstScreen/styles.css" />
</asp:Content>
    

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<div class="boundary">
      <div class="name-text">
            <asp:Label runat="server" Text="TrendBlend" ForeColor="black" CssClass="text"></asp:Label>
        </div>
    
        <div class="buttons"> 
            <div class="loginbutton1">
                <asp:Button ID="Button1" runat="server" Text="Login" CssClass="button2" OnClick="Button1_Click" />
            </div>
           
            <div class="loginbutton2">
                <asp:Button ID="Button2" runat="server" Text="Register" CssClass="button1" OnClick="Button2_Click" />
          </div>

       </div>
</div>
    </asp:Content>