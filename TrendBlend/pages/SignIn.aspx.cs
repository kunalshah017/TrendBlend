using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace TrendBlend.pages
{
    public partial class SignIn : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void HandleSignIn(object sender, EventArgs e)
        {

            SqlConnection con = new SqlConnection(cs);
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", userNameInput.Text);
            cmd.Parameters.AddWithValue("@Password", passwordInput.Text);
            con.Open();
            int a = (int)cmd.ExecuteScalar();
            if (a > 0)
            {

                Response.Redirect("/pages/Home.aspx");

            }
            else
            {
                errorLabel.Visible = true;
                errorLabel.Text = "Incorrect username or password";
            }

        }


    }
}