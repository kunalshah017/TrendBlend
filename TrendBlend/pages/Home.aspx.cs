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
using System.Diagnostics.Eventing.Reader;

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
                }
            }
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

        protected void Upload_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(cs);

            string fpath = Server.MapPath("~/images/");
            string filename = Path.GetFileName(FileUpload1.FileName);
            string extension = Path.GetExtension(filename);

            HttpPostedFile postedFile = FileUpload1.PostedFile;
            int size = postedFile.ContentLength;
            if (FileUpload1.HasFile)
            {
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                {
                    if (size <= 1000000)
                    {
                        FileUpload1.SaveAs(fpath + filename);
                        string path2 = "images/" + filename;

                        string query = "select Id from Users where Username = @Username";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Username", Session["Username"].ToString());
                        int ID = 0; // Declare ID outside the block
                        try
                        {
                            con.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    ID = reader.GetInt32(0);
                                }
                            }
                        }
                        finally
                        {
                            con.Close();
                        }

                        query = "insert into ImageData (ID, Image) values (@ID,@Image)";
                        cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@Image", path2);
                        con.Open();
                        int a = cmd.ExecuteNonQuery();
                        if (a > 0)
                        {
                            Label1.Visible = true;
                            Label1.Text = "Image uploaded successfully";
                        }
                        else
                        {
                            Label1.Visible = true;
                            Label1.Text = "Image not uploaded";
                        }

                    }
                    else
                    {
                        Label1.Visible = true;
                        Label1.Text = "File size should be less than 1MB";
                    }
                }
                else
                {
                    Label1.Visible = true;
                    Label1.Text = "Please upload only jpg, png or jpeg image";
                }
            }
            else
            {
                Label1.Visible = true;
                Label1.Text = "Please select a file";
            }
        }
    }
}