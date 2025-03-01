using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace TrendBlend.pages
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["TrendBlendDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check for authentication
                if (Session["FirstName"] != null)
                {
                    // User is in session
                    userNameLabel.Text = Session["FirstName"].ToString();
                    LoadApparels(Session["UserName"].ToString());
                }
                else
                {
                    Response.Redirect("~/pages/Onboarding.aspx");
                }
            }
        }

        private void LoadApparels(string username)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Apparels WHERE UserID = (SELECT Id FROM Users WHERE UserName = @Username)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Apparel> apparels = new List<Apparel>();

                    while (reader.Read())
                    {
                        apparels.Add(new Apparel
                        {
                            Id = Convert.ToInt32(reader["ApparelId"]),
                            Name = reader["Name"].ToString(),
                            Type = reader["Type"].ToString(),
                            Size = reader["Size"].ToString(),
                            AccessoryType = reader["AccessoryType"].ToString(),
                            Description = reader["Description"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString()
                        });
                    }

                    // Bind the apparels to the UI
                    BindApparels(apparels);
                }
                catch (Exception ex)
                {
                    // Handle exception
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void BindApparels(List<Apparel> apparels)
        {
            // Gets filtered lists
            var tops = apparels.Where(a => a.Type == "Top").ToList();
            var bottoms = apparels.Where(a => a.Type == "Bottom").ToList();
            var footwears = apparels.Where(a => a.Type == "Footwear").ToList();
            var accessories = apparels.Where(a => a.Type == "Accessory").ToList();

            // Show/hide and bind tops
            TopsPanel.Visible = tops.Any();
            if (tops.Any()) BindSlider(tops, topsSlider);

            // Show/hide and bind bottoms
            BottomsPanel.Visible = bottoms.Any();
            if (bottoms.Any()) BindSlider(bottoms, bottomsSlider);

            // Show/hide and bind footwears
            FootwearsPanel.Visible = footwears.Any();
            if (footwears.Any()) BindSlider(footwears, footwearsSlider);

            // Show/hide and bind accessories
            AccessoriesPanel.Visible = accessories.Any();
            if (accessories.Any()) BindSlider(accessories, accessoriesSlider);

            // Show "No apparels" message if no apparels exist
            NoApparelsPanel.Visible = !apparels.Any();
        }

        private void BindSlider(List<Apparel> apparels, Repeater repeater)
        {
            repeater.DataSource = apparels;
            repeater.DataBind();
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            // Clear cookie if exists
            if (Request.Cookies["UserInfo"] != null)
            {
                HttpCookie userCookie = new HttpCookie("UserInfo");
                userCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(userCookie);
            }

            // Clear session
            Session.Clear();

            // Redirect to login
            Response.Redirect("~/pages/SignIn.aspx");
        }
    }

    public class Apparel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string AccessoryType { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
