using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.Text;

namespace TrendBlend.pages
{
    public partial class Register : System.Web.UI.Page
    {
        protected TextBox firstNameInput;
        protected TextBox lastNameInput;
        protected TextBox userNameInput;
        protected TextBox passwordInput;
        protected TextBox emailInput;
        protected TextBox ageInput;
        protected DropDownList colorInput;
        protected HtmlInputGenericControl customColorPicker;

        string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

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

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void HandleRegister(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(cs);

            try
            {
                // Check for existing username
                string query = "SELECT UserName FROM Users WHERE UserName = @userName";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userName", userNameInput.Text);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    ErrorLabel.Text = "Try a different username";
                    userNameInput.Focus();
                    reader.Close();
                    return;
                }
                reader.Close();
                con.Close();

                // Check for existing email
                query = "SELECT Email FROM Users WHERE Email = @email";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", emailInput.Text);
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    ErrorLabel.Text = "Email is already registered";
                    emailInput.Focus();
                    reader.Close();
                    return;
                }
                reader.Close();
                con.Close();

                string hashedPassword = HashPassword(passwordInput.Text);


                // Add user to db
                query = "INSERT INTO Users(FirstName, LastName, UserName, Password, Email, Age, FavouriteColor) VALUES (@firstname, @lastname, @userName, @password, @email, @age, @favColor)";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@firstName", firstNameInput.Text);
                cmd.Parameters.AddWithValue("@lastName", lastNameInput.Text);
                cmd.Parameters.AddWithValue("@userName", userNameInput.Text);
                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@email", emailInput.Text);
                cmd.Parameters.AddWithValue("@age", ageInput.Text);
                cmd.Parameters.AddWithValue("@favColor", colorInput.SelectedValue);
                con.Open();
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    Response.Redirect("~/pages/Home.aspx");
                }
                else
                {
                    ErrorLabel.Text = "Some Error Occurred :(";
                }
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = "An error occurred. Please try again." + ex;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
    }
}