<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="TrendBlend.pages.SignIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>TrendBlend | SignIn</title>
    <link href="/styles/SignIn/styles.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <div class="main_container">
        <div class="back_button" id="backButton">
            <i class="fa fa-arrow-left"></i>
        </div>

        <div class="page_title">
            <div>
                Welcome back,
            </div>
            <div>
                Sign In
            </div>
        </div>

        <div class="login_form">
            <div style="color: red;">
                <asp:Label runat="server" ID="errorLabel" Visible="false" CssClass="error_label" />
            </div>

            <div class="input_container">
                <p>Username</p>
                <asp:TextBox runat="server" ID="userNameInput" CssClass="text_input" />
            </div>
            <div class="input_container">
                <p>Password</p>
                <div style="display: flex; align-items: center; gap: 20px;">
                    <asp:TextBox runat="server" ID="passwordInput" CssClass="text_input" TextMode="Password" />
                    <i class="fa fa-eye-slash password_toggle" style="font-size: 1.5rem; color: var(--medium-slate-blue);" id="passwordToggle"></i>
                </div>
            </div>
        </div>

        <div class="cta_button_container">
            <div>
                <asp:Button ID="signInButton" OnClick="HandleSignIn" runat="server" Text="Done!" CssClass="cta_button" />
            </div>
        </div>

    </div>
    <script type="text/javascript">
        $(document).ready(function () {

            var userNameInput = $('#<%= userNameInput.ClientID %>');
            var passwordInput = $('#<%= passwordInput.ClientID %>');
            var signInButton = $('#<%= signInButton.ClientID %>');
            var errorMessage = $('#<%= errorLabel.ClientID %>');

            $('#passwordToggle').click(function () {
                // Toggle icon
                $(this).toggleClass('fa-eye fa-eye-slash');

                // Toggle password visibility
                var passwordInput = $('#<%= passwordInput.ClientID %>');
                var type = passwordInput.attr('type');

                if (type === 'password') {
                    passwordInput.attr('type', 'text');
                } else {
                    passwordInput.attr('type', 'password');
                }
            });

            signInButton.prop('disabled', true);
            signInButton.css('opacity', '0.5');

            // Function to validate inputs and update button state
            function validateInputs() {
                var username = userNameInput.val().trim();
                var password = passwordInput.val().trim();

                // Enable/disable button based on validation
                if (username !== '' && password !== '') {
                    signInButton.prop('disabled', false);
                    signInButton.css('opacity', '1');
                } else {
                    signInButton.prop('disabled', true);
                    signInButton.css('opacity', '0.5');
                }
            }

            // Add event listeners for input changes
            userNameInput.on('input', validateInputs);
            passwordInput.on('input', validateInputs);

            $('#backButton').on('click', function () {
                window.location.href = '/pages/onboarding.aspx';
            });
        });
    </script>
</asp:Content>
