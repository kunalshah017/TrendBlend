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
using static TrendBlend.pages.TrendyChat;

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

        private List<ApparelItem> GetUserApparels(string username)
        {
            var apparels = new List<ApparelItem>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT a.ApparelID, a.Name, a.Type, a.Size, a.AccessoryType, 
                           a.Description, a.ImageUrl, a.R, a.G, a.B, a.ColorName 
                    FROM Apparels a
                    INNER JOIN Users u ON a.UserID = u.Id
                    WHERE u.UserName = @Username";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    try
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var apparel = new ApparelItem
                                {
                                    ApparelId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.GetString(2),
                                    Size = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    AccessoryType = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Description = reader.GetString(5),
                                    ImageUrl = reader.GetString(6),
                                    R = reader.GetByte(7),
                                    G = reader.GetByte(8),
                                    B = reader.GetByte(9),
                                    ColorName = reader.IsDBNull(10) ? null : reader.GetString(10)
                                };
                                apparels.Add(apparel);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }

            return apparels;
        }


        private async Task<GeminiResponse> GetGeminiResponse(List<ApparelItem> apparels, string eventType)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] GetGeminiResponse started with {apparels.Count} apparels");

            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                try
                {
                    client.Timeout = TimeSpan.FromMinutes(2);

                    string apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];
                    if (string.IsNullOrEmpty(apiKey))
                    {
                        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR: Gemini API key not configured");
                        throw new InvalidOperationException("Gemini API key not configured");
                    }

                    string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Using API URL: {apiUrl}");

                    var partsList = new List<object>();
                    int imageCount = 0;
                    long totalImageSize = 0;

                    // Add introduction
                    partsList.Add(new
                    {
                        text = $"Act as an AI fashion advisor. The user is attending a {eventType}. " +
                               "Analyze their wardrobe and suggest an appropriate outfit. Consider color coordination and event appropriateness. " +
                               "The response should be in JSON format with an outfit name, event type, description, list of selected items with reasons, and styling tips."
                    });
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Added introduction text");

                    // Add apparel items with images
                    int processedItems = 0;
                    foreach (var apparel in apparels)
                    {
                        try
                        {
                            var imageTimer = System.Diagnostics.Stopwatch.StartNew();
                            byte[] imageBytes = await DownloadImageAsync(apparel.ImageUrl).ConfigureAwait(false);
                            imageTimer.Stop();

                            string base64Image = Convert.ToBase64String(imageBytes);
                            totalImageSize += imageBytes.Length;
                            imageCount++;

                            partsList.Add(new
                            {
                                inlineData = new
                                {
                                    mimeType = "image/jpeg",
                                    data = base64Image
                                }
                            });

                            partsList.Add(new
                            {
                                text = $"Item ID: {apparel.ApparelId}\n" +
                                       $"Name: {apparel.Name}\n" +
                                       $"Type: {apparel.Type}\n" +
                                       $"Size: {apparel.Size ?? "N/A"}\n" +
                                       $"Color: {apparel.ColorDescription}\n" +
                                       $"Description: {apparel.Description}\n\n"
                            });

                            processedItems++;
                            if (processedItems % 5 == 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Processed {processedItems}/{apparels.Count} items. Last image: {imageBytes.Length} bytes in {imageTimer.ElapsedMilliseconds}ms");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error processing image for {apparel.Name}: {ex.Message}");
                            continue;
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Finished processing {imageCount} images, total size: {totalImageSize / 1024:N0}KB");

                    // Add final instructions with example format
                    partsList.Add(new
                    {
                        text = @"Please suggest an outfit using the available items in this JSON format:
{
    ""outfitName"": ""Name of the outfit"",
    ""eventType"": ""The event type"",
    ""description"": ""Overall description of the outfit and how it suits the event"",
    ""items"": [
        {
            ""id"": 123,
            ""type"": ""Top/Bottom/Footwear/Accessory"",
            ""reason"": ""Why this item was chosen and how it coordinates with others""
        }
    ],
    ""stylingTips"": ""Additional styling advice and suggestions""
}"
                    });

                    // Create request payload - using the same structure as in GetApparelDetailsFromAI method
                    var requestBody = new
                    {
                        contents = new[]
                        {
                    new
                    {
                        parts = partsList.ToArray()
                    }
                }
                    };

                    // Send request to Gemini
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Serializing payload to JSON");
                    var jsonTimer = System.Diagnostics.Stopwatch.StartNew();
                    var jsonPayload = JsonConvert.SerializeObject(requestBody);
                    jsonTimer.Stop();
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] JSON serialization took {jsonTimer.ElapsedMilliseconds}ms, size: {jsonPayload.Length / 1024:N0}KB");

                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Set headers (same as in GetApparelDetailsFromAI)
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Sending request to Gemini API");
                    var apiRequestTimer = System.Diagnostics.Stopwatch.StartNew();
                    var response = await client.PostAsync(apiUrl, content).ConfigureAwait(false);
                    apiRequestTimer.Stop();
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Gemini API responded in {apiRequestTimer.ElapsedMilliseconds}ms with status: {response.StatusCode}");

                    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Response content length: {responseContent.Length / 1024:N0}KB");

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR: API failed with status {response.StatusCode}");
                        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Response content: {responseContent}");
                        throw new HttpRequestException($"API returned {response.StatusCode}: {responseContent}");
                    }

                    var deserializeTimer = System.Diagnostics.Stopwatch.StartNew();
                    var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseContent);
                    deserializeTimer.Stop();
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] JSON deserialization took {deserializeTimer.ElapsedMilliseconds}ms");

                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] GetGeminiResponse completed in {sw.ElapsedMilliseconds}ms");

                    return geminiResponse;
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR in GetGeminiResponse: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Stack trace: {ex.StackTrace}");
                    throw;
                }
            }
        }


        // Also fix the DownloadImageAsync method to use ConfigureAwait(false)
        private async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using (var client = new HttpClient())
                {
                    var bytes = await client.GetByteArrayAsync(imageUrl).ConfigureAwait(false); // Add ConfigureAwait(false)
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Downloaded {imageUrl}: {bytes.Length / 1024:N0}KB in {sw.ElapsedMilliseconds}ms");
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR downloading {imageUrl}: {ex.Message} in {sw.ElapsedMilliseconds}ms");
                throw;
            }
        }

        private int GetUserId(string username)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string query = "SELECT Id FROM Users WHERE UserName = @UserName";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }
                    return -1;
                }
            }
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
            string description,
            string colorName
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
                    (UserID, Name, Type, Size, AccessoryType, Description, ImageUrl, CreatedAt, R, G, B, ColorName) 
                    VALUES (@UserID, @Name, @Type, @Size, @AccessoryType, @Description, @ImageUrl, GETDATE(), @R, @G, @B, @ColorName)";

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
                            cmd.Parameters.AddWithValue("@ColorName", colorName);

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
                            new { text = "Analyze this clothing item image, it is image from users wardrobe and has to saved digitally in virtual wardrobe such that it can be found easily and other ai models can understand the data, provide details in the following JSON format: {\"notApparel\":\"[true/false if not a Top/Bottom/Footwear/Accessory]\", \"apparelName\": \"[descriptive name]\", \"apparelType\": \"[Top/Bottom/Footwear/Accessory]\", \"color\": \"[hex color code]\", \"colorName\": \"[name of hexcode color]\", \"description\": \"[detailed description under 1000 characters by the perspective of user uploading]\",\"accessoryType\":\"[If apparelType is accessory then accessoryType as Watch/Cap/Sunglasses/Shades/Jewellery/Necklace/Earings/Rings/Braclets etc]\"}" },
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


        [WebMethod]
        public ApparelItem GetApparelDetails(int apparelId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                SELECT ApparelID, Name, Type, Size, AccessoryType, 
                       Description, ImageUrl, R, G, B, ColorName 
                FROM Apparels 
                WHERE ApparelID = @ApparelId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ApparelId", apparelId);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ApparelItem
                            {
                                ApparelId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Type = reader.GetString(2),
                                Size = reader.IsDBNull(3) ? null : reader.GetString(3),
                                AccessoryType = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Description = reader.GetString(5),
                                ImageUrl = reader.GetString(6),
                                R = reader.GetByte(7),
                                G = reader.GetByte(8),
                                B = reader.GetByte(9),
                                ColorName = reader.IsDBNull(10) ? null : reader.GetString(10)
                            };
                        }
                    }
                }
            }
            return null;
        }

        [WebMethod]
        public GeminiResponse GetOutfitRecommendation(string eventType, string username)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] GetOutfitRecommendation started - Event: {eventType}, User: {username}");

            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR: Username is empty");
                    throw new ArgumentException("Username cannot be empty");
                }

                // Get user apparels - measure time
                var apparelsTimer = System.Diagnostics.Stopwatch.StartNew();
                var apparels = GetUserApparels(username);
                apparelsTimer.Stop();
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Got {apparels.Count} apparels in {apparelsTimer.ElapsedMilliseconds}ms");

                if (!apparels.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] No apparels found for user {username}");
                    throw new Exception($"No apparels found for user {username}");
                }

                // Call Gemini API - measure time
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Starting Gemini API request");
                var geminiTimer = System.Diagnostics.Stopwatch.StartNew();

                var task = GetGeminiResponse(apparels, eventType);

                // Show intermediate progress updates every few seconds
                var progressTimer = System.Threading.Tasks.Task.Run(async () =>
                {
                    int secondsWaited = 0;
                    while (!task.IsCompleted)
                    {
                        await System.Threading.Tasks.Task.Delay(5000); // 5 second updates
                        secondsWaited += 5;
                        System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Still waiting for Gemini response... ({secondsWaited}s elapsed)");
                    }
                });

                var result = task.GetAwaiter().GetResult();
                geminiTimer.Stop();

                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Gemini API response received in {geminiTimer.ElapsedMilliseconds}ms");
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Candidates: {result?.candidates?.Length ?? 0}, Parts: {result?.candidates?[0]?.content?.parts?.Length ?? 0}");

                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] GetOutfitRecommendation completed in {sw.ElapsedMilliseconds}ms");

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR in GetOutfitRecommendation: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        [WebMethod]
        public int CreateBlend(string username, string blendName, string description = null, string wearingSuggestion = null)
        {
            try
            {
                // First get the user ID
                int userId = GetUserId(username);
                if (userId <= 0)
                {
                    throw new Exception("User not found");
                }

                // Sanitize inputs
                blendName = blendName?.Trim() ?? "My Blend";
                description = description?.Trim();
                wearingSuggestion = wearingSuggestion?.Trim();

                // Limit text lengths to prevent DB issues
                if (blendName.Length > 100) blendName = blendName.Substring(0, 100);
                if (description != null && description.Length > 500) description = description.Substring(0, 500);
                if (wearingSuggestion != null && wearingSuggestion.Length > 500) wearingSuggestion = wearingSuggestion.Substring(0, 500);

                // Create the blend
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = @"
                INSERT INTO FavouriteBlend (UserID, Name, Description, WearingSuggestion, CreatedAt) 
                VALUES (@UserID, @Name, @Description, @WearingSuggestion, GETDATE());
                SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Name", blendName);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@WearingSuggestion", (object)wearingSuggestion ?? DBNull.Value);

                        // Get the newly created blend ID
                        decimal result = (decimal)cmd.ExecuteScalar();
                        return (int)result;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateBlend: {ex.Message}\n{ex.StackTrace}");
                throw new Exception($"Failed to create blend: {ex.Message}");
            }
        }

        [WebMethod]
        public bool AddApparelToBlend(int blendId, int apparelId)
        {
            try
            {
                if (blendId <= 0 || apparelId <= 0)
                {
                    throw new ArgumentException("Invalid blend or apparel ID");
                }

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    // First check if this apparel is already in the blend
                    string checkQuery = "SELECT COUNT(*) FROM FavouriteBlendApparels WHERE BlendID = @BlendID AND ApparelID = @ApparelID";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@BlendID", blendId);
                        checkCmd.Parameters.AddWithValue("@ApparelID", apparelId);

                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            // Already in blend, no need to add again
                            return true;
                        }
                    }

                    // Add to blend
                    string insertQuery = "INSERT INTO FavouriteBlendApparels (BlendID, ApparelID) VALUES (@BlendID, @ApparelID)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@BlendID", blendId);
                        cmd.Parameters.AddWithValue("@ApparelID", apparelId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddApparelToBlend: {ex.Message}\n{ex.StackTrace}");
                throw new Exception($"Failed to add apparel to blend: {ex.Message}");
            }
        }
    }
}
