using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrendBlend.pages
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check for authentication
                if (Session["FirstName"] != null)
                {
                    // User is in session
                    userNameLabel.Text = Session["FirstName"].ToString();
                }
            }
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            // Clear cookie if exists
            if (Request.Cookies["UserInfo"] != null)
            {
                HttpCookie userCookie = new HttpCookie("UserInfo");
                userCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userCookie);
            }

            // Clear session
            Session.Clear();

            // Redirect to login
            Response.Redirect("~/pages/SignIn.aspx");
        }
    }
}