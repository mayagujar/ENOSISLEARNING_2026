using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ENOSISLEARNING
{
    public partial class FinalBatchSheet : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString.ToString());
        string ConnectionString = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCourseDDL();
            }
        }
        //Bind Dropdown// 
        void BindCourseDDL()
        {
            try
            {
                string Query = "SELECT COURSEID, COURSENAME FROM COURSES_DETAIL WHERE STATUS='A'";

                using (SqlConnection Connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(Query, Connection))
                    {
                        Connection.Open();
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable data = new DataTable();
                        sda.Fill(data);
                        drpCourses.DataSource = data;
                        drpCourses.DataTextField = "COURSENAME";
                        drpCourses.DataValueField = "COURSEID";
                        drpCourses.DataBind();
                    }
                }
                System.Web.UI.WebControls.ListItem selectitem = new System.Web.UI.WebControls.ListItem("Select Course", "");
                drpCourses.Items.Insert(0, selectitem);
            }
            catch (SqlException ex)
            {
               
            }
            catch (Exception ex)
            {

            }
        }

        public class ChapterModel
        {
            public int Chapter_Id { get; set; }
            public string Chapter_Number { get; set; }
        }


        [WebMethod]
        public static List<ChapterModel> ViewChapterGridView(int Course_Id)
        {
            List<ChapterModel> list = new List<ChapterModel>();

            string cs = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT Chapter_Id, Chapter_Number FROM enosis.COURSE_CHAPTER_DETAIL WHERE Course_Id = @Course_Id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Course_Id", Course_Id);
                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new ChapterModel
                        {
                            Chapter_Id = Convert.ToInt32(dr["Chapter_Id"]),
                            Chapter_Number = dr["Chapter_Number"].ToString()
                        });
                    }
                }
            }

            return list;
        }
        //Add Chapter Button Click//
        [WebMethod]
        public static string AddChapter(int Course_Id, string Chapter_Name)
        {
            string connStr = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "INSERT INTO enosis.COURSE_CHAPTER_DETAIL (Chapter_Number, Course_Id) VALUES (@Chapter_Name,@Course_Id)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Course_Id", Course_Id);
                cmd.Parameters.AddWithValue("@Chapter_Name", Chapter_Name);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return "Success";
        }
        //Edit Chapter Button Click//
        [WebMethod]
        public static string UpdateChapter(int Chapter_Id, string Chapter_Name)
        {
            string connStr = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "UPDATE enosis.COURSE_CHAPTER_DETAIL SET Chapter_Number=@Chapter_Name WHERE Chapter_Id=@Chapter_Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Chapter_Name", Chapter_Name);
                cmd.Parameters.AddWithValue("@Chapter_Id", Chapter_Id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return "Success";
        }


        [WebMethod]
        public static string DeleteChapter(int Chapter_Id)
        {
            string connStr = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "DELETE FROM enosis.COURSE_CHAPTER_DETAIL WHERE Chapter_Id=@Chapter_Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Chapter_Id", Chapter_Id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return "Success";
        }
        //Get Topic GridView Data//
        [WebMethod]
        public static List<object> GetTopicsByChapterId(int Chapter_Id)
        {
            List<object> topics = new List<object>();

            using (SqlConnection con = new SqlConnection(
                ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString))
            {
                string query = @"SELECT Topic_Id, Topic_Name, Days 
                         FROM enosis.COURSE_TOPIC_DETAIL 
                         WHERE Chapter_Id = @Chapter_Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Chapter_Id", Chapter_Id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    topics.Add(new
                    {
                        Topic_Id = dr["Topic_Id"],
                        Topic_Name = dr["Topic_Name"],
                        Days = dr["Days"]
                    });
                }

                con.Close();
            }

            return topics;
        }
        //Add Topic Button Click//
        [WebMethod]
        public static string AddTopic(int Chapter_Id, string Topic_Name, int Days)
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = @"INSERT INTO enosis.COURSE_TOPIC_DETAIL 
                             (Topic_Name, Days, Chapter_Id) 
                             VALUES (@Topic_Name, @Days, @Chapter_Id)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Topic_Name", Topic_Name);
                        cmd.Parameters.AddWithValue("@Days", Days);
                        cmd.Parameters.AddWithValue("@Chapter_Id", Chapter_Id);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        //Edit Topic Button Click//
        [WebMethod]
        public static string UpdateTopic(int Topic_Id, string Topic_Name, int Days)
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = @"UPDATE enosis.COURSE_TOPIC_DETAIL 
                             SET Topic_Name=@Topic_Name, Days=@Days 
                             WHERE Topic_Id=@Topic_Id";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Topic_Id", Topic_Id);
                        cmd.Parameters.AddWithValue("@Topic_Name", Topic_Name);
                        cmd.Parameters.AddWithValue("@Days", Days);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        //Delete Topic Button Click//
        [System.Web.Services.WebMethod]
        public static string DeleteTopic(int Topic_Id)
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = "DELETE FROM enosis.COURSE_TOPIC_DETAIL WHERE Topic_Id=@Topic_Id";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Topic_Id", Topic_Id);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}