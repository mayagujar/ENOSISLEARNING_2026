using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ENOSISLEARNING
{
    public partial class Questions : System.Web.UI.Page
    {
        string ConnectionString = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCourseDropDown();
            }
        }
        void BindCourseDropDown()
        {
            try
            {
                string query = "select COURSEID,COURSENAME from COURSES_DETAIL";
                SqlDataAdapter sda = new SqlDataAdapter(query, ConnectionString);
                DataTable data = new DataTable();
                sda.Fill(data);
                ddlCourses.DataSource = data;
                ddlCourses.DataTextField = "COURSENAME";
                ddlCourses.DataValueField = "COURSEID";
                ddlCourses.DataBind();
                ListItem selectitem = new ListItem("Select  Course", "Select Course");
                selectitem.Selected = true;
                ddlCourses.Items.Insert(0, selectitem);
            }
            catch (Exception )
            {
                throw;
            }
        }
        //[WebMethod]
        //public static string UploadQuestionsExcel()
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        HttpPostedFile file = httpRequest.Files["file"];
        //        string courseID = httpRequest.Form["courseID"];

        //        if (file == null || file.ContentLength == 0)
        //            return "No file uploaded";

        //        string fileName = Path.GetFileName(file.FileName);
        //        string tempPath = HttpContext.Current.Server.MapPath("~/TempFiles/" + fileName);

        //        // save to temp folder
        //        file.SaveAs(tempPath);

        //        FileInfo fi = new FileInfo(tempPath);

        //        using (var package = new ExcelPackage(fi))
        //        {
        //            var worksheet = package.Workbook.Worksheets[0];
        //            int rowCount = worksheet.Dimension.Rows;

        //            for (int i = 2; i <= rowCount; i++)
        //            {
        //                string questionType = worksheet.Cells[i, 1].Text.Trim();
        //                string difficulty = worksheet.Cells[i, 2].Text.Trim();
        //                string qtext = worksheet.Cells[i, 3].Text.Trim();
        //                string optA = worksheet.Cells[i, 4].Text.Trim();
        //                string optB = worksheet.Cells[i, 5].Text.Trim();
        //                string optC = worksheet.Cells[i, 6].Text.Trim();
        //                string optD = worksheet.Cells[i, 7].Text.Trim();
        //                string optE = worksheet.Cells[i, 8].Text.Trim();
        //                string correct = worksheet.Cells[i, 9].Text.Trim();

        //                InsertQuestion(courseID, questionType, difficulty, qtext,
        //                               optA, optB, optC, optD, optE, correct);
        //            }
        //        }

        //        return "Questions uploaded successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error: " + ex.Message;
        //    }
        //}

        //private static void InsertQuestion(string courseID, string qType, string difficulty,
        //    string qText, string a, string b, string c, string d, string e, string correct)
        //{
        //    using (SqlConnection conn = new SqlConnection(
        //        ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString))
        //    {
        //        conn.Open();

        //        string sql = @"
        //        INSERT INTO Questions
        //        (QuestionType, DifficultyLevel, QuestionText,
        //        OptionA, OptionB, OptionC, OptionD, OptionE,
        //        CorrectAnswer, CourseID)
        //        VALUES
        //        (@type, @diff, @text,
        //         @a, @b, @c, @d, @e,
        //         @correct, @cid)";

        //        using (SqlCommand cmd = new SqlCommand(sql, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@type", qType);
        //            cmd.Parameters.AddWithValue("@diff", difficulty);
        //            cmd.Parameters.AddWithValue("@text", qText);
        //            cmd.Parameters.AddWithValue("@a", a);
        //            cmd.Parameters.AddWithValue("@b", b);
        //            cmd.Parameters.AddWithValue("@c", c);
        //            cmd.Parameters.AddWithValue("@d", d);
        //            cmd.Parameters.AddWithValue("@e", e);
        //            cmd.Parameters.AddWithValue("@correct", correct);
        //            cmd.Parameters.AddWithValue("@cid", courseID);

        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}
    }
}