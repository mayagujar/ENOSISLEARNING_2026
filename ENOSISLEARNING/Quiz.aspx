<%@ Page Title="" Language="C#" MasterPageFile="~/StudentMaster.Master" AutoEventWireup="true" CodeBehind="Quiz.aspx.cs" Inherits="ENOSISLEARNING.Quiz" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2>Take Quiz</h2>

    <div class="mb-3">
        <label>Select Course</label>
        <asp:DropDownList ID="ddlQuizCourses" runat="server" CssClass="form-control"></asp:DropDownList>
    </div>

    <div class="mb-3">
        <label>Select Difficulty Mode</label>
        <asp:DropDownList ID="ddlDifficulty" runat="server" CssClass="form-control">
            <asp:ListItem Text="Easy" Value="1"></asp:ListItem>
            <asp:ListItem Text="Medium" Value="2"></asp:ListItem>
            <asp:ListItem Text="High" Value="3"></asp:ListItem>
            <asp:ListItem Text="Easy to High" Value="4"></asp:ListItem>
            <asp:ListItem Text="Mixed" Value="5"></asp:ListItem>
        </asp:DropDownList>
    </div>

    <button id="btnStartQuiz" class="btn btn-primary" type="button">Start Quiz</button>

    <hr />

    <div id="quizSection" style="display: none;">
        <div id="questionText" class="mb-3"></div>
        <div id="optionsList"></div>
        <button id="btnNext" class="btn btn-success mt-3" type="button">Next</button>
    </div>

    <div id="resultSection" style="display: none;">
        <h3>Your Score: <span id="score"></span></h3>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/sweetalert/2.1.0/sweetalert.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <script>
        var questions = [];
        var currentIndex = 0;
        var score = 0;
        var totalQuestions = 30;

        $('#btnStartQuiz').click(function () {
            var courseID = $('#<%=ddlQuizCourses.ClientID%>').val();
            var diffMode = $('#<%=ddlDifficulty.ClientID%>').val();

            if (!courseID) {
                swal("Select Course", "Please select a course first", "info");
                return;
            }

            $.ajax({
                type: "POST",
                url: "Quiz.aspx/GetQuizQuestions",
                data: JSON.stringify({ courseID: parseInt(courseID), diffMode: parseInt(diffMode) }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {

                    console.log("AJAX Response:", res.d); 
                    if (!res || !res.d || res.d.length === 0) {
                        swal("No Questions Found", "There are no questions available for the selected Course & Difficulty", "info");
                        return;  
                    }

                    questions = res.d.slice(0, totalQuestions);
                    console.log("Loaded Question Count:", questions.length); 

                    currentIndex = 0;
                    score = 0;

                    $('#quizSection').show();
                    showQuestion();
                },

                error: function () {
                    swal("Error", "Could not load questions", "error");
                }
            });

        });

        function showQuestion() {

            if (questions.length === 0) {
                swal("No Questions", "There are no questions to display.", "info");
                return;
            }

            if (currentIndex >= questions.length) {
                finishQuiz();
                return;
            }


            var q = questions[currentIndex];
            $('#questionText').text((currentIndex + 1) + ". " + q.QuestionText);

            var html = `
      <div><label><input type="radio" name="opt" value="1" /> ${q.OptionA}</label></div>
      <div><label><input type="radio" name="opt" value="2" /> ${q.OptionB}</label></div>
      <div><label><input type="radio" name="opt" value="3" /> ${q.OptionC}</label></div>
      <div><label><input type="radio" name="opt" value="4" /> ${q.OptionD}</label></div>
      <div><label><input type="radio" name="opt" value="5" /> ${q.OptionE}</label></div>
    `;
            $('#optionsList').html(html);
        }

        $('#btnNext').click(function () {
            var selected = $('input[name="opt"]:checked').val();

            if (!selected) {
                swal("Hold On!", "Please select an option", "warning");
                return;
            }

            if (selected === questions[currentIndex].CorrectAnswer) {
                score++;
            }

            currentIndex++;
            if (currentIndex < questions.length)
                showQuestion();
            else
                finishQuiz();
        });

        function finishQuiz() {
            let rating = "";

            if (score <= 10) rating = "Low";
            else if (score <= 15) rating = "Better";
            else if (score <= 20) rating = "Good";
            else if (score <= 25) rating = "Excellent";
            else rating = "Outstanding";

            swal({
                title: "Quiz Finished!",
                text: "You scored: " + score + " / " + questions.length + "\nRating: " + rating,
                icon: "success",
                button: "OK"
            });

            $('#quizSection').hide();
        }
    </script>

</asp:Content>

