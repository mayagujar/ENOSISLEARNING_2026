using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ENOSISLEARNING
{
    public partial class Quiz : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCourses();
            }
        }


        void BindCourses()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COURSEID, COURSENAME FROM COURSES_DETAIL", conn);
                ddlQuizCourses.DataSource = cmd.ExecuteReader();
                ddlQuizCourses.DataTextField = "COURSENAME";
                ddlQuizCourses.DataValueField = "COURSEID";
                ddlQuizCourses.DataBind();
                ddlQuizCourses.Items.Insert(0, new ListItem("--Select--", ""));
            }
        }

        public class QuizQuestion
        {
            public int QuestionID { get; set; }
            public string QuestionText { get; set; }
            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }
            public string OptionE { get; set; }
            public string CorrectAnswer { get; set; }
        }

        [WebMethod]
        public static List<QuizQuestion> GetQuizQuestions(int courseID, int diffMode)
        {
            List<QuizQuestion> list = new List<QuizQuestion>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString))
            {
                con.Open();
                string sql = "";

                if (diffMode == 1)
                    sql = "SELECT * FROM Questions WHERE CourseID=@cid AND DifficultyLevel='Easy' ORDER BY NEWID()";
                else if (diffMode == 2)
                    sql = "SELECT * FROM Questions WHERE CourseID=@cid AND DifficultyLevel='Medium' ORDER BY NEWID()";
                else if (diffMode == 3)
                    sql = "SELECT * FROM Questions WHERE CourseID=@cid AND DifficultyLevel='High' ORDER BY NEWID()";
                else if (diffMode == 4)
                    sql = @"
              SELECT TOP 10 * FROM Questions WHERE CourseID=@cid AND DifficultyLevel='Easy' ORDER BY NEWID()
              UNION ALL
              SELECT TOP 10 * FROM Questions WHERE CourseID=@cid AND DifficultyLevel='Medium' ORDER BY NEWID()
              UNION ALL
              SELECT TOP 10 * FROM Questions WHERE CourseID=@cid AND DifficultyLevel='High' ORDER BY NEWID()";
                else
                    sql = "SELECT * FROM Questions WHERE CourseID=@cid ORDER BY NEWID()";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@cid", courseID);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new QuizQuestion
                    {
                        QuestionID = (int)dr["QuestionID"],
                        QuestionText = dr["QuestionText"].ToString(),
                        OptionA = dr["OptionA"].ToString(),
                        OptionB = dr["OptionB"].ToString(),
                        OptionC = dr["OptionC"].ToString(),
                        OptionD = dr["OptionD"].ToString(),
                        OptionE = dr["OptionE"].ToString(),
                        CorrectAnswer = dr["CorrectAnswer"].ToString()
                    });
                }
            }
            return list;
        }

    }
}