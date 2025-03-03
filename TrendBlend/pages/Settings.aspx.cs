using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TrendBlend.pages
{
    public partial class Settings : System.Web.UI.Page
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        { }

        private string HashPassword(string password)
        {
            string salt = ConfigurationManager.AppSettings["PasswordSalt"];
            string saltedPassword = string.Concat(password, salt);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        protected void ResetPasswordButton_Click(object sender, EventArgs e)
        {
            string newPassword = NewPasswordInput.Text;
            string confirmPassword = ConfirmPasswordInput.Text;

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                PasswordErrorLabel.Text = "Please fill in both password fields";
                PasswordErrorLabel.Visible = true;
                return;
            }

            if (newPassword != confirmPassword)
            {
                PasswordErrorLabel.Text = "Passwords do not match";
                PasswordErrorLabel.Visible = true;
                return;
            }

            string hashedPassword = HashPassword(newPassword);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "UPDATE Users SET Password = @Password WHERE UserName = @Username";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@Username", Session["UserName"].ToString());

                try
                {
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        // Clear the password fields
                        NewPasswordInput.Text = "";
                        ConfirmPasswordInput.Text = "";

                        // Show success message
                        PasswordErrorLabel.Text = "Password updated successfully";
                        PasswordErrorLabel.CssClass = "success_message";
                        PasswordErrorLabel.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    PasswordErrorLabel.Text = "Error updating password";
                    PasswordErrorLabel.Visible = true;
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
            Response.Redirect("~/pages/Onboarding.aspx");
        }
    }
}
