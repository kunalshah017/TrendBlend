using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Web.UI.WebControls;

namespace TrendBlend.pages
{
    public partial class Blend : System.Web.UI.Page
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string blendId = Request.QueryString["id"];
                System.Diagnostics.Debug.WriteLine($"QueryString ID: {blendId}");

                if (string.IsNullOrEmpty(blendId))
                {
                    System.Diagnostics.Debug.WriteLine("BlendId is null or empty");
                    ShowError();
                    return;
                }

                LoadBlendDetails(blendId);
            }
        }

        private void LoadBlendDetails(string blendId)
        {
            // Convert blendId to integer
            if (!int.TryParse(blendId, out int blendIdInt))
            {
                System.Diagnostics.Debug.WriteLine($"Failed to parse BlendId: {blendId}");
                ShowError();
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Parsed BlendId: {blendIdInt}");
            System.Diagnostics.Debug.WriteLine($"Current User: {Session["UserName"]}");

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT b.*, u.UserName 
                    FROM FavouriteBlend b
                    INNER JOIN Users u ON b.UserID = u.Id
                    WHERE b.BlendID = @BlendId 
                    AND u.UserName = @Username";

                System.Diagnostics.Debug.WriteLine($"Query: {query}");

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BlendId", blendIdInt);
                cmd.Parameters.AddWithValue("@Username", Session["UserName"].ToString());

                try
                {
                    con.Open();
                    System.Diagnostics.Debug.WriteLine("DB Connection opened");

                    // First, let's check if the blend exists at all
                    var checkQuery = "SELECT COUNT(*) FROM FavouriteBlend WHERE BlendID = @BlendId";
                    using (var checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@BlendId", blendIdInt);
                        int count = (int)checkCmd.ExecuteScalar();
                        System.Diagnostics.Debug.WriteLine($"Blend exists check: {count > 0}");
                    }

                    SqlDataReader reader = cmd.ExecuteReader();
                    System.Diagnostics.Debug.WriteLine("Executed reader");

                    if (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("Found blend data");
                        BlendPanel.Visible = true;
                        ErrorPanel.Visible = false;

                        BlendName.Text = reader["Name"].ToString();
                        DateTime createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                        BlendDate.Text = createdAt.ToString("MMM dd, yyyy");

                        System.Diagnostics.Debug.WriteLine($"Blend Name: {BlendName.Text}");
                        System.Diagnostics.Debug.WriteLine($"Created At: {BlendDate.Text}");

                        reader.Close();

                        // Load all apparels in the blend
                        LoadApparels(blendIdInt, con);
                        Page.Title = $"TrendBlend | {BlendName.Text}";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No blend data found");
                        ShowError();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading blend details: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    ShowError();
                }
            }
        }

        private void LoadApparels(int blendId, SqlConnection con)
        {
            try
            {
                using (SqlConnection newCon = new SqlConnection(cs))
                {
                    string query = @"
                SELECT 
                    a.*,
                    CASE 
                        WHEN a.ColorName IS NULL OR a.ColorName = '' 
                        THEN NULL 
                        ELSE a.ColorName 
                    END as ColorName,
                    CASE a.Type 
                        WHEN 'Top' THEN 1
                        WHEN 'Bottom' THEN 2
                        WHEN 'Footwear' THEN 3
                        WHEN 'Accessory' THEN 4
                        ELSE 5
                    END as TypeOrder
                FROM Apparels a
                INNER JOIN FavouriteBlendApparels ba ON a.ApparelID = ba.ApparelID
                WHERE ba.BlendID = @BlendId
                ORDER BY TypeOrder, a.CreatedAt DESC";

                    System.Diagnostics.Debug.WriteLine($"Loading apparels for BlendID: {blendId}");

                    newCon.Open();
                    SqlCommand cmd = new SqlCommand(query, newCon);
                    cmd.Parameters.AddWithValue("@BlendId", blendId);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    System.Diagnostics.Debug.WriteLine($"Found {dt.Rows.Count} total apparels");

                    // Filter and bind data by type
                    DataView dv = new DataView(dt);

                    // Images slider - use the original ordered DataTable
                    ImagesRepeater.DataSource = dt;
                    ImagesRepeater.DataBind();

                    // Tops
                    dv.RowFilter = "Type = 'Top'";
                    TopsRepeater.DataSource = dv.ToTable();
                    TopsRepeater.DataBind();
                    TopsPanel.Visible = dv.Count > 0;
                    System.Diagnostics.Debug.WriteLine($"Tops count: {dv.Count}");

                    // Bottoms
                    dv.RowFilter = "Type = 'Bottom'";
                    BottomsRepeater.DataSource = dv.ToTable();
                    BottomsRepeater.DataBind();
                    BottomsPanel.Visible = dv.Count > 0;
                    System.Diagnostics.Debug.WriteLine($"Bottoms count: {dv.Count}");

                    // Footwear
                    dv.RowFilter = "Type = 'Footwear'";
                    FootwearRepeater.DataSource = dv.ToTable();
                    FootwearRepeater.DataBind();
                    FootwearPanel.Visible = dv.Count > 0;
                    System.Diagnostics.Debug.WriteLine($"Footwear count: {dv.Count}");

                    // Accessories
                    dv.RowFilter = "Type = 'Accessory'";
                    AccessoriesRepeater.DataSource = dv.ToTable();
                    AccessoriesRepeater.DataBind();
                    AccessoriesPanel.Visible = dv.Count > 0;
                    System.Diagnostics.Debug.WriteLine($"Accessories count: {dv.Count}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadApparels: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }


        protected string GetColorName(byte r, byte g, byte b)
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

        private void ShowError()
        {
            BlendPanel.Visible = false;
            ErrorPanel.Visible = true;
        }

        protected void ConfirmDeleteButton_Click(object sender, EventArgs e)
        {
            string blendId = Request.QueryString["id"];
            if (string.IsNullOrEmpty(blendId))
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    bool transactionCompleted = false;
                    try
                    {
                        // First delete all apparel associations
                        string deleteApparelsQuery = @"
                DELETE FROM FavouriteBlendApparels 
                WHERE BlendID = @BlendId";

                        using (SqlCommand cmd = new SqlCommand(deleteApparelsQuery, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BlendId", blendId);
                            cmd.ExecuteNonQuery();
                        }

                        // Then delete the blend itself
                        string deleteBlendQuery = @"
                DELETE FROM FavouriteBlend 
                WHERE BlendID = @BlendId 
                AND UserID = (SELECT Id FROM Users WHERE UserName = @Username)";

                        using (SqlCommand cmd = new SqlCommand(deleteBlendQuery, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BlendId", blendId);
                            cmd.Parameters.AddWithValue("@Username", Session["UserName"].ToString());

                            int result = cmd.ExecuteNonQuery();
                            if (result > 0)
                            {
                                transaction.Commit();
                                transactionCompleted = true;
                                Response.Redirect("~/pages/Home.aspx");
                            }
                            else
                            {
                                ShowError();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error deleting blend: {ex.Message}");
                        if (!transactionCompleted)
                        {
                            transaction.Rollback();
                        }
                        ShowError();
                    }
                }
            }
        }

        protected void RemoveButton_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string apparelId = btn.CommandArgument;
            string blendId = Request.QueryString["id"];

            if (string.IsNullOrEmpty(apparelId) || string.IsNullOrEmpty(blendId))
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
            DELETE FROM FavouriteBlendApparels 
            WHERE BlendID = @BlendId 
            AND ApparelID = @ApparelID
            AND BlendID IN (
                SELECT BlendID 
                FROM FavouriteBlend 
                WHERE UserID = (SELECT Id FROM Users WHERE UserName = @Username)
            )";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BlendId", blendId);
                cmd.Parameters.AddWithValue("@ApparelID", apparelId);
                cmd.Parameters.AddWithValue("@Username", Session["UserName"].ToString());

                try
                {
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        // Refresh the page to show updated blend
                        LoadBlendDetails(blendId);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error removing apparel from blend: {ex.Message}");
                }
            }
        }
    }
}
