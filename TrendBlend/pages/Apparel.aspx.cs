namespace TrendBlend.pages
{
    using CloudinaryDotNet.Actions;
    using CloudinaryDotNet;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;

    /// <summary>
    /// Defines the <see cref="Apparel1" />
    /// </summary>
    public partial class Apparel1 : System.Web.UI.Page
    {
        /// <summary>
        /// Defines the cs
        /// </summary>
        internal string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

        /// <summary>
        /// The GetColorName
        /// </summary>
        /// <param name="rgbValue">The rgbValue<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string GetColorName(byte r, byte g, byte b)
        {
            if (r == 0 && g == 0 && b == 0) return "Black";
            if (r == 255 && g == 255 && b == 255) return "White";
            if (r > 220 && g < 30 && b < 30) return "Red";
            if (r < 30 && g > 220 && b < 30) return "Green";
            if (r < 30 && g < 30 && b > 220) return "Blue";
            if (r == g && g == b) return "Grayish Black";
            if (r > 220 && g > 220 && b < 30) return "Yellow";
            if (r > 220 && g < 30 && b > 220) return "Purple";
            if (r < 30 && g > 220 && b > 220) return "Cyan";

            if (r > g && r > b) return "Reddish";
            if (g > r && g > b) return "Greenish";
            if (b > r && b > g) return "Bluish";

            return "Custom Color";
        }

        /// <summary>
        /// The Page_Load
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserName"] == null)
                {
                    Response.Redirect("~/pages/SignIn.aspx");
                    return;
                }

                string apparelId = Request.QueryString["id"];
                if (string.IsNullOrEmpty(apparelId))
                {
                    ShowError();
                    return;
                }

                LoadApparelDetails(apparelId, Session["UserName"].ToString());
            }
        }

        /// <summary>
        /// The LoadApparelDetails
        /// </summary>
        /// <param name="apparelId">The apparelId<see cref="string"/></param>
        /// <param name="username">The username<see cref="string"/></param>
        private void LoadApparelDetails(string apparelId, string username)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT a.*, u.UserName 
                FROM Apparels a 
                INNER JOIN Users u ON a.UserID = u.Id 
                WHERE a.ApparelID = @ApparelId AND u.UserName = @Username";


                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ApparelId", apparelId);
                cmd.Parameters.AddWithValue("@Username", username);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        ApparelPanel.Visible = true;
                        ErrorPanel.Visible = false;

                        ApparelImage.ImageUrl = reader["ImageUrl"].ToString();
                        ApparelName.Text = reader["Name"].ToString();

                        Page.Title = $"TrendBlend | {reader["Name"].ToString()}";

                        ApparelType.Text = reader["Type"].ToString();
                        ApparelSize.Text = reader["Size"].ToString();

                        byte r = Convert.ToByte(reader["R"]);
                        byte g = Convert.ToByte(reader["G"]);
                        byte b = Convert.ToByte(reader["B"]);
                        string rgbValue = $"rgb({r}, {g}, {b})";
                        ColorCircle.Style["background-color"] = rgbValue;
                        if (reader["ColorName"].ToString() != "")
                        {
                            ApparelColor.Text = reader["ColorName"].ToString();
                        }
                        else
                        {
                            ApparelColor.Text = GetColorName(r, g, b);
                        }


                        // Handle Accessory Type
                        if (reader["Type"].ToString() == "Accessory")
                        {
                            AccessoryTypePanel.Visible = true;
                            ApparelAccessoryType.Text = reader["AccessoryType"].ToString();
                        }
                        else
                        {
                            AccessoryTypePanel.Visible = false;
                        }

                        ApparelDescription.Text = reader["Description"].ToString();

                        // Format and display creation date
                        if (reader["CreatedAt"] != DBNull.Value)
                        {
                            DateTime createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                            ApparelCreatedAt.Text = createdAt.ToString("MMM dd, yyyy");
                        }
                    }
                    else
                    {
                        ShowError();
                    }
                }
                catch (Exception)
                {
                    ShowError();
                }
            }
        }

        protected void ConfirmDeleteButton_Click(object sender, EventArgs e)
        {
            string apparelId = Request.QueryString["id"];
            if (string.IsNullOrEmpty(apparelId))
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // First get the image URL
                string getImageQuery = @"SELECT ImageUrl FROM Apparels 
                               WHERE ApparelID = @ApparelId 
                               AND UserID = (SELECT Id FROM Users WHERE UserName = @Username)";

                string imageUrl = null;
                using (SqlCommand getImageCmd = new SqlCommand(getImageQuery, con))
                {
                    getImageCmd.Parameters.AddWithValue("@ApparelId", apparelId);
                    getImageCmd.Parameters.AddWithValue("@Username", Session["UserName"].ToString());

                    imageUrl = (string)getImageCmd.ExecuteScalar();
                }

                if (string.IsNullOrEmpty(imageUrl))
                {
                    ShowError();
                    return;
                }

                try
                {
                    // Setup Cloudinary
                    Account account = new Account(
                        ConfigurationManager.AppSettings["CloudinaryCloud"],
                        ConfigurationManager.AppSettings["CloudinaryApiKey"],
                        ConfigurationManager.AppSettings["CloudinaryApiSecret"]
                    );

                    Cloudinary cloudinary = new Cloudinary(account);

                    // Extract public ID from URL
                    string publicId = imageUrl.Split(new[] { "/upload/" }, StringSplitOptions.None)[1];
                    publicId = publicId.Substring(publicId.IndexOf('/', publicId.IndexOf('/') + 1) + 1);
                    publicId = publicId.Substring(0, publicId.LastIndexOf('.')); // Remove file extension
                    publicId = "apparels/" + publicId; // Add folder name back

                    // Try to delete from Cloudinary first
                    var deleteParams = new DeletionParams(publicId);
                    var deletionResult = cloudinary.Destroy(deleteParams);

                    if (deletionResult.Result == "ok")
                    {
                        // Only if Cloudinary deletion was successful, delete from database
                        string deleteQuery = @"DELETE FROM Apparels 
                                     WHERE ApparelID = @ApparelId 
                                     AND UserID = (SELECT Id FROM Users WHERE UserName = @Username)";

                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, con))
                        {
                            deleteCmd.Parameters.AddWithValue("@ApparelId", apparelId);
                            deleteCmd.Parameters.AddWithValue("@Username", Session["UserName"].ToString());

                            int result = deleteCmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                Response.Redirect("~/pages/Home.aspx");
                            }
                            else
                            {
                                ShowError();
                            }
                        }
                    }
                    else
                    {
                        // If Cloudinary deletion failed, show error
                        ShowError();
                    }
                }
                catch (Exception ex)
                {
                    // Log the error for debugging
                    System.Diagnostics.Debug.WriteLine($"Error deleting apparel: {ex.Message}");
                    ShowError();
                }
            }
        }


        /// <summary>
        /// The ShowError
        /// </summary>
        private void ShowError()
        {
            ApparelPanel.Visible = false;
            ErrorPanel.Visible = true;
        }
    }
}
