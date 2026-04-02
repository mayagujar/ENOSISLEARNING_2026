using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ENOSISLEARNING
{
    /// <summary>
    /// Summary description for Questions1
    /// </summary>
    public class Questions1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                HttpPostedFile file = context.Request.Files["file"];
                string courseID = context.Request.Form["courseID"];

                if (file == null || file.ContentLength == 0)
                {
                    context.Response.Write("No file uploaded");
                    return;
                }

                using (var package = new ExcelPackage(file.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int totalRows = worksheet.Dimension.Rows;

                    for (int i = 2; i <= totalRows; i++)
                    {
                        string questionType = worksheet.Cells[i, 1].Text.Trim();
                        string difficulty = worksheet.Cells[i, 2].Text.Trim();
                        string qText = worksheet.Cells[i, 3].Text.Trim();
                        string optA = worksheet.Cells[i, 4].Text.Trim();
                        string optB = worksheet.Cells[i, 5].Text.Trim();
                        string optC = worksheet.Cells[i, 6].Text.Trim();
                        string optD = worksheet.Cells[i, 7].Text.Trim();
                        string optE = worksheet.Cells[i, 8].Text.Trim();
                        string correct = worksheet.Cells[i, 9].Text.Trim();

                        InsertQuestion(courseID, questionType, difficulty, qText,
                                       optA, optB, optC, optD, optE, correct);
                    }
                }

                context.Response.Write("Upload successful!");
            }
            catch (Exception ex)
            {
                context.Response.Write("Error: " + ex.Message);
            }
        }

        private void InsertQuestion(string courseID, string qType, string diff,
       string qText, string a, string b, string c, string d, string e, string correct)
        {
            string cs = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();
                string sql = @"
                INSERT INTO Questions
                (QuestionType, DifficultyLevel, QuestionText,
                OptionA, OptionB, OptionC, OptionD, OptionE,
                CorrectAnswer, CourseID)
                VALUES
                (@type, @diff, @text,
                 @a, @b, @c, @d, @e,
                 @correct, @cid)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@type", qType);
                    cmd.Parameters.AddWithValue("@diff", diff);
                    cmd.Parameters.AddWithValue("@text", qText);
                    cmd.Parameters.AddWithValue("@a", a);
                    cmd.Parameters.AddWithValue("@b", b);
                    cmd.Parameters.AddWithValue("@c", c);
                    cmd.Parameters.AddWithValue("@d", d);
                    cmd.Parameters.AddWithValue("@e", e);
                    cmd.Parameters.AddWithValue("@correct", correct);
                    cmd.Parameters.AddWithValue("@cid", courseID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}