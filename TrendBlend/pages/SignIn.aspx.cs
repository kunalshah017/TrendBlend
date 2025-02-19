using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace TrendBlend.pages
{
    public partial class SignIn : System.Web.UI.Page
    {

        private string HashPassword(string password)
        {
            // Get salt from config
            string salt = ConfigurationManager.AppSettings["PasswordSalt"];

            // Combine password and salt
            string saltedPassword = string.Concat(password, salt);

            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the salted password to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword);

                // Compute hash
                byte[] hash = sha256.ComputeHash(bytes);

                // Convert hash to string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }


        string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void HandleSignIn(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(cs);
            string hashedPassword = HashPassword(passwordInput.Text);

            string query = "SELECT FirstName, LastName FROM Users WHERE Username = @Username AND Password = @Password";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", userNameInput.Text);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Create cookies if remember me is checked
                        if (rememberMe.Checked)
                        {
                            HttpCookie userCookie = new HttpCookie("UserInfo");
                            userCookie.Values["Username"] = userNameInput.Text;
                            userCookie.Values["Password"] = hashedPassword;
                            Session["FirstName"] = reader["FirstName"].ToString();
                            Session["LastName"] = reader["LastName"].ToString();
                            userCookie.Expires = DateTime.MaxValue;
                            Response.Cookies.Add(userCookie);
                        }
                        else
                        {
                            Session["Username"] = userNameInput.Text;
                            Session["FirstName"] = reader["FirstName"].ToString();
                            Session["LastName"] = reader["LastName"].ToString();
                        }

                        Response.Redirect("~/pages/Home.aspx");
                    }
                    else
                    {
                        errorLabel.Visible = true;
                        errorLabel.Text = "Incorrect username or password";
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