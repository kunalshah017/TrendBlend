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
    public partial class Login : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            ViewState["Password"] = passwordTextBox.Text;

            if (CheckBox1.Checked)
            {

                passwordTextBox.TextMode = TextBoxMode.SingleLine;
            }
            else
            {
                passwordTextBox.TextMode = TextBoxMode.Password;
            }
            passwordTextBox.Attributes["value"] = ViewState["Password"].ToString();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {

            SqlConnection con = new SqlConnection(cs);
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", usernameTextBox.Text);
            cmd.Parameters.AddWithValue("@Password", passwordTextBox.Text);
            con.Open();
            int a = (int)cmd.ExecuteScalar();
            if (a > 0)
            {

                Response.Redirect("/pages/Home.aspx");

            }
            else
            {
                Label1.Visible = true;
                Label1.Text = "incorrect username or password";
            }
            con.Close();

        }


    }
}