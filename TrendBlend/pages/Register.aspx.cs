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
    public partial class WebForm3 : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(cs);
            string query = "insert into Users(firstName, lastName, age, favColor, userName, password, email) values (@firstname, @lastname, @age, @favColor, @userName, @password, @email)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@firstName", fNameInput.Text);
            cmd.Parameters.AddWithValue("@lastName", lNameInput.Text);
            cmd.Parameters.AddWithValue("@age", ageinput.Text);
            cmd.Parameters.AddWithValue("@favColor", colorinput.Text);
            cmd.Parameters.AddWithValue("@userName", usernameinput.Text);
            cmd.Parameters.AddWithValue("@password", passwordinput.Text);
            cmd.Parameters.AddWithValue("@email", emailinput.Text);
            con.Open();
            int a = cmd.ExecuteNonQuery();
            if (a > 0)
            {
                Response.Redirect("~/pages/Home.aspx");
            }
            
            
        }
    }
}