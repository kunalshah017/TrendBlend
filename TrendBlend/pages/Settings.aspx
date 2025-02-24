<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="TrendBlend.pages.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>TrendBlend | Settings</title>
    <link href="/styles/Settings/styles.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="main_container">
        <div class="settings_container">
            <div class="setting_item">
                <div class="setting_header" onclick="togglePasswordReset()">
                    <span>Reset Password</span>
                    <i class="fa fa-chevron-down"></i>
                </div>
                <div class="setting_content" id="passwordResetContent">
                    <div class="input_group">
                        <asp:TextBox ID="NewPasswordInput" runat="server" TextMode="Password"
                            CssClass="settings_input" placeholder="New Password" />
                    </div>
                    <div class="input_group">
                        <asp:TextBox ID="ConfirmPasswordInput" runat="server" TextMode="Password"
                            CssClass="settings_input" placeholder="Confirm Password" />
                    </div>
                    <asp:Label ID="PasswordErrorLabel" runat="server" CssClass="error_message" Visible="false" />
                    <asp:Button ID="ResetPasswordButton" runat="server" Text="Reset Password"
                        CssClass="settings_button" OnClick="ResetPasswordButton_Click" />
                </div>
            </div>

            <div class="logout_container">
                <p>Logout of TrendBlend</p>
                <asp:Button ID="LogoutButton" runat="server" Text="Logout"
                    CssClass="settings_button logout_button" OnClick="LogoutButton_Click" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function togglePasswordReset() {
            const content = document.getElementById('passwordResetContent');
            const header = content.previousElementSibling;
            const icon = header.querySelector('i');

            content.classList.toggle('expanded');
            icon.classList.toggle('rotated');
        }
    </script>
</asp:Content>
