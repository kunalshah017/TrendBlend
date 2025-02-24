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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserName"] == null)
                {
                    Response.Redirect("~/pages/SignIn.aspx");
                    return;
                }

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
