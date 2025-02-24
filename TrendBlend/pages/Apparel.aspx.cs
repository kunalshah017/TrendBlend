namespace TrendBlend.pages
{
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
                        ApparelColor.Text = GetColorName(r, g, b);


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
