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

                if (Session["UserName"] == null)
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
                bool isSearchPage = currentPage.EndsWith("/pages/search.aspx");

                // Set visibility and navigation based on page
                LogoPanel.Visible = isHomePage;
                BackButtonPanel.Visible = !isHomePage;
                RightButtonPanel.Visible = !isSettingsPage;
                TopBarSettingsLink.Visible = isAccountPage;

                backLabel.Text = "Back";
            }
        }
    }
}