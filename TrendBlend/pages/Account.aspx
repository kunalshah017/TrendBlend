<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="TrendBlend.pages.WebForm2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/styles/Account/styles.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="main_container">
        <div class="account_info_container">
            <div class="user_profile_picture">
                <asp:Image ID="ProfileImage" runat="server" />
            </div>
            <div class="account_info">
                <p class="account_info_fname">
                    <asp:Label ID="FirstNameLabel" runat="server" />
                </p>
                <p class="account_info_uname">
                    <asp:Label ID="UserNameLabel" runat="server" />
                </p>
            </div>
        </div>
        <div class="apparels_stats_container">
            <div class="apparel_stat">
                <p class="apparel_number">
                    <asp:Label ID="TopsCountLabel" runat="server" Text="0" />
                </p>
                <p class="apparel_type">Tops</p>
            </div>
            <div class="apparel_stat">
                <p class="apparel_number">
                    <asp:Label ID="BottomsCountLabel" runat="server" Text="0" />
                </p>
                <p class="apparel_type">Bottoms</p>
            </div>
            <div class="apparel_stat">
                <p class="apparel_number">
                    <asp:Label ID="FootwearCountLabel" runat="server" Text="0" />
                </p>
                <p class="apparel_type">Footwear</p>
            </div>
        </div>
        <div class="fav_blends_title">
            Your Favourite Blends 🩷
        </div>
        <div class="favourite_blends_container">
            <asp:Repeater ID="FavouriteBlendRepeater" runat="server">
                <ItemTemplate>
                    <div class="blend_card" onclick="window.location.href='/pages/Blend.aspx?id=<%# Eval("BlendID") %>'">
                        <div class="blend_images_container">
                            <%# RenderBlendImages(Eval("BlendImages") as string) %>
                        </div>
                        <span class="blend_name"><%# Eval("Name") %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
