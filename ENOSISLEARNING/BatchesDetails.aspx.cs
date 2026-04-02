using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ENOSISLEARNING.StudentView;

namespace ENOSISLEARNING
{
    //27/11/2025   BatchesDetails
    public partial class BatchesDetails : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString.ToString());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetCourses();
                GetCoordinator();
            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetCompleteBatches()
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

                using (SqlConnection Conn = new SqlConnection(constr))
                {
                    string query = @"
                SELECT 
                    B.BATCHID,
                    B.BATCHMODE,
                    B.BATCHSUBJECT,
                    B.FACULTYNAME,
                    ISNULL(BSD.TOTALDAYS, 0) AS TOTALDAYS
                FROM Enosis.BATCHESINFO B
                LEFT JOIN 
                (
                    SELECT 
                        BATCHID,
                        COUNT([DATE]) AS TOTALDAYS
                    FROM Enosis.BATCHSHEETDETAILS
                    GROUP BY BATCHID
                ) BSD 
                    ON B.BATCHID = BSD.BATCHID
                WHERE 
                    B.STATUS IN ('Completed', 'Complete')";

                    using (SqlCommand cmd = new SqlCommand(query, Conn))
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

                        foreach (DataRow dr in dt.Rows)
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();

                            foreach (DataColumn col in dt.Columns)
                            {
                                row[col.ColumnName] = dr[col];
                            }

                            rows.Add(row);
                        }

                        return new { data = rows };
                    }
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    error = ex.Message,
                    stack = ex.StackTrace
                };
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object GetBatches()
        {
            string constr = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"
            SELECT  
                BI.BATCHID,
                BI.BATCHTYPE,
                BI.BATCHMODE,
                BI.BATCHSUBJECT,
                BI.STARTDATE,
                BI.ENDDATE,
                BI.STARTTIME,
                BI.ENDTIME,
                BI.FACULTYNAME,
                BI.EXPECTEDSTUDENT,
                BI.STATUS,
                BI.USERID,
                BI.COURSEID,

                ISNULL(COMPLETED.COMPLETEDDAYS, 0) AS COMPLETEDDAYS,

                DATEDIFF(DAY, BI.STARTDATE, BI.ENDDATE) + 1 AS TOTALDAYS,

                CAST(
                    ROUND(
                        (ISNULL(COMPLETED.COMPLETEDDAYS, 0) * 100.0) /
                        NULLIF((DATEDIFF(DAY, BI.STARTDATE, BI.ENDDATE) + 1), 0),
                    0)
                AS INT) AS PROGRESSPERCENT

            FROM ENOSIS.BATCHESINFO AS BI

            LEFT JOIN (
                SELECT 
                    BATCHID,
                    COUNT([DATE]) AS COMPLETEDDAYS
                FROM ENOSIS.BATCHSHEETDETAILS
                GROUP BY BATCHID
            ) AS COMPLETED
                ON BI.BATCHID = COMPLETED.BATCHID

            LEFT JOIN COURSES_DETAIL AS CD
                ON BI.COURSEID = CD.COURSEID

            WHERE BI.STATUS NOT IN ('Completed', 'Complete')";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

                        foreach (DataRow dr in dt.Rows)
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();

                            foreach (DataColumn col in dt.Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }

                            rows.Add(row);
                        }

                        return new { data = rows };
                    }
                }
            }
        }

        public class Batch
        {
            public int BATCHID { get; set; }

            public string BATCHTYPE { get; set; }

            public string BATCHMODE { get; set; }

            public string BATCHSUBJECT { get; set; }

            public DateTime STARTDATE { get; set; }

            public DateTime ENDDATE { get; set; }

            public DateTime STARTTIME { get; set; }

            public DateTime ENDTIME { get; set; }

            public string FACULTYNAME { get; set; }

            public string EXPECTEDSTUDENT { get; set; }

            public string STATUS { get; set; }

            public int USERID { get; set; }

            public int COURSEID { get; set; }

            public int LASTCHAPTERNUMBER { get; set; }
        }

        public class Candidate
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
        void GetCourses()
        {
            string con = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString.ToString();
            try
            {
                string query = "select * from COURSES_DETAIL WHERE STATUS = 'A'";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataTable data = new DataTable();
                sda.Fill(data);
                drpDCourse.DataSource = data;
                drpDCourse.DataTextField = "COURSENAME";
                drpDCourse.DataValueField = "COURSEID";
                drpDCourse.DataBind();
            }
            catch (Exception ex)
            {

                //lblmsg.Text = ex.Message;
            }
        }
        //Bind Coordinator DropDown//
        void GetCoordinator()
        {
            string con = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString.ToString();
            try
            {
                string query = "SELECT USERID, FULLNAME FROM USERDETAILS WHERE (STATUS = 'ACTIVE' OR STATUS = '1') AND FULLNAME IS NOT NULL ORDER BY FULLNAME ASC";
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataTable data = new DataTable();
                sda.Fill(data);
                drpFaculty.DataSource = data;
                drpFaculty.DataTextField = "FULLNAME";
                drpFaculty.DataValueField = "USERID";
                drpFaculty.DataBind();
                FilterFacultyDrp.DataSource = data;
                FilterFacultyDrp.DataTextField = "FULLNAME";
                FilterFacultyDrp.DataValueField = "FULLNAME";
                FilterFacultyDrp.DataBind();
            }
            catch (Exception ex)
            {

                //lblmsg.Text = ex.Message;
            }
        }
        [WebMethod]
        public static List<Candidate> GetCandidates(string keyword)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT CANDIDATE_CODE, FULLNAME FROM CANDIDATES WHERE FULLNAME LIKE @kw + '%' ORDER BY CANDIDATE_CODE DESC", con);
                cmd.Parameters.AddWithValue("@kw", keyword);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<Candidate> candidates = new List<Candidate>();
                while (reader.Read())
                {
                    candidates.Add(new Candidate
                    {
                        ID = reader["CANDIDATE_CODE"].ToString(),
                        Name = reader["FULLNAME"].ToString()
                    });
                }

                return candidates;
            }
        }
        //Add Batch
        [WebMethod]
        public static string AddBatch(
     string BATCHTYPE,
     string BATCHMODE,
     string BATCHSUBJECT,
     string STARTDATE,
     string ENDDATE,
     string STARTTIME,
     string ENDTIME,
     string FACULTYNAME,
     string EXPECTEDSTUDENT,
     string STATUS,
     int USERID,
     int COURSEID,
     string SELECTEDIDS,
     string SELECTEDNAMES   
 )
        {
            try
            {
                using (SqlConnection con = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString))
                {
                    con.Open();

                    // 🔄 Convert date/time
                    DateTime SD = Convert.ToDateTime(STARTDATE);
                    DateTime ED = Convert.ToDateTime(ENDDATE);
                    DateTime ST = Convert.ToDateTime(STARTTIME);
                    DateTime ET = Convert.ToDateTime(ENDTIME);

                    // 🧾 INSERT INTO BATCHESINFO
                    SqlCommand cmd = new SqlCommand(@"
                INSERT INTO ENOSIS.BATCHESINFO
                (
                    BATCHTYPE,
                    BATCHMODE,
                    BATCHSUBJECT,
                    STARTDATE,
                    ENDDATE,
                    STARTTIME,
                    ENDTIME,
                    FACULTYNAME,
                    EXPECTEDSTUDENT,
                    STATUS,
                    CREATEDBY,
                    USERID,
                    COURSEID
                )
                VALUES
                (
                    @BATCHTYPE,
                    @BATCHMODE,
                    @BATCHSUBJECT,
                    @STARTDATE,
                    @ENDDATE,
                    @STARTTIME,
                    @ENDTIME,
                    @FACULTYNAME,
                    @EXPECTEDSTUDENT,
                    @STATUS,
                    @CREATEDBY,
                    @USERID,
                    @COURSEID
                );
                SELECT SCOPE_IDENTITY();", con);

                    // 🔧 Parameters
                    cmd.Parameters.AddWithValue("@BATCHTYPE", BATCHTYPE);
                    cmd.Parameters.AddWithValue("@BATCHMODE", BATCHMODE);
                    cmd.Parameters.AddWithValue("@BATCHSUBJECT", BATCHSUBJECT);
                    cmd.Parameters.AddWithValue("@STARTDATE", SD);
                    cmd.Parameters.AddWithValue("@ENDDATE", ED);
                    cmd.Parameters.AddWithValue("@STARTTIME", ST);
                    cmd.Parameters.AddWithValue("@ENDTIME", ET);
                    cmd.Parameters.AddWithValue("@FACULTYNAME", FACULTYNAME);
                    cmd.Parameters.AddWithValue("@EXPECTEDSTUDENT", EXPECTEDSTUDENT);
                    cmd.Parameters.AddWithValue("@STATUS", STATUS);

                    // 👤 Logged-in user
                    string CREATEDBY = HttpContext.Current.Session["USERNAME"]?.ToString() ?? "SYSTEM";
                    cmd.Parameters.AddWithValue("@CREATEDBY", CREATEDBY);

                    cmd.Parameters.AddWithValue("@USERID", USERID);
                    cmd.Parameters.AddWithValue("@COURSEID", COURSEID);

                    // 🔑 Get generated BATCHID
                    int BATCHID = Convert.ToInt32(cmd.ExecuteScalar());

                    // 🔗 Insert Candidate ↔ Batch mapping
                    if (!string.IsNullOrWhiteSpace(SELECTEDIDS))
                    {
                        string[] studentIdList = SELECTEDIDS.Split(',');

                        foreach (string sid in studentIdList)
                        {
                            if (string.IsNullOrWhiteSpace(sid)) continue;

                            SqlCommand cmdStudent = new SqlCommand(@"
                        INSERT INTO CANDIDATEBATCHMAPPING
                        (CANDIDATE_CODE, COURSEID, BATCHID)
                        VALUES
                        (@CANDIDATE_CODE, @COURSEID, @BATCHID)", con);

                            cmdStudent.Parameters.AddWithValue("@CANDIDATE_CODE", sid.Trim());
                            cmdStudent.Parameters.AddWithValue("@COURSEID", COURSEID);
                            cmdStudent.Parameters.AddWithValue("@BATCHID", BATCHID);

                            try
                            {
                                cmdStudent.ExecuteNonQuery();
                            }
                            catch
                            {
                                // duplicate mapping ignore
                            }
                        }
                    }

                    return "success";
                }
            }
            catch (Exception ex)
            {
                return "error: " + ex.Message;
            }
        }

        //Edit
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Batch GetBatchById(int batchId)
        {
            Batch batch = null;
            string connStr = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString;

            int SafeInt(object value)
            {
                if (value == null || value == DBNull.Value) return 0;
                int result;
                return int.TryParse(value.ToString(), out result) ? result : 0;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
        SELECT 
            bi.BATCHID,
            bi.BATCHTYPE,
            bi.BATCHMODE,
            bi.BATCHSUBJECT,
            bi.STARTDATE,
            bi.ENDDATE,
            bi.STARTTIME,
            bi.ENDTIME,
            bi.FACULTYNAME,
            bi.EXPECTEDSTUDENT,
            bi.STATUS,
            bi.USERID,
            cd.COURSEID,
            ISNULL(lc.ChapterNumber, 0) AS LASTCHAPTERNUMBER
        FROM Enosis.BatchesInfo bi
        LEFT JOIN (
            SELECT 
                BATCHID,
                MAX(ChapterNumber) AS ChapterNumber
            FROM Enosis.BatchSheetDetails
            GROUP BY BATCHID
        ) lc ON bi.BATCHID = lc.BATCHID
        LEFT JOIN COURSES_DETAIL cd 
            ON bi.BATCHSUBJECT = cd.COURSENAME
        WHERE 
            bi.STATUS <> 'Complete'
            AND bi.BATCHID = @BATCHID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@BATCHID", batchId);
                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        batch = new Batch
                        {
                            BATCHID = SafeInt(dr["BATCHID"]),
                            BATCHTYPE = dr["BATCHTYPE"]?.ToString() ?? "",
                            BATCHMODE = dr["BATCHMODE"]?.ToString() ?? "",
                            BATCHSUBJECT = dr["BATCHSUBJECT"]?.ToString() ?? "",
                            STARTDATE = dr["STARTDATE"] != DBNull.Value ? Convert.ToDateTime(dr["STARTDATE"]) : DateTime.MinValue,
                            ENDDATE = dr["ENDDATE"] != DBNull.Value ? Convert.ToDateTime(dr["ENDDATE"]) : DateTime.MinValue,
                            STARTTIME = dr["STARTTIME"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)dr["STARTTIME"]) : DateTime.MinValue,
                            ENDTIME = dr["ENDTIME"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)dr["ENDTIME"]) : DateTime.MinValue,
                            FACULTYNAME = dr["FACULTYNAME"]?.ToString() ?? "",
                            EXPECTEDSTUDENT = dr["EXPECTEDSTUDENT"]?.ToString() ?? "",
                            STATUS = dr["STATUS"]?.ToString() ?? "",
                            USERID = SafeInt(dr["USERID"]),
                            COURSEID = SafeInt(dr["COURSEID"]),
                            LASTCHAPTERNUMBER = SafeInt(dr["LASTCHAPTERNUMBER"])
                        };
                    }
                    dr.Close();
                }
            }

            return batch;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateBatchAsComplete(int id, string status)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ToString()))
                {
                    Conn.Open();
                    string query = "UPDATE enosis.BatchesInfo SET Status=@Status WHERE BatchId=@BatchId";
                    using (SqlCommand cmd = new SqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@BatchId", id);
                        cmd.ExecuteNonQuery();
                    }
                    return "success";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        //Update Batch
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateBatch(
      int batchId,
      string batchType,
      string batchMode,
      string batchSubject,
      string startDate,
      string endDate,
      string startTime,
      string endTime,
      string facultyName,
      string expectedStudent,
      string status,
      int userId,
      int courseId,
      string selectedIDs,
      string selectedNames)
        {
            try
            {
                string currentUser = HttpContext.Current.Session["USERNAME"]?.ToString() ?? "System";

                using (SqlConnection con = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString))
                {
                    con.Open();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            #region 1️⃣ Update BatchesInfo

                            string updateBatchQuery = @"
                    UPDATE Enosis.BatchesInfo
                    SET BATCHTYPE = @BATCHTYPE,
                        BATCHMODE = @BATCHMODE,
                        BATCHSUBJECT = @BATCHSUBJECT,
                        STARTDATE = @STARTDATE,
                        ENDDATE = @ENDDATE,
                        STARTTIME = @STARTTIME,
                        ENDTIME = @ENDTIME,
                        FACULTYNAME = @FACULTYNAME,
                        EXPECTEDSTUDENT = @EXPECTEDSTUDENT,
                        STATUS = @STATUS,
                        UPDATEDDATE = @UPDATEDDATE,
                        UPDATEDBY = @UPDATEDBY,
                        USERID = @USERID,
                        COURSEID = @COURSEID
                    WHERE BATCHID = @BATCHID";

                            using (SqlCommand cmd = new SqlCommand(updateBatchQuery, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@BATCHTYPE", batchType);
                                cmd.Parameters.AddWithValue("@BATCHMODE", batchMode);
                                cmd.Parameters.AddWithValue("@BATCHSUBJECT", batchSubject);

                                cmd.Parameters.AddWithValue("@STARTDATE",
                                    string.IsNullOrEmpty(startDate)
                                    ? (object)DBNull.Value
                                    : DateTime.Parse(startDate));

                                cmd.Parameters.AddWithValue("@ENDDATE",
                                    string.IsNullOrEmpty(endDate)
                                    ? (object)DBNull.Value
                                    : DateTime.Parse(endDate));

                                cmd.Parameters.AddWithValue("@STARTTIME",
                                    string.IsNullOrEmpty(startTime)
                                    ? (object)DBNull.Value
                                    : TimeSpan.Parse(startTime));

                                cmd.Parameters.AddWithValue("@ENDTIME",
                                    string.IsNullOrEmpty(endTime)
                                    ? (object)DBNull.Value
                                    : TimeSpan.Parse(endTime));

                                cmd.Parameters.AddWithValue("@FACULTYNAME", facultyName);
                                cmd.Parameters.AddWithValue("@EXPECTEDSTUDENT", expectedStudent);
                                cmd.Parameters.AddWithValue("@STATUS", status);
                                cmd.Parameters.AddWithValue("@UPDATEDDATE", DateTime.Now);
                                cmd.Parameters.AddWithValue("@UPDATEDBY", currentUser);
                                cmd.Parameters.AddWithValue("@USERID", userId);
                                cmd.Parameters.AddWithValue("@COURSEID", courseId);
                                cmd.Parameters.AddWithValue("@BATCHID", batchId);

                                cmd.ExecuteNonQuery();
                            }

                            #endregion

                            #region 2️⃣ Update COURSEID for existing mappings

                            SqlCommand updateMapping = new SqlCommand(@"
                        UPDATE CANDIDATEBATCHMAPPING
                        SET COURSEID = @COURSEID
                        WHERE BATCHID = @BATCHID
                    ", con, tran);

                            updateMapping.Parameters.AddWithValue("@COURSEID", courseId);
                            updateMapping.Parameters.AddWithValue("@BATCHID", batchId);
                            updateMapping.ExecuteNonQuery();

                            #endregion

                            #region 3️⃣ Delete removed students

                            if (!string.IsNullOrEmpty(selectedIDs))
                            {
                                SqlCommand deleteCmd = new SqlCommand(@"
                            DELETE FROM CANDIDATEBATCHMAPPING
                            WHERE BATCHID = @BATCHID
                            AND CANDIDATE_CODE NOT IN (" + selectedIDs + ")", con, tran);
        

                                deleteCmd.Parameters.AddWithValue("@BATCHID", batchId);
                                deleteCmd.ExecuteNonQuery();
                            }

                            #endregion

                            #region 4️⃣ Insert new students

                            if (!string.IsNullOrEmpty(selectedIDs))
                            {
                                string[] studentIdList = selectedIDs.Split(',');

                                foreach (string studentId in studentIdList)
                                {
                                    if (string.IsNullOrWhiteSpace(studentId)) continue;

                                    SqlCommand insertCmd = new SqlCommand(@"
                                IF NOT EXISTS (
                                    SELECT 1 FROM CANDIDATEBATCHMAPPING
                                    WHERE CANDIDATE_CODE = @CandidateID
                                    AND BATCHID = @BATCHID
                                )
                                BEGIN
                                    INSERT INTO CANDIDATEBATCHMAPPING
                                    (CANDIDATE_CODE, COURSEID, BATCHID)
                                    VALUES (@CandidateID, @COURSEID, @BATCHID)
                                END
                            ", con, tran);

                                    insertCmd.Parameters.AddWithValue("@CandidateID", studentId.Trim());
                                    insertCmd.Parameters.AddWithValue("@COURSEID", courseId);
                                    insertCmd.Parameters.AddWithValue("@BATCHID", batchId);

                                    insertCmd.ExecuteNonQuery();
                                }
                            }

                            #endregion

                            tran.Commit();
                            return "success";
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "error: " + ex.Message;
            }
        }


        //Delete Batch
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string DeleteBatch(int batchId)
        {
            string connStr = ConfigurationManager
                .ConnectionStrings["CONN_ENOSISLEARNING"]
                .ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // Delete from CANDIDATEBATCHMAPPING
                    using (SqlCommand cmd1 = new SqlCommand(
                        "DELETE FROM CANDIDATEBATCHMAPPING WHERE BATCHID = @BATCHID",
                        con, transaction))
                    {
                        cmd1.Parameters.Add("@BATCHID", SqlDbType.Int).Value = batchId;
                        cmd1.ExecuteNonQuery();
                    }

                    // Delete from BATCHSHEETDETAILS
                    using (SqlCommand cmd2 = new SqlCommand(
                        "DELETE FROM ENOSIS.BATCHSHEETDETAILS WHERE BATCHID = @BATCHID",
                        con, transaction))
                    {
                        cmd2.Parameters.Add("@BATCHID", SqlDbType.Int).Value = batchId;
                        cmd2.ExecuteNonQuery();
                    }

                    // Delete from BATCHESINFO
                    using (SqlCommand cmd3 = new SqlCommand(
                        "DELETE FROM ENOSIS.BATCHESINFO WHERE BATCHID = @BATCHID",
                        con, transaction))
                    {
                        cmd3.Parameters.Add("@BATCHID", SqlDbType.Int).Value = batchId;
                        int rowsAffected = cmd3.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            transaction.Rollback();
                            return "Error: Batch not found.";
                        }
                    }

                    transaction.Commit();
                    return "Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return "Error: " + ex.Message;
                }
            }
        }
        //Info button click Event for session value
        [WebMethod(EnableSession = true)]
        public static void SetBatchSession(
    int BatchId,
    string FacultyName,
    string StartDate,
    string EndDate,
    int CourseId,
    string BatchSubject)
        {
            HttpContext.Current.Session["BatchId"] = BatchId;
            HttpContext.Current.Session["FacultyName"] = FacultyName;
            HttpContext.Current.Session["StartDate"] = StartDate;
            HttpContext.Current.Session["EndDate"] = EndDate;
            HttpContext.Current.Session["CourseId"] = CourseId;
            HttpContext.Current.Session["BatchSubject"] = BatchSubject;
        }
    }
}