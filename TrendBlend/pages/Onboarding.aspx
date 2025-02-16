<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="Onboarding.aspx.cs" Inherits="TrendBlend.pages.Onboarding" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../styles/Onboarding/styles.css" />
    <title>TrendBlend | Welcome</title>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main_container">
        <div class="app_title">
            Trend Blend
        </div>
        <div class="onboarding_icons">
            <img src="../assets/dress_icons/coat-1.svg" style="left: 5%; top: 20%;" />
            <img src="../assets/dress_icons/jeans-1.svg" style="left: 80%; top: 12%;" />
            <img src="../assets/dress_icons/hoodie-1.svg" style="left: 10%; top: 50%;" />
            <img src="../assets/dress_icons/woman-shirt-1.svg" style="left: 80%; top: 55%;" />
            <img src="../assets/dress_icons/woman-dress-1.svg" style="left: 45%; top: 2%;" />
        </div>
        <div class="cta_button_container">
            <div>
                <asp:Button runat="server" Text="Sign In" CssClass="cta_button" ID="SignInButton" OnClick="SignInButton_Click" />
            </div>
            <div>
                <asp:Button runat="server" Text="Register" CssClass="cta_button" ID="RegisterButton" OnClick="RegisterButton_Click" />
            </div>
        </div>
    </div>
</asp:Content>
