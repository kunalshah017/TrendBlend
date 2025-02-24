using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text.RegularExpressions;

namespace TrendBlend.services
{
    /// <summary>
    ///    Used to submit an apparel to the database
    /// </summary>
    /// 
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class ApparelService : System.Web.Services.WebService
    {
        private static string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

        private (int R, int G, int B) ParseColor(string color)
        {
            // Remove any whitespace
            color = color.Trim();

            if (color.StartsWith("#"))
            {
                // Parse hex color
                color = color.TrimStart('#');
                return (
                    R: Convert.ToInt32(color.Substring(0, 2), 16),
                    G: Convert.ToInt32(color.Substring(2, 2), 16),
                    B: Convert.ToInt32(color.Substring(4, 2), 16)
                );
            }
            else if (color.StartsWith("rgb"))
            {
                // Parse RGB color
                var matches = Regex.Match(color, @"rgb\((\d+),\s*(\d+),\s*(\d+)\)");
                if (matches.Success)
                {
                    return (
                        R: int.Parse(matches.Groups[1].Value),
                        G: int.Parse(matches.Groups[2].Value),
                        B: int.Parse(matches.Groups[3].Value)
                    );
                }
            }

            // Default to black if parsing fails
            return (0, 0, 0);
        }

        [WebMethod]
        public string SubmitApparel(
            string username,
            string imageData,
            string apparelName,
            string apparelType,
            string color,
            string size,
            string accessoryType,
            string description
        )
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                    return "Error: User not authenticated";

                // Setup Cloudinary
                Account account = new Account(
                    ConfigurationManager.AppSettings["CloudinaryCloud"],
                    ConfigurationManager.AppSettings["CloudinaryApiKey"],
                    ConfigurationManager.AppSettings["CloudinaryApiSecret"]
                );

                Cloudinary cloudinary = new Cloudinary(account);

                // Upload image to Cloudinary
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageData),
                    Folder = "apparels"
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                string imageUrl = uploadResult.SecureUrl.ToString();

                var (R, G, B) = ParseColor(color);

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    // Get User ID
                    string userIdQuery = "SELECT Id FROM Users WHERE UserName = @Username";
                    using (SqlCommand userCmd = new SqlCommand(userIdQuery, con))
                    {
                        userCmd.Parameters.AddWithValue("@Username", username);
                        int userId = (int)userCmd.ExecuteScalar();

                        // Insert Apparel
                        string insertQuery = @"INSERT INTO Apparels 
                    (UserID, Name, Type, Size, AccessoryType, Description, ImageUrl, CreatedAt, R, G, B) 
                    VALUES (@UserID, @Name, @Type, @Size, @AccessoryType, @Description, @ImageUrl, GETDATE(), @R, @G, @B)";

                        using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@UserID", userId);
                            cmd.Parameters.AddWithValue("@Name", apparelName);
                            cmd.Parameters.AddWithValue("@Type", apparelType);
                            cmd.Parameters.AddWithValue("@Color", color);
                            cmd.Parameters.AddWithValue("@Size", size);
                            cmd.Parameters.AddWithValue("@AccessoryType", string.IsNullOrEmpty(accessoryType) ? DBNull.Value : (object)accessoryType);
                            cmd.Parameters.AddWithValue("@Description", description);
                            cmd.Parameters.AddWithValue("@ImageUrl", imageUrl);
                            cmd.Parameters.AddWithValue("@R", R);
                            cmd.Parameters.AddWithValue("@G", G);
                            cmd.Parameters.AddWithValue("@B", B);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return "success";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
