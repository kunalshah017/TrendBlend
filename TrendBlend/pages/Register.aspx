<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PublicLayout.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="TrendBlend.pages.Register" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>TrendBlend | Register</title>
    <link rel="stylesheet" type="text/css" href="/styles/Register/styles.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

    <div class="main_container">
        <div class="back_button" id="backButton">
            <i class="fa fa-arrow-left"></i>
        </div>
        <div class="page_title">
            Register
        </div>
        <div style="color: red;">
            <asp:Label ID="ErrorLabel" runat="server" />
        </div>
        <div class="register_form">

            <div class="multi_input_container">
                <div class="input_container">
                    <p>First Name</p>
                    <asp:TextBox ID="firstNameInput" runat="server" CssClass="text_input"></asp:TextBox>
                </div>
                <div class="input_container">
                    <p>Last Name</p>
                    <asp:TextBox ID="lastNameInput" runat="server" CssClass="text_input"></asp:TextBox>
                </div>
            </div>
            <div class="multi_input_container">
                <div class="input_container" style="flex-basis: 30%;">
                    <p>Age</p>
                    <asp:TextBox ID="ageInput" runat="server" CssClass="text_input" TextMode="Number"></asp:TextBox>
                </div>
                <div class="input_container" style="flex-basis: 70%;">
                    <p>Favourite Color</p>
                    <div style="display: flex; gap: 10px;">
                        <div style="display: flex; align-items: center; flex-basis: 70%;">
                            <asp:DropDownList ID="colorInput" runat="server" CssClass="text_input" Style="flex: 1;">
                                <asp:ListItem Text="Select a color" Value="" />
                                <asp:ListItem Text="Red" Value="#FF0000" />
                                <asp:ListItem Text="Blue" Value="#0000FF" />
                                <asp:ListItem Text="Green" Value="#008000" />
                                <asp:ListItem Text="Purple" Value="#800080" />
                                <asp:ListItem Text="Pink" Value="#FFC0CB" />
                                <asp:ListItem Text="Orange" Value="#FFA500" />
                            </asp:DropDownList>
                        </div>
                        <input type="color" runat="server" id="customColorPicker" class="text_input" style="flex-basis: 20%;" />
                    </div>
                </div>
            </div>
            <div class="input_container">
                <p>Email</p>
                <asp:TextBox ID="emailInput" CssClass="text_input" TextMode="Email" runat="server" />
            </div>
            <div class="input_container">
                <p>Username</p>
                <asp:TextBox ID="userNameInput" CssClass="text_input" runat="server" />
            </div>
            <div class="input_container">
                <p>Password</p>
                <asp:TextBox ID="passwordInput" CssClass="text_input" TextMode="Password" runat="server" />
            </div>
        </div>
        <div class="cta_button_container">
            <div>
                <asp:Button ID="registerButton" runat="server" OnClick="HandleRegister" Text="Done!" CssClass="cta_button" />
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            // Get references to all input elements
            var firstNameInput = $('#<%= firstNameInput.ClientID %>');
            var lastNameInput = $('#<%= lastNameInput.ClientID %>');
            var ageInput = $('#<%= ageInput.ClientID %>');
            var colorInput = $('#<%= colorInput.ClientID %>');
            var emailInput = $('#<%= emailInput.ClientID %>');
            var userNameInput = $('#<%= userNameInput.ClientID %>');
            var passwordInput = $('#<%= passwordInput.ClientID %>');
            var registerButton = $('#<%= registerButton.ClientID %>');
            var errorLabel = $('#<%= ErrorLabel.ClientID %>');

            // Initially disable the button
            registerButton.prop('disabled', true);
            registerButton.css('opacity', '0.5');

            // Email validation function
            function isValidEmail(email) {
                var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                return emailRegex.test(email);
            }

            // Function to validate all inputs
            function validateInputs() {
                var firstName = firstNameInput.val().trim();
                var lastName = lastNameInput.val().trim();
                var age = ageInput.val().trim();
                var color = colorInput.val().trim();
                var email = emailInput.val().trim();
                var username = userNameInput.val().trim();
                var password = passwordInput.val().trim();

                // Clear previous error
                errorLabel.hide();

                // Check each field and display appropriate error
                if (!firstName) {
                    errorLabel.text('First Name is required');
                    errorLabel.show();
                    return false;
                }
                if (!lastName) {
                    errorLabel.text('Last Name is required');
                    errorLabel.show();
                    return false;
                }
                if (!age) {
                    errorLabel.text('Age is required');
                    errorLabel.show();
                    return false;
                }
                if (parseInt(age) < 5 || parseInt(age) > 100) {
                    errorLabel.text('Please enter a valid age');
                    errorLabel.show();
                    return false;
                }
                if (!color) {
                    errorLabel.text('Please select a color');
                    errorLabel.show();
                    return false;
                }
                if (!email) {
                    errorLabel.text('Email is required');
                    errorLabel.show();
                    return false;
                }
                if (!isValidEmail(email)) {
                    errorLabel.text('Please enter a valid email address');
                    errorLabel.show();
                    return false;
                }
                if (!username) {
                    errorLabel.text('Username is required');
                    errorLabel.show();
                    return false;
                }
                if (!password) {
                    errorLabel.text('Password is required');
                    errorLabel.show();
                    return false;
                }

                // If all validations pass
                return true;
            }

            // Function to update button state
            function updateButtonState() {
                if (validateInputs()) {
                    registerButton.prop('disabled', false);
                    registerButton.css('opacity', '1');
                    errorLabel.hide();
                } else {
                    registerButton.prop('disabled', true);
                    registerButton.css('opacity', '0.5');
                }
            }

            firstNameInput.on('input', updateButtonState);
            lastNameInput.on('input', updateButtonState);
            ageInput.on('input', updateButtonState);
            colorInput.on('change', updateButtonState);
            emailInput.on('input', updateButtonState);
            userNameInput.on('input', updateButtonState);
            passwordInput.on('input', updateButtonState);

            function updateColorPreview(color) {
                $('.color-preview').css('background-color', color);
            }

            $('#<%= customColorPicker.ClientID %>').on('change', function () {
                var hexValue = $(this).val();
                var $dropdown = $('#<%= colorInput.ClientID %>');

                // Remove existing custom option if present
                $dropdown.find('option:contains("Custom Color")').remove();

                // Add new custom option
                $('<option>', {
                    value: hexValue,
                    text: 'Custom Color'
                }).appendTo($dropdown);

                // Set the dropdown to the new value
                $dropdown.val(hexValue);

                // Update color preview
                updateColorPreview(hexValue);
            });

            // Initialize color preview with current dropdown value
            $('#<%= colorInput.ClientID %>').on('change', function () {
                var selectedColor = $(this).val();
                $('#<%= customColorPicker.ClientID %>').val(selectedColor);
                updateColorPreview(selectedColor);
            });

            // Initial color preview
            updateColorPreview($('#<%= colorInput.ClientID %>').val());

            $('#backButton').on('click', function () {
                window.location.href = '/pages/onboarding.aspx';
            });
        });
    </script>
</asp:Content>
