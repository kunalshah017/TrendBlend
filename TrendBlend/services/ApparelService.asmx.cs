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
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        [WebMethod]
        public async Task<string> GetApparelDetailsFromAI(string imageData)
        {

            // Create a static HttpClient with proper settings
            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(2); // Increase timeout to 2 minutes

                    string API_KEY = ConfigurationManager.AppSettings["GeminiApiKey"];
                    string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

                    // Check if the image data is in correct format
                    if (!imageData.StartsWith("data:image"))
                    {
                        throw new ArgumentException("Image data is not in valid base64 format");
                    }

                    // Extract the base64 part correctly
                    string base64Data = imageData.Contains(",") ?
                        imageData.Substring(imageData.IndexOf(",") + 1) :
                        imageData;

                    // Simplified request body structure
                    var requestBody = new
                    {
                        contents = new[]
                        {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = "Analyze this clothing item image, it is image from users wardrobe and has to saved digitally in virtual wardrobe such that it can be found easily and other ai models can understand the data, provide details in the following JSON format: {\"notApparel\":\"[true/false if not a Top/Bottom/Footwear/Accessory]\", \"apparelName\": \"[descriptive name]\", \"apparelType\": \"[Top/Bottom/Footwear/Accessory]\", \"color\": \"[hex color code]\", \"description\": \"[detailed description under 1000 characters by the perspective of user uploading]\",\"accessoryType\":\"[If apparelType is accessory then accessoryType as Watch/Cap/Sunglasses/Shades/Jewellery/Necklace/Earings/Rings/Braclets etc]\"}" },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/jpeg",
                                    data = base64Data
                                }
                            }
                        }
                    }
                }
                    };

                    string jsonRequest = JsonConvert.SerializeObject(requestBody);


                    // Set up request with proper headers
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    // Set request headers directly
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                    // IMPORTANT: Add ConfigureAwait(false) to avoid deadlocks in ASP.NET
                    var response = await client.PostAsync($"{API_URL}?key={API_KEY}", content).ConfigureAwait(false);

                    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"API returned {response.StatusCode}: {responseContent}");
                    }


                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

                    // Make sure we're accessing the correct properties
                    if (jsonResponse.candidates == null || jsonResponse.candidates.Count == 0)
                    {
                        throw new Exception("API response doesn't contain expected 'candidates' array");
                    }

                    var result = jsonResponse.candidates[0].content.parts[0].text.ToString();

                    return result;
                }
                catch (TaskCanceledException)
                {
                    return JsonConvert.SerializeObject(new { error = "Request to Gemini API timed out. Please try again." });
                }
                catch (Exception ex)
                {
                    return JsonConvert.SerializeObject(new { error = ex.Message });
                }
            }
        }
    }
}
