<%@ Page Title="" Language="C#" MasterPageFile="~/NewMyAdmin.Master" AutoEventWireup="true" CodeBehind="AllCourseContent.aspx.cs" Inherits="ENOSISLEARNING.FinalBatchSheet" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />
<style>

/* ===== Glass Mask Style ===== */
.masked-table {
    border-radius: 4px;
    overflow: hidden;
    background: #ffffff;
    box-shadow: 0 8px 20px rgba(0, 0, 0, 0.06);
}

/* Header Style */
.masked-table thead {
    background: linear-gradient(135deg, #1e3c72, #2a5298);
    color: #fff;
    font-weight: 500;
}

.masked-table thead th {
    border: none !important;
    padding: 12px;
}

/* Body Style */
.masked-table tbody td {
    padding: 12px;
    border-top: 1px solid #f0f0f0;
}

/* Hover Effect */
.masked-table tbody tr:hover {
    background-color: #f5f9ff;
    transition: 0.2s ease-in-out;
}

/* Inner Topic Table Slightly Softer */
.inner-table {
    background: #f9fbff;
    box-shadow: inset 0 0 10px rgba(0,0,0,0.03);
}

/* Footer Input Styling */
.masked-table tfoot input {
    border-radius: 8px;
    border: 1px solid #dce3f0;
}

.masked-table tfoot button {
    border-radius: 8px;
}

/* Smooth Collapse Animation */
.collapse {
    transition: all 0.3s ease;
}

</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
                  <asp:DropDownList 
    ID="drpCourses" 
    runat="server" 
    CssClass="form-select custom-select"
    style="width:155px;height:35px;">
</asp:DropDownList>

<div id="chapterAccordion">
    <table class="table table-bordered mt-2 masked-table" id="chapterTable" style="display:none;">
    <thead>
        <tr>
            <th>Chapter Number</th>
            <th style="width:100px">Action</th>
        </tr>
    </thead>

    <tbody id="chapterBody"></tbody>

    <!-- ✅ FOOTER ADD SECTION -->
    <tfoot>
        <tr>
            <td>
                <input type="text" id="txtNewChapter"
                       class="form-control"
                       placeholder="Enter new chapter name..." />
            </td>
            <td>
                <button type="button"
                        id="btnAddChapter"
                        class="btn btn-success w-100">
                    Add
                </button>
            </td>
        </tr>
    </tfoot>
</table>
</div>
    <%-- Scripts --%>
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
<script>
    $(document).ready(function () {

        // ===============================
        // COURSE CHANGE → LOAD CHAPTERS
        // ===============================
        $('#<%= drpCourses.ClientID %>').change(function () {

        var courseId = $(this).val();

        if (courseId != "") {

            $.ajax({
                type: "POST",
                url: "AllCourseContent.aspx/ViewChapterGridView",
                data: JSON.stringify({ Course_Id: parseInt(courseId) }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",

                success: function (response) {

                    var data = response.d;
                    var rows = "";

                    if (data.length > 0) {

                        $("#chapterTable").show();

                        $.each(data, function (i, item) {

                            // ===== Chapter Row =====
                            rows += "<tr>";
                            rows += "<td>";

                            rows += "<a data-bs-toggle='collapse' href='#collapse" + item.Chapter_Id + "' ";
                            rows += "role='button' aria-expanded='false' ";
                            rows += "class='text-decoration-none fw-bold d-flex align-items-center w-100'>";

                            rows += "<i class='fa-solid fa-plus me-2 toggle-icon'></i>";
                            rows += item.Chapter_Number;
                            rows += "</a>";

                            rows += "</td>";

                            rows += "<td class='text-center'>";
                            rows += "<a class='btn btn-sm btn-primary edit-btn me-1' data-id='" + item.Chapter_Id + "'>";
                            rows += "<i class='fa fa-edit'></i></a>";

                            rows += "<a class='btn btn-sm btn-danger delete-btn' data-id='" + item.Chapter_Id + "'>";
                            rows += "<i class='fa fa-trash'></i></a>";
                            rows += "</td>";

                            rows += "</tr>";


                            // ===== Collapse Row =====
                            rows += "<tr>";
                            rows += "<td colspan='2' class='p-0 border-0'>";

                            // 👇 IMPORTANT CHANGE HERE
                            rows += "<div class='collapse' id='collapse" + item.Chapter_Id + "' data-bs-parent='#chapterAccordion'>";

                            rows += "<div class='p-3 bg-light'>";

                            rows += "<table class='table table-sm mb-0 masked-table inner-table'>";
                            rows += "<thead class='table-light'>";
                            rows += "<tr>";
                            rows += "<th>Topic Name</th>";
                            rows += "<th style='width:80px'>Days</th>";
                            rows += "<th style='width:140px' class='text-center'>Action</th>";
                            rows += "</tr>";
                            rows += "</thead>";

                            // ===== BODY =====
                            rows += "<tbody class='topic-body'>";
                            rows += "<tr><td colspan='3' class='text-center text-muted'>Click to load topics</td></tr>";
                            rows += "</tbody>";

                            // ===== FOOTER =====
                            rows += "<tfoot>";
                            rows += "<tr>";

                            rows += "<td>";
                            rows += "<input type='text' class='form-control form-control-sm topic-name-input' placeholder='Enter topic name'>";
                            rows += "</td>";

                            rows += "<td>";
                            rows += "<input type='number' min='1' class='form-control form-control-sm topic-days-input' placeholder='Days'>";
                            rows += "</td>";

                            rows += "<td class='text-center'>";
                            rows += "<button type='button' class='btn btn-sm btn-success add-topic-btn' data-chapter='" + item.Chapter_Id + "'>";
                            rows += "Add Topic";
                            rows += "</button>";
                            rows += "</td>";

                            rows += "</tr>";
                            rows += "</tfoot>";

                            rows += "</table>";
                            rows += "</div>";
                            rows += "</div>";

                            rows += "</td>";
                            rows += "</tr>";
                        });

                    } else {

                        $("#chapterTable").show();
                        rows = "<tr><td colspan='2' class='text-center'>No Data Found</td></tr>";
                    }

                    $("#chapterBody").html(rows);
                }
            });

        } else {
            $("#chapterTable").hide();
            $("#chapterBody").html("");
        }

    });


    // ===============================
    // ICON TOGGLE
    // ===============================
    $(document).on('shown.bs.collapse', '.collapse', function () {
        var icon = $(this).closest('tr').prev('tr').find('.toggle-icon');
        icon.removeClass('fa-plus').addClass('fa-minus');
    });

    $(document).on('hidden.bs.collapse', '.collapse', function () {
        var icon = $(this).closest('tr').prev('tr').find('.toggle-icon');
        icon.removeClass('fa-minus').addClass('fa-plus');
    });


    // ===============================
    // LOAD TOPICS ON EXPAND
    // ===============================
    $(document).on('shown.bs.collapse', '.collapse', function () {

        var collapseDiv = $(this);
        var chapterId = collapseDiv.attr("id").replace("collapse", "");
        var topicContainer = collapseDiv.find(".topic-body");

        topicContainer.html("<tr><td colspan='3' class='text-center'>Loading...</td></tr>");

        $.ajax({
            type: "POST",
            url: "AllCourseContent.aspx/GetTopicsByChapterId",
            data: JSON.stringify({ Chapter_Id: parseInt(chapterId) }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (response) {

                var data = response.d;
                var topicRows = "";

                if (data.length > 0) {

                    $.each(data, function (i, item) {

                        topicRows += "<tr>";
                        topicRows += "<td>" + item.Topic_Name + "</td>";
                        topicRows += "<td>" + item.Days + "</td>";

                        topicRows += "<td class='text-center'>";

                        topicRows += "<a class='btn btn-sm btn-primary topic-edit-btn me-1' ";
                        topicRows += "data-id='" + item.Topic_Id + "' ";
                        topicRows += "data-name='" + item.Topic_Name + "' ";
                        topicRows += "data-days='" + item.Days + "'>";
                        topicRows += "<i class='fa fa-edit'></i></a>";

                        topicRows += "<a class='btn btn-sm btn-danger topic-delete-btn' ";
                        topicRows += "data-id='" + item.Topic_Id + "'>";
                        topicRows += "<i class='fa fa-trash'></i></a>";

                        topicRows += "</td>";
                        topicRows += "</tr>";
                    });

                } else {
                    topicRows = "<tr><td colspan='3' class='text-center'>No Topics Found</td></tr>";
                }

                topicContainer.html(topicRows);
            }
        });

    });
 });
</script>
<script>
    // Add Chapter Button Click
    $("#btnAddChapter").click(function () {

        var chapterName = $("#txtNewChapter").val().trim();
        var courseId = $('#<%= drpCourses.ClientID %>').val();

    if (courseId == "") {
        Swal.fire({
            icon: 'warning',
            title: 'Select Course First',
            text: 'Please select a course before adding chapter.'
        });
        return;
    }

    if (chapterName == "") {
        Swal.fire({
            icon: 'info',
            title: 'Enter Chapter Name',
            text: 'Chapter name cannot be empty.'
        });
        return;
    }

    $.ajax({
        type: "POST",
        url: "AllCourseContent.aspx/AddChapter",
        data: JSON.stringify({
            Course_Id: parseInt(courseId),
            Chapter_Name: chapterName
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        beforeSend: function () {
            Swal.fire({
                title: 'Adding Chapter...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
        },

        success: function (response) {

            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: 'Chapter added successfully.',
                timer: 1500,
                showConfirmButton: false
            });

            $("#txtNewChapter").val("");
            $('#<%= drpCourses.ClientID %>').trigger("change");
        },

        error: function (err) {

            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Something went wrong!'
            });

            console.log(err);
        }
    });

    });

    //Edit Chapter Button Click
    // EDIT CHAPTER
    $(document).on("click", ".edit-btn", function () {

        var chapterId = $(this).data("id");
        var currentName = $(this).closest("tr").find("td:first").text().trim();

        Swal.fire({
            title: 'Edit Chapter',
            input: 'text',
            inputValue: currentName,
            inputPlaceholder: 'Enter new chapter name',
            showCancelButton: true,
            confirmButtonText: 'Update',
            preConfirm: (newName) => {

                if (!newName) {
                    Swal.showValidationMessage('Chapter name cannot be empty');
                    return false;
                }

                return $.ajax({
                    type: "POST",
                    url: "AllCourseContent.aspx/UpdateChapter",
                    data: JSON.stringify({
                        Chapter_Id: chapterId,
                        Chapter_Name: newName
                    }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json"
                });
            }
        }).then((result) => {

            if (result.isConfirmed) {

                Swal.fire({
                    icon: 'success',
                    title: 'Updated!',
                    timer: 1200,
                    showConfirmButton: false
                });

                $('#<%= drpCourses.ClientID %>').trigger("change");
        }

    });

    });

    // DELETE CHAPTER
    $(document).on("click", ".delete-btn", function () {

        var chapterId = $(this).data("id");

        Swal.fire({
            title: 'Are you sure?',
            text: "This chapter will be permanently deleted!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {

            if (result.isConfirmed) {

                $.ajax({
                    type: "POST",
                    url: "AllCourseContent.aspx/DeleteChapter",
                    data: JSON.stringify({ Chapter_Id: chapterId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    success: function () {

                        Swal.fire({
                            icon: 'success',
                            title: 'Deleted!',
                            timer: 1200,
                            showConfirmButton: false
                        });

                        $('#<%= drpCourses.ClientID %>').trigger("change");
                },

                error: function (err) {
                    console.log(err);
                }
            });

        }

    });

    });

    //Add Topic Button Click
    $(document).on('click', '.add-topic-btn', function (e) {

        e.preventDefault(); // important

        var button = $(this);
        var chapterId = button.data("chapter");
        var collapseDiv = $("#collapse" + chapterId);

        var row = button.closest("tr");

        var topicName = row.find(".topic-name-input").val().trim();
        var days = row.find(".topic-days-input").val().trim();

        if (topicName === "" || days === "") {
            Swal.fire({
                icon: 'warning',
                title: 'Missing Fields',
                text: 'Please enter topic name and days'
            });
            return;
        }

        button.prop("disabled", true); // prevent double click

        $.ajax({
            type: "POST",
            url: "AllCourseContent.aspx/AddTopic",
            data: JSON.stringify({
                Chapter_Id: parseInt(chapterId),
                Topic_Name: topicName,
                Days: parseInt(days)
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (response) {

                button.prop("disabled", false);

                if (response.d === "Success") {

                    // Clear input
                    row.find(".topic-name-input").val("");
                    row.find(".topic-days-input").val("");

                    // Keep collapse open
                    collapseDiv.collapse("show");

                    loadTopics(chapterId);

                    Swal.fire({
                        icon: 'success',
                        title: 'Topic Added',
                        text: 'Topic added successfully',
                        timer: 1500,
                        showConfirmButton: false
                    });
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.d
                    });
                }
            },

            error: function () {

                button.prop("disabled", false);

                Swal.fire({
                    icon: 'error',
                    title: 'Server Error',
                    text: 'Something went wrong'
                });
            }
        });

    });

    function loadTopics(chapterId) {

        var collapseDiv = $("#collapse" + chapterId);
        var topicContainer = collapseDiv.find(".topic-body");

        topicContainer.html("<tr><td colspan='3' class='text-center'>Loading...</td></tr>");

        $.ajax({
            type: "POST",
            url: "AllCourseContent.aspx/GetTopicsByChapterId",
            data: JSON.stringify({ Chapter_Id: parseInt(chapterId) }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (response) {

                var data = response.d;
                var topicRows = "";

                if (data && data.length > 0) {

                    $.each(data, function (i, item) {

                        topicRows += "<tr>";
                        topicRows += "<td>" + item.Topic_Name + "</td>";
                        topicRows += "<td>" + item.Days + "</td>";
                        topicRows += "<td class='text-center'>";

                        topicRows += "<a class='btn btn-sm btn-primary topic-edit-btn me-1' data-id='" + item.Topic_Id + "'>";
                        topicRows += "<i class='fa fa-edit'></i></a>";

                        topicRows += "<a class='btn btn-sm btn-danger topic-delete-btn' data-id='" + item.Topic_Id + "'>";
                        topicRows += "<i class='fa fa-trash'></i></a>";

                        topicRows += "</td>";
                        topicRows += "</tr>";
                    });

                } else {
                    topicRows = "<tr><td colspan='3' class='text-center'>No Topics Found</td></tr>";
                }

                topicContainer.html(topicRows);
            },

            error: function () {
                topicContainer.html("<tr><td colspan='3' class='text-danger text-center'>Error loading topics</td></tr>");
            }
        });
    }

    //Update Topic Button Click
    $(document).on('click', '.topic-edit-btn', function () {

        var topicId = $(this).data("id");
        var row = $(this).closest("tr");

        var currentName = row.find("td:eq(0)").text().trim();
        var currentDays = row.find("td:eq(1)").text().trim();

        Swal.fire({
            title: 'Edit Topic',
            html:
                '<input id="swalTopicName" class="swal2-input" value="' + currentName + '">' +
                '<input id="swalTopicDays" type="number" class="swal2-input" value="' + currentDays + '">',
            focusConfirm: false,
            preConfirm: () => {
                return {
                    name: document.getElementById('swalTopicName').value,
                    days: document.getElementById('swalTopicDays').value
                }
            }
        }).then((result) => {

            if (result.isConfirmed) {

                $.ajax({
                    type: "POST",
                    url: "AllCourseContent.aspx/UpdateTopic",
                    data: JSON.stringify({
                        Topic_Id: parseInt(topicId),
                        Topic_Name: result.value.name,
                        Days: parseInt(result.value.days)
                    }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    success: function (response) {

                        if (response.d === "Success") {

                            // Update row instantly
                            row.find("td:eq(0)").text(result.value.name);
                            row.find("td:eq(1)").text(result.value.days);

                            Swal.fire({
                                icon: 'success',
                                title: 'Updated',
                                timer: 1200,
                                showConfirmButton: false
                            });
                        }
                        else {
                            Swal.fire('Error', response.d, 'error');
                        }
                    }
                });
            }

        });

    });

    //Delete Topic Button Click
    $(document).on('click', '.topic-delete-btn', function () {

        var topicId = $(this).data("id");
        var row = $(this).closest("tr");

        Swal.fire({
            title: 'Delete Topic?',
            text: "This action cannot be undone",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it'
        }).then((result) => {

            if (result.isConfirmed) {

                $.ajax({
                    type: "POST",
                    url: "AllCourseContent.aspx/DeleteTopic",
                    data: JSON.stringify({ Topic_Id: parseInt(topicId) }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",

                    success: function (response) {

                        if (response.d === "Success") {

                            row.fadeOut(300, function () {
                                $(this).remove();
                            });

                            Swal.fire({
                                icon: 'success',
                                title: 'Deleted',
                                timer: 1200,
                                showConfirmButton: false
                            });
                        }
                        else {
                            Swal.fire('Error', response.d, 'error');
                        }
                    }
                });
            }

        });

    });
</script>
</asp:Content>
