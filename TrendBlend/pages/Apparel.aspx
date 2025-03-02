<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="Apparel.aspx.cs" Inherits="TrendBlend.pages.Apparel1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/styles/Apparel/styles.css" rel="stylesheet" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="main_container">
        <asp:Panel ID="ApparelPanel" runat="server">

            <div class="apparel_content">
                <div class="apparel_image_container">
                    <div>
                        <asp:Image ID="ApparelImage" runat="server" CssClass="apparel_image" />
                    </div>
                </div>
                <div class="apparel_info">
                    <h1 class="apparel_name">
                        <asp:Label ID="ApparelName" runat="server" />
                    </h1>
                    <div class="info_grid">
                        <div class="info_item">
                            <span class="info_label">Type</span>
                            <asp:Label ID="ApparelType" runat="server" CssClass="info_value" />
                        </div>
                        <div class="info_item">
                            <span class="info_label">Size</span>
                            <asp:Label ID="ApparelSize" runat="server" CssClass="info_value" />
                        </div>
                        <asp:Panel ID="AccessoryTypePanel" runat="server" CssClass="info_item" Visible="false">
                            <span class="info_label">Accessory Type</span>
                            <asp:Label ID="ApparelAccessoryType" runat="server" CssClass="info_value" />
                        </asp:Panel>
                        <div class="info_item">
                            <span class="info_label">Added On</span>
                            <asp:Label ID="ApparelCreatedAt" runat="server" CssClass="info_value" />
                        </div>
                        <div class="info_item">
                            <span class="info_label">Color</span>
                            <div class="color-info">
                                <span id="ColorCircle" runat="server" class="color-circle"></span>
                                <asp:Label ID="ApparelColor" runat="server" CssClass="info_value" />
                            </div>
                        </div>
                    </div>
                    <div class="description_section">
                        <span class="info_label">Description</span>
                        <asp:Label ID="ApparelDescription" runat="server" CssClass="description_text" />
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="ErrorPanel" runat="server" Visible="false" CssClass="error_panel">
            <div class="error_content">
                <i class="fa fa-exclamation-circle error_icon" style="font-size: 3rem;"></i>
                <h2>Apparel Not Found</h2>
                <p>The requested apparel could not be found or you don't have access to view it.</p>
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
                <h3>Delete Apparel?</h3>
                <p>Are you sure you want to delete this apparel? This action cannot be undone.</p>
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
            const $deleteModal = $('#deleteConfirmModal');
            const $deleteButton = $('#deleteButton');

            $deleteButton.on('click', function (e) {
                e.preventDefault();
                showDeleteModal();
            });

            function showDeleteModal() {
                console.log('Showing modal'); // Debug log
                $deleteModal.addClass('visible');
            }

            function hideDeleteModal() {
                console.log('Hiding modal'); // Debug log
                $deleteModal.removeClass('visible');
            }

            // Make hideDeleteModal available globally for the cancel button
            window.hideDeleteModal = hideDeleteModal;
        });
</script>



</asp:Content>
