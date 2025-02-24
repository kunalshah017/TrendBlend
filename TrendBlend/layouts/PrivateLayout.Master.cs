using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace TrendBlend.layouts
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check for authentication
                HttpCookie userCookie = Request.Cookies["UserInfo"];
                if (userCookie == null && Session["UserName"] == null)
                {
                    Response.Redirect("~/pages/Onboarding.aspx");
                    return;
                }

                // Get current page
                string currentPage = Request.Path.ToLower();

                // Handle logo/back button visibility
                bool isHomePage = currentPage.EndsWith("/pages/home.aspx");
                bool isAccountPage = currentPage.EndsWith("/pages/account.aspx");
                bool isSettingsPage = currentPage.EndsWith("/pages/settings.aspx");

                // Set visibility and navigation based on page
                LogoPanel.Visible = isHomePage;
                BackButtonPanel.Visible = !isHomePage;
                RightButtonPanel.Visible = !isSettingsPage;

                backLabel.Text = "Home";

                // Set back button navigation and right icon based on page
                if (isSettingsPage)
                {
                    // Cast to HyperLink to access NavigateUrl
                    var backLink = (HyperLink)BackButtonPanel.FindControl("BackLink");
                    if (backLink != null)
                    {
                        backLink.NavigateUrl = "~/pages/Account.aspx";
                        backLabel.Text = "Account";
                    }
                }

                // Update the right icon and navigation
                var rightIcon = (HtmlGenericControl)TopBarRightLink.FindControl("TopBarRightIcon");
                if (rightIcon != null)
                {
                    rightIcon.Attributes["class"] = isAccountPage ? "fa fa-bars" : "fa fa-user";
                }

                TopBarRightLink.NavigateUrl = isAccountPage ? "~/pages/Settings.aspx" : "~/pages/Account.aspx";
            }
        }

        protected void Account_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/pages/Account.aspx");
        }
    }
}