using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;

namespace TrendBlend.pages
{
    public partial class Search : System.Web.UI.Page
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

        public string GetColorName(byte r, byte g, byte b)
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check for type parameter in query string
                string apparelType = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(apparelType))
                {
                    // Set the dropdown value
                    TypeDropDown.SelectedValue = apparelType;
                }
            }

            SearchButton_Click(null, null);
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string searchText = SearchText.Text.Trim();
            string apparelType = TypeDropDown.SelectedValue;
            string colorHex = ColorInput.Value;
            bool includeColor = colorHex != "null";

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
            SELECT a.*, 
                   CASE 
                       WHEN @IncludeColor = 1 THEN
                           CAST(SQRT(
                               POWER(CAST(a.R AS FLOAT) - CAST(@R AS FLOAT), 2) + 
                               POWER(CAST(a.G AS FLOAT) - CAST(@G AS FLOAT), 2) + 
                               POWER(CAST(a.B AS FLOAT) - CAST(@B AS FLOAT), 2)
                           ) AS FLOAT)
                       ELSE 0.0 
                   END as ColorDistance
            FROM Apparels a
            INNER JOIN Users u ON a.UserID = u.Id
            WHERE u.UserName = @Username
            AND (@SearchText = '' OR a.Name LIKE '%' + @SearchText + '%')
            AND (@ApparelType = '' OR a.Type = @ApparelType)
            ORDER BY 
                CASE 
                    WHEN @IncludeColor = 1 THEN
                        CAST(SQRT(
                            POWER(CAST(a.R AS FLOAT) - CAST(@R AS FLOAT), 2) + 
                            POWER(CAST(a.G AS FLOAT) - CAST(@G AS FLOAT), 2) + 
                            POWER(CAST(a.B AS FLOAT) - CAST(@B AS FLOAT), 2)
                        ) AS FLOAT)
                    ELSE 0.0 
                END ASC,
                a.CreatedAt DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", Session["UserName"].ToString());
                cmd.Parameters.AddWithValue("@SearchText", searchText);
                cmd.Parameters.AddWithValue("@ApparelType", apparelType);
                cmd.Parameters.AddWithValue("@IncludeColor", includeColor);

                if (includeColor)
                {
                    Color color = ColorTranslator.FromHtml(colorHex);
                    cmd.Parameters.AddWithValue("@R", (float)color.R);
                    cmd.Parameters.AddWithValue("@G", (float)color.G);
                    cmd.Parameters.AddWithValue("@B", (float)color.B);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@R", 0f);
                    cmd.Parameters.AddWithValue("@G", 0f);
                    cmd.Parameters.AddWithValue("@B", 0f);
                }

                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    SearchResults.DataSource = dt;
                    SearchResults.DataBind();
                }
                catch (Exception ex)
                {
                    // Log error appropriately
                    Response.Write("Error: " + ex.Message);
                }
            }
        }


    }
}
