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

            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", userNameInput.Text);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
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