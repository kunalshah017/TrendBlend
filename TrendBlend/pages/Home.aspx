<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="TrendBlend.pages.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>TrendBlend</title>
    <link href="/styles/Home/styles.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="main_container">
        <div class="welcome_message_container">
            <div class="welcome_message">
                <p>Welcome,</p>
                <asp:Label ID="userNameLabel" runat="server" CssClass="welcome_message_username" />
            </div>
            <div class="welcome_message_icons">
                <img src="../assets/dress_icons/jeans-1.svg" style="left: 10%; top: 12%; transform: rotate(-20deg);" />
                <img src="../assets/dress_icons/woman-shirt-1.svg" style="left: 40%; top: 5%; transform: rotate(30deg);" />
            </div>
        </div>
        <div class="apparels_container">
            <asp:Panel ID="TopsPanel" runat="server" CssClass="slider_container">
                <div class="slider_title_container">
                    <asp:Label ID="sliderTitle" runat="server" Text="Tops" CssClass="slider_title"></asp:Label>
                    <asp:HyperLink ID="topviewmore" runat="server" CssClass="view_more_link" NavigateUrl="~/pages/Search.aspx?type=Top">
                        View More
                    </asp:HyperLink>
                </div>
                <div class="slider">
                    <asp:Repeater ID="topsSlider" runat="server">
                        <ItemTemplate>
                            <div class="apparel_item" onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("Id") %>'">
                                <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                <p><%# Eval("Name") %></p>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="BottomsPanel" runat="server" CssClass="slider_container">
                <div class="slider_title_container">
                    <asp:Label ID="Label1" runat="server" Text="Bottoms" CssClass="slider_title"></asp:Label>
                    <asp:HyperLink ID="HyperLink1" runat="server" CssClass="view_more_link" NavigateUrl="~/pages/Search.aspx?type=Bottom">
                        View More
                    </asp:HyperLink>
                </div>
                <div class="slider">
                    <asp:Repeater ID="bottomsSlider" runat="server">
                        <ItemTemplate>
                            <div class="apparel_item" onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("Id") %>'">
                                <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                <p><%# Eval("Name") %></p>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="FootwearsPanel" runat="server" CssClass="slider_container">
                <div class="slider_title_container">
                    <asp:Label ID="Label2" runat="server" Text="Footwears" CssClass="slider_title"></asp:Label>
                    <asp:HyperLink ID="HyperLink2" runat="server" CssClass="view_more_link" NavigateUrl="~/pages/Search.aspx?type=Footwear">
                        View More
                    </asp:HyperLink>
                </div>
                <div class="slider">
                    <asp:Repeater ID="footwearsSlider" runat="server">
                        <ItemTemplate>
                            <div class="apparel_item" onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("Id") %>'">
                                <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                <p><%# Eval("Name") %></p>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="AccessoriesPanel" runat="server" CssClass="slider_container">
                <div class="slider_title_container">
                    <asp:Label ID="Label3" runat="server" Text="Accessories" CssClass="slider_title"></asp:Label>
                    <asp:HyperLink ID="HyperLink3" runat="server" CssClass="view_more_link" NavigateUrl="~/pages/Search.aspx?type=Accessory">
                        View More
                    </asp:HyperLink>
                </div>
                <div class="slider">
                    <asp:Repeater ID="accessoriesSlider" runat="server">
                        <ItemTemplate>
                            <div class="apparel_item" onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("Id") %>'">
                                <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                <p><%# Eval("Name") %></p>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="NoApparelsPanel" runat="server" CssClass="no_apparel_message_container" Visible="false">
                <div class="no_apparel_message">
                    <p>No apparels here yet,</p>
                    <p>Use<span style="font-size: 2.5rem;"> + </span>icon to add them!</p>
                </div>
                <div>
                    <img src="../assets/images/down_arrow.png" class="no_apparel_message_down_arrow" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

