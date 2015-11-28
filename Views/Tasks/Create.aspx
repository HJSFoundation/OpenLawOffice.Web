﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" 
    Inherits="System.Web.Mvc.ViewPage<OpenLawOffice.Web.ViewModels.Tasks.CreateTaskViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Create Task
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="/Scripts/moment.min.js"></script>
    <script type="text/javascript" src="/Scripts/tinymce/tinymce.min.js"></script>

    <script language="javascript">
        var templates = <%= ViewData["TemplateJson"] %>;
        function taskTemplateClick(id) {
            for (var i = 0; i < templates.length; i++) {
                if (templates[i].Id == id) {
                    $("#Task_Title").val(templates[i].Title);
                    tinymce.activeEditor.setContent(templates[i].Title);
                    //$("#Task_Description").val(templates[i].Description);
                    if (templates[i].Active)
                        $("#Task_Active").prop('checked',true);
                    else
                        $("#Task_Active").prop('checked',false);
                    if (templates[i].ProjectedStart != null)
                        $("#Task_ProjectedStart").val(moment(templates[i].ProjectedStart).format("M/D/YYYY h:mm:ss A"));
                    else 
                        $("#Task_ProjectedStart").val(null);
                    if (templates[i].DueDate != null)
                        $("#Task_DueDate").val(moment(templates[i].DueDate).format("M/D/YYYY h:mm:ss A"));
                    else 
                        $("#Task_DueDate").val(null);
                    if (templates[i].ProjectedEnd != null)
                        $("#Task_ProjectedEnd").val(moment(templates[i].ProjectedEnd).format("M/D/YYYY h:mm:ss A"));
                    else 
                        $("#Task_ProjectedEnd").val(null);
                    if (templates[i].ActualEnd != null)
                        $("#Task_ActualEnd").val(moment(templates[i].ActualEnd).format("M/D/YYYY h:mm:ss A"));
                    else 
                        $("#Task_ActualEnd").val(null);
                }
            }
            return false;
        }
        $(document).ready(function () {
            tinymce.init({
                selector: "#Task_Description",
                theme: "modern",
                toolbar: "insertfile undo redo | styleselect | bold underline italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image"
            });
        });
    </script>
            
    <div id="roadmap">
        <div class="zero">Matter: [<%: Html.ActionLink((string)ViewData["Matter"], "Details", "Matters", new { id = ViewData["MatterId"] }, null) %>]</div>
        <div id="current" class="one">Create Task<a id="pageInfo" class="btn-question" style="padding-left: 15px;">Help</a></div>
    </div>
    
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <input type="hidden" id="MatterId" value="<%: Request["MatterId"].ToString() %>" />
    <table class="detail_table">
        <tr>
            <td class="display-label">
                Title<span class="required-field" title="Required Field">*</span>
            </td>
            <td class="display-field">
                <%: Html.TextBoxFor(model => model.Task.Title, new { @style = "width: 100%;" })%>
                <%: Html.ValidationMessageFor(model => model.Task.Title)%>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <% Model.TaskTemplates.ForEach(x =>
                    {
                        // Needs to build to the json being built to hold values
                        %> 
                [<a href="javascript:taskTemplateClick('<%: x.Id.Value %>')"><%: x.TaskTemplateTitle %></a>]
                <%
                    }); %>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Description<span class="required-field" title="Required Field">*</span>
            </td>
            <td class="display-field">
                <%: Html.TextAreaFor(model => model.Task.Description, new { style = "height: 300px; width: 100%;" })%>
                <%: Html.ValidationMessageFor(model => model.Task.Description)%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Active<span class="required-field" title="Required Field">*</span>
            </td>
            <td class="display-field">
                <%: Html.CheckBoxFor(model => model.Task.Active, new { Checked = true })%>
                Uncheck if the task is already completed
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Projected Start
            </td>
            <td class="display-field">
                <%: Html.EditorFor(model => model.Task.ProjectedStart)%>
                <%: Html.ValidationMessageFor(model => model.Task.ProjectedStart)%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Due Date
            </td>
            <td class="display-field">
                <%: Html.EditorFor(model => model.Task.DueDate)%>
                <%: Html.ValidationMessageFor(model => model.Task.DueDate)%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Projected End
            </td>
            <td class="display-field">
                <%: Html.EditorFor(model => model.Task.ProjectedEnd)%>
                <%: Html.ValidationMessageFor(model => model.Task.ProjectedEnd)%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Actual End
            </td>
            <td class="display-field">
                <%: Html.EditorFor(model => model.Task.ActualEnd)%>
                <%: Html.ValidationMessageFor(model => model.Task.ActualEnd)%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Responsible User
            </td>
            <td class="display-field">
                <%: Html.DropDownListFor(x => x.ResponsibleUser.User.PId,
                        new SelectList((IList)ViewData["UserList"], "PId", "Username"),
                        new { @size = 5, @style = "width: 100%" })%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Responsiblity<span class="required-field" title="Required Field">*</span>
            </td>
            <td class="display-field">
                <%: Html.TextBoxFor(model => model.ResponsibleUser.Responsibility, new { @style = "width: 100%;", @Value = "Lead" })%>
                <%: Html.ValidationMessageFor(model => model.ResponsibleUser.Responsibility)%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Assigned Contact
            </td>
            <td class="display-field">
                <%: Html.DropDownListFor(x => x.TaskContact.Contact.Id,
                        new SelectList((IList)ViewData["EmployeeContactList"], "Id", "DisplayName"),
                        new { @size = 5, @style = "width: 100%" })%>
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Assignment<span class="required-field" title="Required Field">*</span>
            </td>
            <td class="display-field">
                <%: Html.EnumDropDownListFor(model => model.TaskContact.AssignmentType) %>
            </td>
        </tr>
        
        <%--<tr>
            <td class="display-label">
                Group
            </td>
            <td class="display-field">
                <%: Html.CheckBoxFor(model => model.Task.IsGroupingTask)%>
                Would you like to make this task a grouping task? A grouping task is capable of
                having subtasks and will automatically calculate it's own projected start, end,
                due and actual end dates.
            </td>
        </tr>
        <tr>
            <td class="display-label">
                Parent
            </td>
            <td class="display-field">
                Parent:
                <%: Html.TextBoxFor(model => model.Task.Parent.Id, new { @readonly = true })%>
                <%: Html.ValidationMessageFor(model => model.Task.Parent.Id)%>
                <br />
                <br />
                <table id="parentlist">
                </table>
                <div id="parentpager">
                </div>
                <input id="parentclear" type="button" style="width: 200px;" value="clear" />
                <script language="javascript">
                    $(function () {
                        $("#parentlist").jqGrid({
                            treeGrid: true,
                            autowidth: true,
                            url: '/Tasks/ListChildrenJqGrid?MatterId=<%: ViewData["MatterId"].ToString() %>',
                            datatype: 'json',
                            jsonReader: {
                                root: 'Rows',
                                page: 'CurrentPage',
                                total: 'TotalRecords',
                                id: 'Id',
                                rows: 'Rows'
                            },
                            colNames: ['id', 'Title', 'Type', 'Due'],
                            colModel: [
                                { name: 'Id', width: 1, hidden: true, key: true },
                                { name: 'Title', width: 350 },
                                { name: 'Type', width: 250 },
                                { name: 'DueDate', width: 120, formatter: 'date' }
                            ],
                            pager: '#parentpager',
                            gridview: true,
                            treedatatype: 'json',
                            treeGridModel: 'adjacency',
                            ExpandColumn: 'Title',
                            caption: 'Parent Task',
                            onSelectRow: function (id) {
                                $("#Parent_Id").val(id);
                            }
                        });
                    });

                    $("#parentclear").click(function () {
                        $("#parentlist").jqGrid('resetSelection');
                        $("#Parent_Id").val(null);
                    });
                </script>
            </td>
        </tr>--%>
    </table>
    <p>
        <input type="submit" value="Save" />
    </p>
    <% } %>
    <div id="pageInfoDialog" title="Help">
        <p>
        <span style="font-weight: bold; text-decoration: underline;">Info:</span>
        Fill in the information on this page to create a new task.  Required fields are indicated with an
        <span style="color: #ee0000;font-size: 12px;cursor:help;" title="Required Field">*</span><br /><br />
        <span style="font-weight: bold; text-decoration: underline;">Usage:</span>
        Select a "parent" task to make this task be a "subtask" of another task.  To deselect a parent task, click "clear". 
        Fields marked with an <span style="color: #ee0000;font-size: 12px;cursor:help;" title="Required Field">*</span> are required.
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MenuContent" runat="server">    
    <li>
        <%: Html.ActionLink("Matter", "Details", "Matters", new { id = ViewData["MatterId"] }, null)%></li>
    <li>
        <%: Html.ActionLink("Tasks of Matter", "Tasks", "Matters", new { id = ViewData["MatterId"] }, null) %></li>
</asp:Content>