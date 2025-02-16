<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="Onboarding.aspx.cs" Inherits="TrendBlend.pages.WebForm4" %>

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
            <img src="../assets/dress_icons/coat-1.svg" style="animation: slideIn 1s ease-out 0s forwards; left: 10%; top: 30%;" />
            <img src="../assets/dress_icons/coat-2.svg" style="animation: slideIn 1s ease-out 0.3s forwards; left: 40%; top: 10%;" />
            <img src="../assets/dress_icons/hoodie-1.svg" style="animation: slideIn 1s ease-out 0.6s forwards; left: 20%; top: 60%;" />
            <img src="../assets/dress_icons/jeans-1.svg" style="animation: slideIn 1s ease-out 0.9s forwards; left: 70%; top: 40%;" />
            <img src="../assets/dress_icons/woman-dress-1.svg" style="animation: slideIn 1s ease-out 1.2s forwards; left: 50%; top: 80%;" />
            <img src="../assets/dress_icons/woman-dress-2.svg" style="animation: slideIn 1s ease-out 1.5s forwards; left: 80%; top: 20%;" />
            <img src="../assets/dress_icons/woman-shirt-1.svg" style="animation: slideIn 1s ease-out 1.8s forwards; left: 30%; top: 50%;" />
        </div>

    </div>
</asp:Content>
