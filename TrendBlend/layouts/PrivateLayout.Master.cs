using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
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


                if (userCookie == null && Session["Username"] == null)
                {
                    // Not authenticated, redirect to login
                    Response.Redirect("~/pages/SignIn.aspx");
                }
            }
        }

        protected void Account_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/pages/Account.aspx");
        }
    }
}