<%@ Page Title="" Language="C#" MasterPageFile="~/NewMyAdmin.Master" AutoEventWireup="true" CodeBehind="Questions.aspx.cs" Inherits="ENOSISLEARNING.Questions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />
    <style>
        @media (max-width: 576px) {
            .row {
                flex-direction: column;
            }
        }

        .form-container {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 3px 10px rgba(0,0,0,0.1);
            max-width: 800px;
            margin: auto;
        }

        label {
            font-weight: 600;
            color: #333;
        }

        input[type="file"] {
            cursor: pointer;
        }

        #btnUpload {
            padding: 10px 24px;
            font-size: 16px;
        }

        input:focus, select:focus {
            border-color: #4a90e2;
            box-shadow: 0 0 4px rgba(74,144,226,0.5);
        }

        input[type="file"] {
            padding: 8px;
            border: 1px dashed #ccc;
            border-radius: 6px;
            background: #fafafa;
        }

            input[type="file"]:hover {
                border-color: #007bff;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager runat="server"></asp:ScriptManager>

    <%--Question upload button--%>
    <button
        type="button"
        class="btn btn-success"
        data-bs-toggle="modal"
        data-bs-target="#uploadQuestionsModal">
        Upload Quiz Questions
    </button>

    <%--Pop-Up Modal--%>
    <div class="modal fade" id="uploadQuestionsModal" tabindex="-1" aria-labelledby="uploadQuestionsModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">

                <!-- Modal header with close -->
                <div class="modal-header">
                    <h5 class="modal-title" id="uploadQuestionsModalLabel">Upload Quiz Questions
                    </h5>
                    <button
                        type="button"
                        class="btn-close"
                        data-bs-dismiss="modal"
                        aria-label="Close">
                    </button>
                </div>

                <!-- Modal body -->
                <div class="modal-body">
                    <div class="container">
                        <div class="row g-3">

                            <div class="col-md-6">
                                <label class="form-label">Select Course</label>
                                <asp:DropDownList
                                    ID="ddlCourses"
                                    runat="server"
                                    CssClass="form-control">
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-6">
                                <label class="form-label">Choose Excel File</label>
                                <input
                                    type="file"
                                    id="fileUploadQuestions"
                                    class="form-control" />
                            </div>

                        </div>
                    </div>
                </div>

                <!-- Modal footer with submit -->
                <div class="modal-footer">
                    <button
                        type="button"
                        id="btnUpload"
                        class="btn btn-primary">
                        Import Questions
                    </button>
                    <button
                        type="button"
                        class="btn btn-secondary"
                        data-bs-dismiss="modal">
                        Close
                    </button>
                </div>

            </div>
        </div>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/sweetalert/2.1.0/sweetalert.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <script>
        $('#btnUpload').click(function () {
            var fileInput = $('#fileUploadQuestions')[0].files[0];
            var courseID = $('#<%=ddlCourses.ClientID%>').val();

            if (!fileInput) {
                swal("Oops!", "Please choose an Excel file", "warning");
                return;
            }

            if (!courseID || courseID === "Select Course") {
                swal("Select Course", "Please select a course", "info");
                return;
            }

            var formData = new FormData();
            formData.append("file", fileInput);
            formData.append("courseID", courseID);

            $.ajax({
                url: "Questions.ashx",
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    swal("Questions uploaded successfully..!!!", response, "success")
                        .then(() => {
                            const myModalEl = document.getElementById('uploadQuestionsModal');
                            const modal = bootstrap.Modal.getInstance(myModalEl);
                            if (modal) modal.hide();

                            // clear leftover backdrop & enable clicking
                            $('.modal-backdrop').remove();
                            $('body').removeClass('modal-open');

                            // optionally redirect back to the main questions page
                            window.location = "Questions.aspx";
                        });
                },
                error: function (err) {
                    console.error(err);
                    swal("Error!", "Upload failed", "error");
                }
            });
        });
    </script>
</asp:Content>
