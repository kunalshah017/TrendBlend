<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="Blend.aspx.cs" Inherits="TrendBlend.pages.Blend" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/styles/Blend/styles.css" rel="stylesheet" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="main_container">
        <asp:Panel ID="BlendPanel" runat="server">
            <div class="blend_content">
                <div class="images_slider">
                    <button type="button" class="slider_nav prev_btn">
                        <i class="fa fa-chevron-left"></i>
                    </button>
                    <div class="slider_container" id="imageSlider">
                        <asp:Repeater ID="ImagesRepeater" runat="server">
                            <ItemTemplate>
                                <div class="slider_image">
                                    <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <button type="button" class="slider_nav next_btn">
                        <i class="fa fa-chevron-right"></i>
                    </button>
                </div>

                <div class="blend_info">
                    <h1 class="blend_name">
                        <asp:Label ID="BlendName" runat="server" />
                    </h1>
                    <p class="blend_date">
                        Created on
                       
                        <asp:Label ID="BlendDate" runat="server" />
                    </p>
                </div>

                <div class="apparels_section">
                    <!-- Tops Section -->
                    <asp:Panel ID="TopsPanel" runat="server" CssClass="apparel_type_section">
                        <h2 class="type_header">Tops</h2>
                        <div class="apparels_grid">
                            <asp:Repeater ID="TopsRepeater" runat="server">
                                <ItemTemplate>
                                    <div class="apparel_card">
                                        <div class="apparel_image" onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("ApparelID") %>'">
                                            <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                        </div>
                                        <div class="apparel_info">
                                            <h3 onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("ApparelID") %>'"><%# Eval("Name") %></h3>
                                            <p><%# !string.IsNullOrEmpty(Eval("Size")?.ToString()) ? $"Size: {Eval("Size")}" : "" %></p>
                                            <div class="color-display">
                                                <span class="color-circle" style='background-color: rgb(<%# Eval("R") %>, <%# Eval("G") %>, <%# Eval("B") %>)'></span>
                                                <span class="color-name"><%# !string.IsNullOrEmpty(Eval("ColorName")?.ToString()) ? Eval("ColorName") : GetColorName(Convert.ToByte(Eval("R")), Convert.ToByte(Eval("G")), Convert.ToByte(Eval("B"))) %></span>
                                            </div>
                                            <asp:LinkButton ID="RemoveButton" runat="server"
                                                CssClass="remove_button"
                                                OnClick="RemoveButton_Click"
                                                CommandArgument='<%# Eval("ApparelID") %>'>
            <i class="fa fa-minus"></i> Remove
        </asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:Panel>

                    <!-- Bottoms Section -->
                    <asp:Panel ID="BottomsPanel" runat="server" CssClass="apparel_type_section">
                        <h2 class="type_header">Bottoms</h2>
                        <div class="apparels_grid">
                            <asp:Repeater ID="BottomsRepeater" runat="server">
                                <ItemTemplate>
                                    <div class="apparel_card">
                                        <div class="apparel_image" onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("ApparelID") %>'">
                                            <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                        </div>
                                        <div class="apparel_info">
                                            <h3 onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("ApparelID") %>'"><%# Eval("Name") %></h3>
                                            <p><%# !string.IsNullOrEmpty(Eval("Size")?.ToString()) ? $"Size: {Eval("Size")}" : "" %></p>
                                            <div class="color-display">
                                                <span class="color-circle" style='background-color: rgb(<%# Eval("R") %>, <%# Eval("G") %>, <%# Eval("B") %>)'></span>
                                                <span class="color-name"><%# !string.IsNullOrEmpty(Eval("ColorName")?.ToString()) ? Eval("ColorName") : GetColorName(Convert.ToByte(Eval("R")), Convert.ToByte(Eval("G")), Convert.ToByte(Eval("B"))) %></span>
                                            </div>
                                            <asp:LinkButton ID="RemoveButton" runat="server"
                                                CssClass="remove_button"
                                                OnClick="RemoveButton_Click"
                                                CommandArgument='<%# Eval("ApparelID") %>'>
            <i class="fa fa-minus"></i> Remove
        </asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:Panel>

                    <!-- Footwear Section -->
                    <asp:Panel ID="FootwearPanel" runat="server" CssClass="apparel_type_section">
                        <h2 class="type_header">Footwear</h2>
                        <div class="apparels_grid">
                            <asp:Repeater ID="FootwearRepeater" runat="server">
                                <ItemTemplate>
                                    <div class="apparel_card">
                                        <div class="apparel_image" onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("ApparelID") %>'">
                                            <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                        </div>
                                        <div class="apparel_info">
                                            <h3 onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("ApparelID") %>'"><%# Eval("Name") %></h3>
                                            <p><%# !string.IsNullOrEmpty(Eval("Size")?.ToString()) ? $"Size: {Eval("Size")}" : "" %></p>
                                            <div class="color-display">
                                                <span class="color-circle" style='background-color: rgb(<%# Eval("R") %>, <%# Eval("G") %>, <%# Eval("B") %>)'></span>
                                                <span class="color-name"><%# !string.IsNullOrEmpty(Eval("ColorName")?.ToString()) ? Eval("ColorName") : GetColorName(Convert.ToByte(Eval("R")), Convert.ToByte(Eval("G")), Convert.ToByte(Eval("B"))) %></span>
                                            </div>
                                            <asp:LinkButton ID="RemoveButton" runat="server"
                                                CssClass="remove_button"
                                                OnClick="RemoveButton_Click"
                                                CommandArgument='<%# Eval("ApparelID") %>'>
            <i class="fa fa-minus"></i> Remove
        </asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:Panel>

                    <!-- Accessories Section -->
                    <asp:Panel ID="AccessoriesPanel" runat="server" CssClass="apparel_type_section">
                        <h2 class="type_header">Accessories</h2>
                        <div class="apparels_grid">
                            <asp:Repeater ID="AccessoriesRepeater" runat="server">
                                <ItemTemplate>
                                    <div class="apparel_card">
                                        <div class="apparel_image" onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("ApparelID") %>'">
                                            <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                                        </div>
                                        <div class="apparel_info">
                                            <h3 onclick="window.location.href='/pages/Apparel.aspx?id=<%# Eval("ApparelID") %>'"><%# Eval("Name") %></h3>
                                            <p><%# !string.IsNullOrEmpty(Eval("Size")?.ToString()) ? $"Size: {Eval("Size")}" : "" %></p>
                                            <div class="color-display">
                                                <span class="color-circle" style='background-color: rgb(<%# Eval("R") %>, <%# Eval("G") %>, <%# Eval("B") %>)'></span>
                                                <span class="color-name"><%# !string.IsNullOrEmpty(Eval("ColorName")?.ToString()) ? Eval("ColorName") : GetColorName(Convert.ToByte(Eval("R")), Convert.ToByte(Eval("G")), Convert.ToByte(Eval("B"))) %></span>
                                            </div>
                                            <asp:LinkButton ID="RemoveButton" runat="server"
                                                CssClass="remove_button"
                                                OnClick="RemoveButton_Click"
                                                CommandArgument='<%# Eval("ApparelID") %>'>
            <i class="fa fa-minus"></i> Remove
        </asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="ErrorPanel" runat="server" Visible="false" CssClass="error_panel">
            <div class="error_content">
                <i class="fa fa-exclamation-circle error_icon"></i>
                <h2>Blend Not Found</h2>
                <p>The requested blend could not be found or you don't have access to view it.</p>
                <asp:HyperLink runat="server" NavigateUrl="~/pages/Home.aspx" CssClass="error_back_button">
                    <i class="fa fa-arrow-left"></i> Back to Home
                </asp:HyperLink>
            </div>
        </asp:Panel>
        <button type="button" id="deleteButton" class="delete_button">
            <i class="fa fa-trash"></i>
        </button>
        <div id="deleteConfirmModal" class="delete_confirm_modal">
            <div class="delete_confirm_content">
                <i class="fa fa-exclamation-triangle"></i>
                <h3>Delete Blend?</h3>
                <p>Are you sure you want to delete this blend? This action cannot be undone.</p>
                <div class="delete_confirm_buttons">
                    <asp:Button ID="CancelDeleteButton" runat="server" Text="Cancel"
                        CssClass="delete_confirm_button delete_confirm_cancel" OnClientClick="hideDeleteModal(); return false;" />
                    <asp:Button ID="ConfirmDeleteButton" runat="server" Text="Delete"
                        CssClass="delete_confirm_button delete_confirm_delete" OnClick="ConfirmDeleteButton_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            const slider = $('#imageSlider');
            const prevBtn = $('.prev_btn');
            const nextBtn = $('.next_btn');

            prevBtn.click(() => {
                slider[0].scrollBy({
                    left: -320,
                    behavior: 'smooth'
                });
            });

            nextBtn.click(() => {
                slider[0].scrollBy({
                    left: 320,
                    behavior: 'smooth'
                });
            });

            const $deleteModal = $('#deleteConfirmModal');
            const $deleteButton = $('#deleteButton');

            $deleteButton.on('click', function (e) {
                e.preventDefault();
                showDeleteModal();
            });

            function showDeleteModal() {
                $deleteModal.addClass('visible');
            }

            function hideDeleteModal() {
                $deleteModal.removeClass('visible');
            }

            window.hideDeleteModal = hideDeleteModal;
        });
    </script>
</asp:Content>
