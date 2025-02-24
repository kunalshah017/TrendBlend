<%@ Page Title="" Language="C#" MasterPageFile="~/layouts/PrivateLayout.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="TrendBlend.pages.Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/styles/Search/styles.css" rel="stylesheet" />
    <title>TrendBlend | Search</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="main_container">
        <div class="search_container">
            <div class="search_inputs">
                <div class="input_group">
                    <asp:TextBox ID="SearchText" runat="server" CssClass="search_text" placeholder="Search apparels..." />
                </div>
                <div class="multi_input_container">
                    <div class="input_group">
                        <asp:DropDownList ID="TypeDropDown" runat="server" CssClass="search_dropdown">
                            <asp:ListItem Text="All Types" Value="" />
                            <asp:ListItem Text="Top" Value="Top" />
                            <asp:ListItem Text="Bottom" Value="Bottom" />
                            <asp:ListItem Text="Footwear" Value="Footwear" />
                            <asp:ListItem Text="Accessory" Value="Accessory" />
                        </asp:DropDownList>
                    </div>
                    <div class="input_group">
                        <p>Color Search</p>
                        <input type="color" id="ColorInput" runat="server" class="search_color" value="#ffffff" title="Select color to filter" />
                        <asp:HiddenField ID="SelectedColorHidden" runat="server" Value="null" />
                    </div>
                </div>
                <asp:Button ID="SearchButton" runat="server" Text="Search 🔍" CssClass="search_button" OnClick="SearchButton_Click" />
            </div>
        </div>

        <div class="results_container">
            <asp:Repeater ID="SearchResults" runat="server">
                <ItemTemplate>
                    <div class="apparel_card">
                        <div class="apparel_image">
                            <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("Name") %>' />
                        </div>
                        <div class="apparel_info">
                            <h3><%# Eval("Name") %></h3>
                            <p>Size: <%# Eval("Size") %></p>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            const colorInput = $("#<%= ColorInput.ClientID %>");
            const selectedColorHidden = $("#<%= SelectedColorHidden.ClientID %>");
            const searchButton = $("#<%= SearchButton.ClientID %>");

            // Handle color input changes
            colorInput.on('input change', function () {
                const color = this.value;
                this.style.backgroundColor = color;
                selectedColorHidden.val(color);

            });

            // Initialize color input background
            colorInput[0].style.backgroundColor = colorInput.val();
        });
    </script>

</asp:Content>
