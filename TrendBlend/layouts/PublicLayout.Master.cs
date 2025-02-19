using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrendBlend.layouts
{
    public partial class Site2 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie userCookie = Request.Cookies["UserInfo"];

            if (userCookie != null)
            {
                string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

                SqlConnection con = new SqlConnection(cs);

                string query = "SELECT FirstName, LastName FROM Users WHERE Username = @Username AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", userCookie["Username"].ToString());
                cmd.Parameters.AddWithValue("@Password", userCookie["Password"].ToString());

                try
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userCookie.Expires = DateTime.MaxValue;
                            Session["Username"] = userCookie["Username"].ToString();
                            Session["FirstName"] = reader["FirstName"].ToString();
                            Session["LastName"] = reader["LastName"].ToString();
                            Response.Cookies.Add(userCookie);
                            Response.Redirect("~/pages/Home.aspx");
                        }
                        else
                        {
                            userCookie.Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies.Add(userCookie);
                            Response.Redirect("~/pages/SignIn.aspx");
                        }
                    }
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}