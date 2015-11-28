﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<OpenLawOffice.Web.ViewModels.Notes.NoteViewModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Task Notes
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div id="roadmap">
        <div class="zero">Matter: [<%: Html.ActionLink((string)ViewData["Matter"], "Details", "Matters", new { id = ViewData["MatterId"] }, null) %>]</div>
        <div class="one">Task: [<%: Html.ActionLink((string)ViewData["Task"], "Details", "Tasks", new { id = ViewData["TaskId"] }, null) %>]</div>
        <div id="current" class="two">Task Notes<a id="pageInfo" class="btn-question" style="padding-left: 15px;">Help</a></div>
    </div>
    
    <table class="listing_table">
        <tr>
            <th>
                Title
            </th>
            <th>
                Body
            </th>
            <th style="width: 20px;">
            </th>
        </tr>
        <% bool altRow = true; 
           foreach (var item in Model)
           { 
               altRow = !altRow;
               if (altRow)
               { %> <tr class="tr_alternate"> <% }
               else
               { %> <tr> <% } %>
            <td>
                <%: Html.ActionLink(item.Title, "Details", "Notes", new { id = item.Id }, null)%>
            </td>
            <td>
                <% if (item.Body.Length > 100)
                   {
                %>
                <%: item.Body.Substring(0, 100)%>...
                <% }
                   else
                   { %>
                <%: item.Body%>
                <% } %>
            </td>
            <td>
                <%: Html.ActionLink("Edit", "Edit", "Notes", new { id = item.Id }, new { @class = "btn-edit", title = "Edit" })%>
            </td>
        </tr>
        <% } %>
    </table>
    
    <div id="pageInfoDialog" title="Help">
        <p>
        <span style="font-weight: bold; text-decoration: underline;">Info:</span>
        Notes allow users to make comments or record information on tasks.<br /><br />
        <span style="font-weight: bold; text-decoration: underline;">Usage:</span>
        Clicking the title will show the details of the note.  Click the 
        <img src="../../Content/fugue-icons-3.5.6/icons-shadowless/pencil.png" /> (edit icon) to make 
        changes to the note.
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MenuContent" runat="server">
    <li>Actions</li>
    <ul style="list-style: none outside none; padding-left: 1em;">
        <li>
            <%: Html.ActionLink("New Note", "Create", "Notes", new { TaskId = RouteData.Values["Id"].ToString() }, null)%></li>
    </ul>
    <li>
        <%: Html.ActionLink("Task", "Details", "Tasks", new { Id  = RouteData.Values["Id"].ToString() }, null)%></li>
</asp:Content>