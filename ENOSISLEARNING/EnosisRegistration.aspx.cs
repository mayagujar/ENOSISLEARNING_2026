using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;


namespace ENOSISLEARNING
{
    public partial class EnosisRegistration : System.Web.UI.Page
    {
        string CONNSTRING1 = System.Configuration.ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ToString();

        int REGNO = 0;
        public static string MODE = "";
        dbconnect objDB = new dbconnect();
        DataTable dt = new DataTable();

        [WebMethod]
        public static List<string> GetAutoCompleteData(string username)
        {
            List<string> result = new List<string>();
            string CONNSTRING1 = System.Configuration.ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ToString();
            using (SqlConnection con = new SqlConnection(CONNSTRING1))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT FULLNAME FROM CANDIDATES where FULLNAME LIKE  @SearchText + '%'", con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchText", username);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(dr["FULLNAME"].ToString());
                    }
                    return result;
                }
            }
        }
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!IsPostBack)
        //        {
        //            BindQualification();
        //            BindCourses();
        //            BindYears();
        //            BindCoordinator();
        //            txtCity.Text = "PUNE";
        //            rbMale.Checked = true;
        //            chkFresher.Checked = true;
        //            txtPinCode.Text = "411015";
        //            txtLocation.Text = "Vishrantwadi";
        //            txtAddress.Text = "Vishrantwadi";

        //            chkPlacement.Checked = true;
        //            //  txtCity.Text = "Vishrantwadi";

        //            txtName.Focus();
        //            if (Request.QueryString["ID"] != null)
        //            {
        //                MODE = OPERATION.EDIT.ToString();

        //                Session["REGNO"] = REGNO = Convert.ToInt32(Request.QueryString["ID"].ToString());

        //                hdnCandidateId.Value = REGNO.ToString();

        //                ShowDetails(REGNO);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        lblError.Text = ex.Message.ToString();
        //        string pageName = System.IO.Path.GetFileName(Request.Path);
        //        dbconnect.LogError(pageName, ex.Message);
        //    }
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    BindQualification();
                    BindCourses();
                    BindYears();
                    BindCoordinator();

                    txtCity.Text = "PUNE";
                    rbMale.Checked = true;
                    chkFresher.Checked = true;
                    txtPinCode.Text = "411015";
                    txtLocation.Text = "Vishrantwadi";
                    txtAddress.Text = "Vishrantwadi";
                    chkPlacement.Checked = true;

                    txtName.Focus();

                    if (Request.QueryString["ID"] != null)
                    {
                        MODE = "EDIT";

                        Session["REGNO"] = REGNO = Convert.ToInt32(Request.QueryString["ID"]);
                        hdnCandidateId.Value = REGNO.ToString();

                        ShowDetails(REGNO);
                    }
                    else
                    {
                        MODE = "ADD"; 
                    }

                    ViewState["MODE"] = MODE; 
                }
                else
                {
                    MODE = ViewState["MODE"].ToString(); 
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
                string pageName = System.IO.Path.GetFileName(Request.Path);
                dbconnect.LogError(pageName, ex.Message);
            }
        }
        private void BindCourses()
        {
            try
            {
                string QUERY = "SELECT COURSEID,COURSENAME FROM COURSES_DETAIL WHERE STATUS='A'";
                drpCourses.DataSource = dbconnect.GetDataSet(QUERY);
                drpCourses.DataTextField = "COURSENAME";
                drpCourses.DataValueField = "COURSEID";
                drpCourses.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        public void BindYears()
        {
            drpPassingYears.DataSource = DBGetData.GetYears();
            drpPassingYears.DataBind();
        }
        private void ShowDetails(int RegNo)
        {
            try
            {
                string QUERY = "SELECT * FROM CANDIDATES WHERE CANDIDATE_CODE=" + REGNO;
                dt = dbconnect.GetDataSet(QUERY);
                if (dt.Rows.Count > 0)
                {
                    txtName.Text = dt.Rows[0]["FULLNAME"].ToString();
                    txtName.Enabled = false;
                    if (dt.Rows[0]["GENDER"].ToString().ToUpper() == GENDER.MALE.ToString())
                    {
                        rbMale.Checked = true;
                    }
                    else
                    {
                        rbFeMale.Checked = true;
                    }
                    DateTime dtDOB = Convert.ToDateTime(dt.Rows[0]["DOB"].ToString());

                    txtDOB.Text = dtDOB.ToString("yyyy-MM-dd");

                   // txtDOB.Text = dtDOB.ToString("dd/MM/yyyy").Replace("-", "/");
                  
                    txtMobileNo.Text = dt.Rows[0]["MOBILENO"].ToString(); ;
                    txtEmail.Text = dt.Rows[0]["EMAIL"].ToString();
                    string query = "SELECT COURSEID FROM CANDIDATES_COURSES WHERE CANDIDATEID=" + RegNo;
                    DataTable dtCourses = new DataTable();
                    dtCourses = dbconnect.GetDataSet(query);
                    for (int r = 0; r < dtCourses.Rows.Count; r = r + 1)
                    {
                        for (int i = 0; i < drpCourses.Items.Count; i = i + 1)
                        {
                            if (drpCourses.Items[i].Value == dtCourses.Rows[r][0].ToString())
                            {
                                drpCourses.Items[i].Selected = true;
                            }
                        }
                    }
                    txtSkills.Text = dt.Rows[0]["SKILLS"].ToString();
                    txtAddress.Text = dt.Rows[0]["ADDRESS"].ToString();
                    txtLocation.Text = dt.Rows[0]["LOCATION"].ToString();
                    txtPinCode.Text = dt.Rows[0]["PINCODE"].ToString();
                    txtCity.Text = dt.Rows[0]["CITY"].ToString();
                    drpPassingYears.SelectedValue = dt.Rows[0]["PASSINGYEAR"].ToString();
                    drpQualifications.SelectedValue = dt.Rows[0]["QUALIFICATION"].ToString();
                    txtPercentage.Text = String.Format("{0:0.##}", dt.Rows[0]["PERCENTAGE"].ToString()); 

                    drpCoordinator.SelectedValue = dt.Rows[0]["COUNSELLORNAME"].ToString();
                    if (dt.Rows[0]["REQPLACEMENT"].ToString().ToUpper() == "TRUE")
                    {
                        chkPlacement.Checked = true;
                    }
                    else
                    {
                        chkPlacement.Checked = false;
                    }
                    if (dt.Rows[0]["ISFRESHER"].ToString().ToUpper() == "FALSE")
                    {
                        chkFresher.Checked = false;
                        SetExp(true);
                        drpExpYears.Attributes.Add("required", "true");
                        txtCompanyName.Attributes.Add("required", "true");
                        drpExpYears.SelectedValue = dt.Rows[0]["YEARSEXPERIENCE"].ToString();
                        drpMonths.SelectedValue = dt.Rows[0]["MONTHSEXPERIENCE"].ToString();
                        txtCompanyName.Text = dt.Rows[0]["COMPANYNAME"].ToString();

                    }
                    else
                    {
                        chkFresher.Checked = true;
                        SetExp(false);
                        drpExpYears.Attributes.Add("required", "false");
                        txtCompanyName.Attributes.Add("required", "false");
                    }
                    if (dt.Rows[0]["RESUME"] != null && dt.Rows[0]["RESUME"].ToString() != "")
                    {
                        hykResume.Text = "<i class=\"	fa fa-download\"></i>  " + dt.Rows[0]["RESUME"].ToString();
                        ViewState["RESUME"] = dt.Rows[0]["RESUME"].ToString();
                        hykResume.NavigateUrl = "~/Resumes/" + dt.Rows[0]["RESUME"].ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                lblError.Text = ex.Message.ToString();
                string pageName = System.IO.Path.GetFileName(Request.Path);
                dbconnect.LogError(pageName, ex.Message);
            }
        }
        private void BindCoordinator()
        {
            string con = ConfigurationManager.ConnectionStrings["CONN_ENOSISLEARNING"].ConnectionString.ToString();
            try
            {
                string query = "select USERID,UPPER(USERNAME) AS USERNAME from USERDETAILS WHERE STATUS='ACTIVE'";
                //drpCoordinator.DataSource = DBGetData.GetCoordinator();
                //drpCourses.DataSource = dbconnect.GetDataSet(query);
                SqlDataAdapter sda = new SqlDataAdapter(query, con);
                DataTable data = new DataTable();
                sda.Fill(data);
                drpCoordinator.DataSource = data;
                drpCoordinator.DataTextField = "USERNAME";
                drpCoordinator.DataValueField = "USERNAME";
                drpCoordinator.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private void BindQualification()
        {
            try
            {
                drpQualifications.DataSource = DBGetData.GetEducation();
                drpQualifications.DataTextField = "NAME";
                drpQualifications.DataValueField = "ID";
                drpQualifications.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private int CheckEmail(string semail)
        {
            int code = 0;
            try
            {
                code = objDB.CheckEmail(semail, "I");
                if (code > 0)
                {
                    string str = "EmailId Already Exists, Please Check. Registration Id=" + code.ToString() + "<br/> Please Update Your Information";
                    lblError.Text = str;
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + str + "')", true);
                    ShowDetails(code);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            return code;
        }
        //protected void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    CandidateEntity obj = new CandidateEntity();
        //    try
        //    {
        //        if (Page.IsValid)
        //        {

        //            obj.Name = txtName.Text.ToString();
        //            obj.Gender = rbMale.Checked == true ? rbMale.Text : rbFeMale.Text;

        //            DateTime dt;
        //            if (txtDOB.Text != null && txtDOB.Text.Length > 0)
        //            {

        //                 dt = Convert.ToDateTime(txtDOB.Text);
        //                obj.dobdate = dt.Month + "/" + dt.Day + "/" + dt.Year;
        //                obj.dobpass = dt.Day + "/" + dt.Month + "/" + dt.Year;

        //            }
        //            else
        //            {
        //                lblError.Text = "Please Enter Date Correctly";
        //                return;


        //            }
        //            obj.semail = txtEmail.Text.ToString();
        //            obj.smobile = txtMobileNo.Text.ToString();
        //            if (drpCourses.SelectedItem != null)
        //            {
        //                obj.CourseId = Convert.ToInt32(drpCourses.SelectedItem.Value.ToString());
        //            }
        //            obj.Skills = txtSkills.Text;
        //            obj.Address = txtAddress.Text;
        //            obj.PinCode = txtPinCode.Text;
        //            obj.City = txtCity.Text;
        //            obj.Location = txtLocation.Text;
        //            if (drpQualifications.SelectedItem != null && drpQualifications.SelectedItem.Value != "0")
        //            {
        //                obj.Qualification = Convert.ToInt32(drpQualifications.SelectedItem.Value.ToString());
        //            }
        //            else
        //            {
        //                lblError.Text = "Please Select Qualification";
        //                return;
        //            }

        //            if (txtPercentage.Text != null && txtPercentage.Text.Length > 0)
        //                obj.percentage = float.Parse(txtPercentage.Text.ToString());

        //            if (drpPassingYears.SelectedValue != null && drpPassingYears.SelectedValue.Length > 0)
        //                obj.PassingYears = Convert.ToInt32(drpPassingYears.SelectedValue);

        //            if (chkFresher.Checked == false)
        //            {
        //                if (drpExpYears.SelectedValue != null && drpExpYears.SelectedValue.Length > 0)
        //                    obj.ExpYears = Convert.ToInt32(drpExpYears.SelectedValue);
        //                if (drpMonths.SelectedValue != null && drpMonths.SelectedValue.Length > 0)
        //                    obj.iExpMonths = Convert.ToInt32(drpMonths.SelectedValue);
        //                obj.CompanyName = txtCompanyName.Text;
        //            }
        //            else
        //            {
        //                obj.IsFresher = 1;
        //            }
        //            obj.ReqPlacement = chkPlacement.Checked ?1:0;


        //            Session["CANDIDATENAME"] = txtName.Text;
        //            obj.strcoordinator = drpCoordinator.SelectedItem.Text;
        //            if (MODE == OPERATION.ADD.ToString())
        //            {
        //                if (CheckEmail(obj.semail) > 0)
        //                    return;
        //            }
        //            string BatchCourse = "";
        //            string Course = "";
        //            if (drpCourses.SelectedItem != null && drpCourses.SelectedItem.Text != "")
        //            {

        //                Course = drpCourses.SelectedItem.Text;
        //            }
        //            else
        //            {
        //                lblError.Text = "Please Select Courses";
        //                return;
        //            }
        //            string CurrentMonth = String.Format("{0:MMM}", DateTime.Now).ToString().ToUpper();
        //            BatchCourse = DBGetData.GetBatchCourseName(Course);
        //            obj.BatchName = DateTime.Today.Year.ToString() + "-" + CurrentMonth + "-" + BatchCourse;


        //            if (txtTotalFees.Text != null && txtTotalFees.Text.Length > 0)
        //                obj.totalfees = Convert.ToDouble(txtTotalFees.Text);

        //            SaveResume();
        //            if (txtName.Enabled == true)
        //            {
        //                obj.CandidateId = obj.AddStudent();
        //                try
        //                {
        //                 //   SENDEMAIL s1 = new SENDEMAIL();
        //                   // s1.SendRegistrationEmail(obj.Name, obj.semail, obj.CandidateId, "enosisregistration.aspx");
        //                }
        //                catch(Exception ex)
        //                {
        //                    string pageName = System.IO.Path.GetFileName(Request.Path);
        //                    dbconnect.LogError(pageName, ex.Message);
        //                }

        //            }
        //            else
        //            {

        //                obj.CandidateId = Convert.ToInt32(Session["REGNO"].ToString());
        //                obj.UpdateStudent();

        //            }
        //            InsertCourses(obj.CandidateId);

        //        }
        //        ViewState["CANDIDATEID"] = obj.CandidateId;
        //      //  PDFLIB.genrateRegistrationPdf(txtName.Text, drpCourses.SelectedItem.Text, txtTotalFees.Text, txtMobileNo.Text, obj.Gender, txtDOB.Text, txtEmail.Text, drpQualifications.SelectedItem.Text, drpExpYears.SelectedItem.Text, drpMonths.SelectedItem.Text);
        //        updateRegistrationStatus();
        //            if (Session["USERNAME"] != null)
        //            {
        //                Response.Write(@"<script language='javascript'>alert('You Have Registered Your Details Successfully');</script>");
        //                Response.Redirect("RegConfirmation.aspx?Id=" + obj.CandidateId);
        //            }
        //            else
        //            {
        //                Response.Write(@"<script language='javascript'>alert('You Have Updated Your Details Successfully');</script>");

        //                Response.Redirect("RegConfirmation.aspx?Id=" + obj.CandidateId);
        //            }
        //    }
        //    catch (Exception ex)
        //    {
        //        lblError.Text = ex.Message.ToString();
        //        string pageName = System.IO.Path.GetFileName(Request.Path);
        //        dbconnect.LogError(pageName, ex.Message);
        //    }
        //}
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CandidateEntity obj = new CandidateEntity();

            try
            {
                if (Page.IsValid)
                {
                    // 🔹 Common Data Mapping
                    obj.Name = txtName.Text;
                    obj.Gender = rbMale.Checked ? rbMale.Text : rbFeMale.Text;

                    if (!string.IsNullOrEmpty(txtDOB.Text))
                    {
                        DateTime dt = Convert.ToDateTime(txtDOB.Text);
                        obj.dobdate = dt.Month + "/" + dt.Day + "/" + dt.Year;
                        obj.dobpass = dt.Day + "/" + dt.Month + "/" + dt.Year;
                    }
                    else
                    {
                        lblError.Text = "Please Enter Date Correctly";
                        return;
                    }

                    obj.semail = txtEmail.Text;
                    obj.smobile = txtMobileNo.Text;

                    if (drpCourses.SelectedItem != null)
                        obj.CourseId = Convert.ToInt32(drpCourses.SelectedValue);
                    else
                    {
                        lblError.Text = "Please Select Courses";
                        return;
                    }

                    obj.Skills = txtSkills.Text;
                    obj.Address = txtAddress.Text;
                    obj.PinCode = txtPinCode.Text;
                    obj.City = txtCity.Text;
                    obj.Location = txtLocation.Text;

                    if (drpQualifications.SelectedValue != "0")
                        obj.Qualification = Convert.ToInt32(drpQualifications.SelectedValue);
                    else
                    {
                        lblError.Text = "Please Select Qualification";
                        return;
                    }

                    if (!string.IsNullOrEmpty(txtPercentage.Text))
                        obj.percentage = float.Parse(txtPercentage.Text);

                    if (!string.IsNullOrEmpty(drpPassingYears.SelectedValue))
                        obj.PassingYears = Convert.ToInt32(drpPassingYears.SelectedValue);

                    // 🔹 Experience (simple direct)
                    obj.ExpYears = string.IsNullOrEmpty(drpExpYears.SelectedValue) ? 0 : Convert.ToInt32(drpExpYears.SelectedValue);
                    obj.iExpMonths = string.IsNullOrEmpty(drpMonths.SelectedValue) ? 0 : Convert.ToInt32(drpMonths.SelectedValue);
                    obj.CompanyName = txtCompanyName.Text;

                    obj.ReqPlacement = chkPlacement.Checked ? 1 : 0;

                    obj.strcoordinator = drpCoordinator.SelectedItem.Text;

                    // 🔹 Batch
                    string Course = drpCourses.SelectedItem.Text;
                    string CurrentMonth = DateTime.Now.ToString("MMM").ToUpper();
                    string BatchCourse = DBGetData.GetBatchCourseName(Course);
                    obj.BatchName = DateTime.Now.Year + "-" + CurrentMonth + "-" + BatchCourse;

                    if (!string.IsNullOrEmpty(txtTotalFees.Text))
                        obj.totalfees = Convert.ToDouble(txtTotalFees.Text);

                    // 🔥 Save Resume
                    SaveResume();

                    // ===============================
                    // 💥 MODE BASED LOGIC
                    // ===============================

                    if (MODE == OPERATION.ADD.ToString())
                    {
                        if (CheckEmail(obj.semail) > 0)
                            return;

                        // 🔥 Save Resume first
                        SaveResume();

                        using (SqlConnection con = new SqlConnection(CONNSTRING1))
                        {
                            string query = @"INSERT INTO CANDIDATES
        (FULLNAME, GENDER, DOB, MOBILENO, EMAIL, COURSEID, SKILLS, ADDRESS, LOCATION, CITY, PINCODE,
         QUALIFICATION, PERCENTAGE, PASSINGYEAR, YEARSEXPERIENCE, MONTHSEXPERIENCE, COMPANYNAME,
         RESUME, CREATEDDATE, ReqPlacement, COUNSELLORNAME, BATCHNAME, TOTALFEES)

        VALUES
        (@FULLNAME, @GENDER, @DOB, @MOBILENO, @EMAIL, @COURSEID, @SKILLS, @ADDRESS, @LOCATION, @CITY, @PINCODE,
         @QUALIFICATION, @PERCENTAGE, @PASSINGYEAR, @YEARSEXPERIENCE, @MONTHSEXPERIENCE, @COMPANYNAME,
         @RESUME, GETDATE(), @ReqPlacement, @COUNSELLORNAME, @BATCHNAME, @TOTALFEES);

        SELECT SCOPE_IDENTITY();";

                            SqlCommand cmd = new SqlCommand(query, con);

                            // 🔹 Basic
                            cmd.Parameters.AddWithValue("@FULLNAME", txtName.Text);
                            cmd.Parameters.AddWithValue("@GENDER", rbMale.Checked ? "Male" : "Female");
                            cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(txtDOB.Text));
                            cmd.Parameters.AddWithValue("@MOBILENO", txtMobileNo.Text);
                            cmd.Parameters.AddWithValue("@EMAIL", txtEmail.Text);

                            // 🔹 Course
                            cmd.Parameters.AddWithValue("@COURSEID", drpCourses.SelectedValue);

                            // 🔹 Contact
                            cmd.Parameters.AddWithValue("@SKILLS", txtSkills.Text);
                            cmd.Parameters.AddWithValue("@ADDRESS", txtAddress.Text);
                            cmd.Parameters.AddWithValue("@LOCATION", txtLocation.Text);
                            cmd.Parameters.AddWithValue("@CITY", txtCity.Text);
                            cmd.Parameters.AddWithValue("@PINCODE", txtPinCode.Text);

                            // 🔹 Education
                            cmd.Parameters.AddWithValue("@QUALIFICATION", drpQualifications.SelectedValue);
                            cmd.Parameters.AddWithValue("@PERCENTAGE",
                                string.IsNullOrEmpty(txtPercentage.Text) ? 0 : Convert.ToDouble(txtPercentage.Text));
                            cmd.Parameters.AddWithValue("@PASSINGYEAR", drpPassingYears.SelectedValue);

                            // 🔹 Experience (direct, no logic)
                            cmd.Parameters.AddWithValue("@YEARSEXPERIENCE", drpExpYears.SelectedValue);
                            cmd.Parameters.AddWithValue("@MONTHSEXPERIENCE", drpMonths.SelectedValue);
                            cmd.Parameters.AddWithValue("@COMPANYNAME", txtCompanyName.Text);

                            // 🔹 Resume
                            string resume = ViewState["FILENAME"] != null ? ViewState["FILENAME"].ToString() : "";
                            cmd.Parameters.AddWithValue("@RESUME", resume);

                            // 🔹 Other
                            cmd.Parameters.AddWithValue("@ReqPlacement", chkPlacement.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@COUNSELLORNAME", drpCoordinator.SelectedItem.Text);

                            // 🔥 Batch Name Logic (fixed)
                            string courseName = drpCourses.SelectedItem.Text;
                            string currentMonth = DateTime.Now.ToString("MMM").ToUpper();
                            string batchCourse = DBGetData.GetBatchCourseName(courseName);
                            string batchName = DateTime.Now.Year + "-" + currentMonth + "-" + batchCourse;

                            cmd.Parameters.AddWithValue("@BATCHNAME", batchName);

                            // 🔹 Fees
                            cmd.Parameters.AddWithValue("@TOTALFEES",
                                string.IsNullOrEmpty(txtTotalFees.Text) ? 0 : Convert.ToDouble(txtTotalFees.Text));

                            // 🔥 Execute
                            con.Open();
                            obj.CandidateId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                    else
                    {
                        // 🔥 UPDATE (as it is)
                        obj.CandidateId = Convert.ToInt32(Session["REGNO"]);
                        obj.UpdateStudent();
                    }

                    // 🔹 Courses Insert
                    InsertCourses(obj.CandidateId);

                    ViewState["CANDIDATEID"] = obj.CandidateId;

                    updateRegistrationStatus();

                    if (Session["USERNAME"] != null)
                    {
                        Response.Write("<script>alert('You Have Registered Your Details Successfully');</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('You Have Updated Your Details Successfully');</script>");
                    }

                    Response.Redirect("RegConfirmation.aspx?Id=" + obj.CandidateId);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                string pageName = System.IO.Path.GetFileName(Request.Path);
                dbconnect.LogError(pageName, ex.Message);
            }
        }


        //Add Candidate Details to Database
        public int InsertCandidate()
        {
            int newId = 0;

            using (SqlConnection con = new SqlConnection("CONN_ENOSISLEARNING"))
            {
                string query = @"INSERT INTO Candidate
        (Name, Gender, dobdate, dobpass, Email, MobileNo, CourseId, Skills, Address, PinCode, City, Location,
         Qualification, Percentage, PassingYears, ExpYears, ExpMonths, CompanyName, ReqPlacement,
         Coordinator, BatchName, TotalFees, Resume)

        VALUES
        (@Name, @Gender, @dobdate, @dobpass, @Email, @MobileNo, @CourseId, @Skills, @Address, @PinCode, @City, @Location,
         @Qualification, @Percentage, @PassingYears, @ExpYears, @ExpMonths, @CompanyName, @ReqPlacement,
         @Coordinator, @BatchName, @TotalFees, @Resume);

        SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, con);

                // 🔹 Direct form values
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Gender", rbMale.Checked ? "Male" : "Female");

                DateTime dt = Convert.ToDateTime(txtDOB.Text);
                cmd.Parameters.AddWithValue("@dobdate", dt.Month + "/" + dt.Day + "/" + dt.Year);
                cmd.Parameters.AddWithValue("@dobpass", dt.Day + "/" + dt.Month + "/" + dt.Year);

                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@MobileNo", txtMobileNo.Text);

                cmd.Parameters.AddWithValue("@CourseId", drpCourses.SelectedValue);

                cmd.Parameters.AddWithValue("@Skills", txtSkills.Text);
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@PinCode", txtPinCode.Text);
                cmd.Parameters.AddWithValue("@City", txtCity.Text);
                cmd.Parameters.AddWithValue("@Location", txtLocation.Text);

                cmd.Parameters.AddWithValue("@Qualification", drpQualifications.SelectedValue);
                cmd.Parameters.AddWithValue("@Percentage", txtPercentage.Text);
                cmd.Parameters.AddWithValue("@PassingYears", drpPassingYears.SelectedValue);

                cmd.Parameters.AddWithValue("@ExpYears", drpExpYears.SelectedValue);
                cmd.Parameters.AddWithValue("@ExpMonths", drpMonths.SelectedValue);
                cmd.Parameters.AddWithValue("@CompanyName", txtCompanyName.Text);

                cmd.Parameters.AddWithValue("@ReqPlacement", chkPlacement.Checked ? 1 : 0);

                cmd.Parameters.AddWithValue("@Coordinator", drpCoordinator.SelectedItem.Text);

                string Course = drpCourses.SelectedItem.Text;
                string CurrentMonth = DateTime.Now.ToString("MMM").ToUpper();
                string BatchCourse = DBGetData.GetBatchCourseName(Course);

                string BatchName = DateTime.Now.Year + "-" + CurrentMonth + "-" + BatchCourse;
                cmd.Parameters.AddWithValue("@BatchName", BatchName);

                cmd.Parameters.AddWithValue("@TotalFees", txtTotalFees.Text);

                string resume = ViewState["FILENAME"] != null ? ViewState["FILENAME"].ToString() : "";
                cmd.Parameters.AddWithValue("@Resume", resume);

                con.Open();
                newId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return newId;
        }
        private void SaveResume()
        {
            string filename = "";
            string resumeFolder = Server.MapPath(@"~\Resumes\");

            if (!Directory.Exists(resumeFolder))
            {
                Directory.CreateDirectory(resumeFolder);
            }

            if (fResume.HasFile)
            {
                string fileExt = Path.GetExtension(fResume.FileName);

                filename = txtName.Text.Replace(" ", "_") + "_" + drpPassingYears.SelectedValue + fileExt;

                string fullPath = Path.Combine(resumeFolder, filename);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                fResume.SaveAs(fullPath);

                hykResume.Text = filename;
                hykResume.NavigateUrl = "~/Resumes/" + filename;

                ViewState["RESUME"] = filename;

                ViewState["FILENAME"] = filename;
            }
            else if (ViewState["RESUME"] != null)
            {
                filename = ViewState["RESUME"].ToString();
                hykResume.Text = filename;
                hykResume.NavigateUrl = "~/Resumes/" + filename;

                ViewState["FILENAME"] = filename;
            }
        }

        protected void updateRegistrationStatus()
        {
            try
            {
                dbconnect db = new dbconnect();
                if (ViewState["FILENAME"] != null)
                {
                    string file = ViewState["FILENAME"].ToString();
                    string name = txtName.Text;
                    int i = db.updateRegistrationStatus(name, file);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
        protected void InsertCourses(int RegNo)
        {
            dbconnect.ExecuteQuery("DELETE FROM CANDIDATES_COURSES WHERE CANDIDATEID=" + RegNo);
            string message = "";
            foreach (System.Web.UI.WebControls.ListItem item in drpCourses.Items)
            {
                if (item.Selected)
                {
                    dbconnect.ExecuteQuery("insert into CANDIDATES_COURSES values(" + RegNo + "," + item.Value.ToString() + ",GETDATE())");
                    message += item.Text + " " + item.Value + "\\n";
                }
            }
            Response.Write(message);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "alert('" + message + "');", true);
        }
        protected void drpCourses_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string message = "";
                foreach (System.Web.UI.WebControls.ListItem item in drpCourses.Items)
                {
                    if (item.Selected)
                    {
                        if (message.Length > 0)
                        {
                            message = message + "," + item.Value;
                        }
                        else
                        {
                            message = item.Value;
                        }
                    }
                }
                if (message.Length > 0)
                {
                    string str = "select sum(fees) from COURSES_DETAIL where COURSEID in(" + message + ")";
                    double fees = Convert.ToDouble(dbconnect.ExecuteScalerQuery(str));
                    lblFees.Visible = true;
                    txtTotalFees.Visible = true;
                    txtTotalFees.Text = fees.ToString();
                }
                else
                {
                    lblFees.Visible = false;
                    txtTotalFees.Visible = false;
                }
            }
            catch(Exception ex)
            {
                lblError.Text = ex.Message;
                string pageName = System.IO.Path.GetFileName(Request.Path);
                dbconnect.LogError(pageName, ex.Message);
            }
        }
        protected void chkFresher_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFresher.Checked == true)
            {
                SetExp(false);
            }
            else
            {
                SetExp(true);
            }
        }
        private void SetExp(bool b1)
        {
            trexp1.Visible = b1;
            trexp2.Visible = b1;
            trexp3.Visible = b1;
            trexp4.Visible = b1;
        }
    }
}
