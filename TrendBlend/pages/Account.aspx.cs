using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace TrendBlend.pages
{
    public partial class WebForm2 : System.Web.UI.Page
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

                LoadUserInfo();
                LoadApparelStats();
            }
        }

        private void LoadUserInfo()
        {
            FirstNameLabel.Text = Session["FirstName"].ToString();
            UserNameLabel.Text = Session["UserName"].ToString();
            ProfileImage.ImageUrl = $"https://api.dicebear.com/9.x/dylan/svg?seed={Session["UserName"].ToString()}&facialHairProbability=0&backgroundColor=7573fc,a695f9,cec2ff";
            Page.Title = $"TrendBlend | {Session["UserName"].ToString()}";
        }

        private void LoadApparelStats()
        {
            string username = Session["UserName"].ToString();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT 
                        Type,
                        COUNT(*) as Count
                    FROM Apparels a
                    INNER JOIN Users u ON a.UserID = u.Id
                    WHERE u.UserName = @Username
                    GROUP BY Type";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    int tops = 0, bottoms = 0, footwear = 0;

                    while (reader.Read())
                    {
                        string type = reader["Type"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);

                        switch (type.ToLower())
                        {
                            case "top":
                                tops = count;
                                break;
                            case "bottom":
                                bottoms = count;
                                break;
                            case "footwear":
                                footwear = count;
                                break;
                        }
                    }

                    TopsCountLabel.Text = tops.ToString();
                    BottomsCountLabel.Text = bottoms.ToString();
                    FootwearCountLabel.Text = footwear.ToString();
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
