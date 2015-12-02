﻿// -----------------------------------------------------------------------
// <copyright file="TasksController.cs" company="Nodine Legal, LLC">
// Licensed to Nodine Legal, LLC under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  Nodine Legal, LLC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>
// -----------------------------------------------------------------------

namespace OpenLawOffice.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using AutoMapper;
    using System.Configuration;
    using System.Web.Profile;
    using System.Web.Security;

    [HandleError(View = "Errors/Index", Order = 10)]
    public class TasksController : BaseController
    {
        //[HttpGet]
        //[Authorize(Roles = "Login, User")]
        //public ActionResult ListChildrenJqGrid(long? id)
        //{
        //    List<ViewModels.Tasks.TaskViewModel> modelList;
        //    ViewModels.JqGridObject jqObject;
        //    List<object> anonList;
        //    int level = 0;

        //    if (id == null)
        //    {
        //        // jqGrid uses nodeid by default
        //        if (!string.IsNullOrEmpty(Request["nodeid"]))
        //            id = long.Parse(Request["nodeid"]);
        //    }

        //    anonList = new List<object>();

        //    if (!string.IsNullOrEmpty(Request["n_level"]))
        //        level = int.Parse(Request["n_level"]) + 1;

        //    if (!id.HasValue)
        //    {
        //        string matterid = Request["MatterId"];
        //        if (string.IsNullOrEmpty(matterid))
        //            modelList = GetList();
        //        else
        //            modelList = GetListForMatter(Guid.Parse(matterid));
        //    }
        //    else
        //    {
        //        modelList = GetChildrenList(id.Value);
        //    }

        //    modelList.ForEach(x =>
        //    {
        //        if (x.IsGroupingTask)
        //        {
        //            // isLeaf = false
        //            anonList.Add(new
        //            {
        //                Id = x.Id,
        //                Title = x.Title,
        //                Type = x.Type,
        //                DueDate = x.DueDate,
        //                Description = x.Description,
        //                level = level,
        //                isLeaf = false,
        //                expanded = false
        //            });
        //        }
        //        else
        //        {
        //            // isLeaf = true
        //            anonList.Add(new
        //            {
        //                Id = x.Id,
        //                Title = x.Title,
        //                Type = x.Type,
        //                DueDate = x.DueDate,
        //                Description = x.Description,
        //                level = level,
        //                isLeaf = true,
        //                expanded = false
        //            });
        //        }
        //    });

        //    jqObject = new ViewModels.JqGridObject()
        //    {
        //        TotalPages = 1,
        //        CurrentPage = 1,
        //        TotalRecords = modelList.Count,
        //        Rows = anonList.ToArray()
        //    };

        //    return Json(jqObject, JsonRequestBehavior.AllowGet);
        //}

        public static List<ViewModels.Tasks.TaskViewModel> GetChildrenList(long id)
        {
            List<ViewModels.Tasks.TaskViewModel> viewModelList;

            viewModelList = new List<ViewModels.Tasks.TaskViewModel>();
            ViewModels.Tasks.TaskViewModel viewModel;

            Data.Tasks.Task.ListChildren(id).ForEach(x =>
            {
                viewModel = Mapper.Map<ViewModels.Tasks.TaskViewModel>(x);

                if (viewModel.IsGroupingTask)
                {
                    if (Data.Tasks.Task.GetTaskForWhichIAmTheSequentialPredecessor(x.Id.Value) != null)
                        viewModel.Type = "Sequential Group";
                    else
                        viewModel.Type = "Group";
                }
                else
                {
                    if (x.SequentialPredecessor != null)
                        viewModel.Type = "Sequential";
                    else
                        viewModel.Type = "Standard";
                }

                viewModelList.Add(viewModel);
            });

            return viewModelList;
        }

        public static List<ViewModels.Tasks.TaskViewModel> GetListForMatter(Guid matterid, bool? active)
        {
            List<ViewModels.Tasks.TaskViewModel> viewModelList;
            ViewModels.Tasks.TaskViewModel viewModel;

            viewModelList = new List<ViewModels.Tasks.TaskViewModel>();

            Data.Tasks.Task.ListForMatter(matterid, active).ForEach(x =>
            {
                viewModel = Mapper.Map<ViewModels.Tasks.TaskViewModel>(x);

                if (viewModel.IsGroupingTask)
                {
                    if (Data.Tasks.Task.GetTaskForWhichIAmTheSequentialPredecessor(x.Id.Value)
                        != null)
                        viewModel.Type = "Sequential Group";
                    else
                        viewModel.Type = "Group";
                }
                else
                {
                    if (x.SequentialPredecessor != null)
                        viewModel.Type = "Sequential";
                    else
                        viewModel.Type = "Standard";
                }

                viewModelList.Add(viewModel);
            });

            return viewModelList;
        }

        public static List<ViewModels.Tasks.TaskViewModel> GetList()
        {
            List<ViewModels.Tasks.TaskViewModel> viewModelList;
            ViewModels.Tasks.TaskViewModel viewModel;

            viewModelList = new List<ViewModels.Tasks.TaskViewModel>();

            Data.Tasks.Task.List().ForEach(x =>
            {
                viewModel = Mapper.Map<ViewModels.Tasks.TaskViewModel>(x);

                if (viewModel.IsGroupingTask)
                {
                    if (Data.Tasks.Task.GetTaskForWhichIAmTheSequentialPredecessor(x.Id.Value)
                        != null)
                        viewModel.Type = "Sequential Group";
                    else
                        viewModel.Type = "Group";
                }
                else
                {
                    if (x.SequentialPredecessor != null)
                        viewModel.Type = "Sequential";
                    else
                        viewModel.Type = "Standard";
                }

                viewModelList.Add(viewModel);
            });

            return viewModelList;
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Details(long id)
        {
            ViewModels.Tasks.TaskViewModel viewModel;
            Common.Models.Tasks.Task model;
            Common.Models.Matters.Matter matter;

            model = Data.Tasks.Task.Get(id);

            viewModel = Mapper.Map<ViewModels.Tasks.TaskViewModel>(model);
            
            viewModel.Notes = new List<ViewModels.Notes.NoteViewModel>();
            Data.Notes.NoteTask.ListForTask(id).ForEach(x =>
            {
                viewModel.Notes.Add(Mapper.Map<ViewModels.Notes.NoteViewModel>(x));
            });

            PopulateCoreDetails(viewModel);

            if (model.Parent != null && model.Parent.Id.HasValue)
            {
                model.Parent = Data.Tasks.Task.Get(model.Parent.Id.Value);
                viewModel.Parent = Mapper.Map<ViewModels.Tasks.TaskViewModel>(model.Parent);
            }

            if (model.SequentialPredecessor != null && model.SequentialPredecessor.Id.HasValue)
            {
                model.SequentialPredecessor = Data.Tasks.Task.Get(model.SequentialPredecessor.Id.Value);
                viewModel.SequentialPredecessor = Mapper.Map<ViewModels.Tasks.TaskViewModel>(model.SequentialPredecessor);
            }

            matter = Data.Tasks.Task.GetRelatedMatter(model.Id.Value);

            ViewBag.MatterId = matter.Id.Value;
            ViewBag.Matter = matter.Title;

            return View(viewModel);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(long id)
        {
            ViewModels.Tasks.TaskViewModel viewModel;
            Common.Models.Tasks.Task model;
            Common.Models.Matters.Matter matter;

            model = Data.Tasks.Task.Get(id);
            viewModel = Mapper.Map<ViewModels.Tasks.TaskViewModel>(model);

            if (model.Parent != null && model.Parent.Id.HasValue)
            {
                model.Parent = Data.Tasks.Task.Get(model.Parent.Id.Value);
                viewModel.Parent = Mapper.Map<ViewModels.Tasks.TaskViewModel>(model.Parent);
            }

            if (model.SequentialPredecessor != null && model.SequentialPredecessor.Id.HasValue)
            {
                model.SequentialPredecessor = Data.Tasks.Task.Get(model.SequentialPredecessor.Id.Value);
                viewModel.SequentialPredecessor = Mapper.Map<ViewModels.Tasks.TaskViewModel>(model.SequentialPredecessor);
            }

            matter = Data.Tasks.Task.GetRelatedMatter(model.Id.Value);

            ViewBag.MatterId = matter.Id.Value;
            ViewBag.Matter = matter.Title;

            return View(viewModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(long id, ViewModels.Tasks.TaskViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Tasks.Task model;
            Common.Models.Matters.Matter matterModel;
            Common.Models.Tasks.Task currentModel;

            currentUser = Data.Account.Users.Get(User.Identity.Name);

            model = Mapper.Map<Common.Models.Tasks.Task>(viewModel);

            matterModel = Data.Tasks.Task.GetRelatedMatter(id);

            currentModel = Data.Tasks.Task.Get(id);

            if (model.Parent != null && model.Parent.Id.HasValue)
            {
                if (model.Parent.Id.Value == model.Id.Value)
                {
                    //  Task is trying to set itself as its parent
                    ModelState.AddModelError("Parent.Id", "Parent cannot be the task itself.");

                    ViewBag.MatterId = matterModel.Id.Value;
                    ViewBag.Matter = matterModel.Title;

                    return View(viewModel);
                }
            }

            model.Description = new Ganss.XSS.HtmlSanitizer().Sanitize(model.Description);
            model = Data.Tasks.Task.Edit(model, currentUser);

            return RedirectToAction("Details", new { Id = id });
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Close(long id)
        {
            return Close(id, null);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Close(long id, ViewModels.Tasks.TaskViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Tasks.Task model;

            currentUser = Data.Account.Users.Get(User.Identity.Name);

            model = Data.Tasks.Task.Get(id);
            model.Active = false;
            model.ActualEnd = DateTime.Now;

            model = Data.Tasks.Task.Edit(model, currentUser);

            if (!string.IsNullOrEmpty(Request["NewTask"]) && Request["NewTask"] == "on")
            { // not empty & "on"
                Common.Models.Matters.Matter matter = Data.Tasks.Task.GetRelatedMatter(id);
                return RedirectToAction("Create", "Tasks", new { MatterId = matter.Id });
            }

            return RedirectToAction("Details", new { Id = id });
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Create()
        {
            int contactId = -1;
            ViewModels.Tasks.CreateTaskViewModel viewModel;
            Common.Models.Matters.Matter matter;
            List<ViewModels.Account.UsersViewModel> userList;
            List<ViewModels.Contacts.ContactViewModel> employeeContactList;
            Newtonsoft.Json.Linq.JArray taskTemplates;

            userList = new List<ViewModels.Account.UsersViewModel>();
            employeeContactList = new List<ViewModels.Contacts.ContactViewModel>();
            
            dynamic profile = ProfileBase.Create(Membership.GetUser().UserName);
            if (profile.ContactId != null && !string.IsNullOrEmpty(profile.ContactId))
                contactId = int.Parse(profile.ContactId);

            Data.Account.Users.List().ForEach(x =>
            {
                userList.Add(Mapper.Map<ViewModels.Account.UsersViewModel>(x));
            });

            Data.Contacts.Contact.ListEmployeesOnly().ForEach(x =>
            {
                employeeContactList.Add(Mapper.Map<ViewModels.Contacts.ContactViewModel>(x));
            });

            viewModel = new ViewModels.Tasks.CreateTaskViewModel();
            viewModel.TaskTemplates = new List<ViewModels.Tasks.TaskTemplateViewModel>();
            taskTemplates = new Newtonsoft.Json.Linq.JArray();
            Data.Tasks.TaskTemplate.List().ForEach(x =>
            {
                viewModel.TaskTemplates.Add(Mapper.Map<ViewModels.Tasks.TaskTemplateViewModel>(x));
                Newtonsoft.Json.Linq.JObject template = new Newtonsoft.Json.Linq.JObject();
                template.Add(new Newtonsoft.Json.Linq.JProperty("Id", x.Id.Value));
                template.Add(new Newtonsoft.Json.Linq.JProperty("TaskTemplateTitle", x.TaskTemplateTitle));
                template.Add(new Newtonsoft.Json.Linq.JProperty("Title", x.Title));
                template.Add(new Newtonsoft.Json.Linq.JProperty("Description", x.Description));
                template.Add(new Newtonsoft.Json.Linq.JProperty("ProjectedStart", DTProp(x.ProjectedStart)));
                template.Add(new Newtonsoft.Json.Linq.JProperty("DueDate", DTProp(x.DueDate)));
                template.Add(new Newtonsoft.Json.Linq.JProperty("ProjectedEnd", DTProp(x.ProjectedEnd)));
                template.Add(new Newtonsoft.Json.Linq.JProperty("ActualEnd", DTProp(x.ActualEnd)));
                template.Add(new Newtonsoft.Json.Linq.JProperty("Active", x.Active));
                taskTemplates.Add(template);
            });
            
            viewModel.ResponsibleUser = new ViewModels.Tasks.TaskResponsibleUserViewModel()
            {
                User = new ViewModels.Account.UsersViewModel() { PId =  Data.Account.Users.Get(Membership.GetUser().UserName).PId },
            };
            if (contactId > 0)
            {
                viewModel.TaskContact = new ViewModels.Tasks.TaskAssignedContactViewModel()
                {
                    Contact = new ViewModels.Contacts.ContactViewModel()
                    {
                        Id = contactId
                    }
                };
            }


            matter = Data.Matters.Matter.Get(Guid.Parse(Request["MatterId"]));
            ViewBag.MatterId = matter.Id.Value;
            ViewBag.Matter = matter.Title;
            ViewBag.UserList = userList;
            ViewBag.EmployeeContactList = employeeContactList;
            ViewBag.TemplateJson = taskTemplates.ToString();

            return View(new ViewModels.Tasks.CreateTaskViewModel()
            {
                TaskTemplates = viewModel.TaskTemplates,
                ResponsibleUser = viewModel.ResponsibleUser,                
                TaskContact = new ViewModels.Tasks.TaskAssignedContactViewModel()
                {
                    AssignmentType = ViewModels.AssignmentTypeViewModel.Direct,
                    Contact = viewModel.TaskContact.Contact
                }
            });
        }

        private string DTProp(string val)
        {
            if (string.IsNullOrEmpty(val)) return null;

            if (val.Contains("[NOW]"))
                val = val.Replace("[NOW]", DateTime.Now.ToString("M/d/yyyy h:mm tt"));
            if (val.Contains("[DATE]"))
                val = val.Replace("[DATE]", DateTime.Now.ToString("M/d/yyyy"));
            if (val.Contains("[DATE+"))
            {
                int num = -1;
                try
                {
                    // abc[DATE+12]asd
                    string a = val.Substring(0, val.IndexOf("[DATE+"));
                    // a=abc
                    string b = val.Substring(val.IndexOf("[DATE+") + 6);
                    // b=12]asd
                    string c = b.Substring(0, b.IndexOf("]"));
                    // c=12
                    string d = b.Substring(b.IndexOf("]") + 1);
                    // d=asd
                    num = int.Parse(c);
                    val = a + DateTime.Now.AddDays(num).ToString("M/d/yyyy") + d;
                }
                catch
                {
                    num = -1;
                    val = "Invalid Format";
                }
            }

            return val;
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Create(ViewModels.Tasks.CreateTaskViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Tasks.Task model;
            Guid matterid = Guid.Empty;

            currentUser = Data.Account.Users.Get(User.Identity.Name);

            model = Mapper.Map<Common.Models.Tasks.Task>(viewModel.Task);
            model.Description = new Ganss.XSS.HtmlSanitizer().Sanitize(model.Description);
            model = Data.Tasks.Task.Create(model, currentUser);

            matterid = Guid.Parse(Request["MatterId"]);

            Data.Tasks.Task.RelateMatter(model, matterid, currentUser);
            Data.Tasks.TaskResponsibleUser.Create(new Common.Models.Tasks.TaskResponsibleUser()
            {
                Task = model,
                User = new Common.Models.Account.Users() { PId = viewModel.ResponsibleUser.User.PId },
                Responsibility = viewModel.ResponsibleUser.Responsibility
            }, currentUser);
            Data.Tasks.TaskAssignedContact.Create(new Common.Models.Tasks.TaskAssignedContact()
            {
                Task = model,
                Contact = new Common.Models.Contacts.Contact() { Id = viewModel.TaskContact.Contact.Id },
                AssignmentType = (Common.Models.Tasks.AssignmentType)(int)viewModel.TaskContact.AssignmentType
            }, currentUser);

            return RedirectToAction("Details", new { Id = model.Id });
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult TodoListForAll()
        {
            DateTime? start = null;
            DateTime? stop = null;
            List<dynamic> jsonList;
            List<Common.Models.Settings.TagFilter> tagFilters;
            if (Request["start"] != null)
                start = Common.Utilities.UnixTimeStampToDateTime(double.Parse(Request["start"]));
            if (Request["stop"] != null)
                stop = Common.Utilities.UnixTimeStampToDateTime(double.Parse(Request["stop"]));

            tagFilters = Common.Settings.Manager.Instance.System.GlobalTaskTagFilters.ToUserSettingsModel();

            jsonList = new List<dynamic>();

            Data.Tasks.Task.GetTodoListForAll(tagFilters, start, stop).ForEach(x =>
            {
                if (x.DueDate.HasValue)
                {
                    jsonList.Add(new
                    {
                        id = x.Id.Value,
                        title = x.Title,
                        allDay = true,
                        start = Common.Utilities.DateTimeToUnixTimestamp(x.DueDate.Value.ToLocalTime()),
                        description = x.Description
                    });
                }
            });

            return Json(jsonList, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult TodoListForUser(Guid? id)
        {
            DateTime? start = null;
            DateTime? stop = null;
            List<dynamic> jsonList;
            Common.Models.Account.Users user;
            List<Common.Models.Tasks.Task> taskList;
            List<Common.Models.Settings.TagFilter> tagFilter;
            if (Request["start"] != null)
                start = Common.Utilities.UnixTimeStampToDateTime(double.Parse(Request["start"]));
            if (Request["stop"] != null)
                stop = Common.Utilities.UnixTimeStampToDateTime(double.Parse(Request["stop"]));

            if (!id.HasValue)
            {
                if (string.IsNullOrEmpty(Request["UserPId"]))
                    return null;
                else
                    id = Guid.Parse(Request["UserPId"]);
            }

            user = Data.Account.Users.Get(id.Value);

            tagFilter = Data.Settings.UserTaskSettings.ListTagFiltersFor(user);

            jsonList = new List<dynamic>();

            if (Request["ContactId"] == null || string.IsNullOrEmpty(Request["ContactId"]))
                taskList = Data.Tasks.Task.GetTodoListFor(user, tagFilter, start, stop);
            else
            {
                int contactId = int.Parse(Request["ContactId"]);
                taskList = Data.Tasks.Task.GetTodoListFor(user, 
                    new Common.Models.Contacts.Contact() { Id = contactId }, tagFilter, start, stop);
            }

            taskList.ForEach(x =>
            {
                if (x.DueDate.HasValue)
                {
                    jsonList.Add(new
                    {
                        id = x.Id.Value,
                        title = x.Title,
                        allDay = true,
                        start = Common.Utilities.DateTimeToUnixTimestamp(x.DueDate.Value.ToLocalTime()),
                        description = x.Description
                    });
                }
            });

            return Json(jsonList, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult TodoListForContact(int? id)
        {
            DateTime? start = null;
            DateTime? stop = null;
            List<dynamic> jsonList;
            Common.Models.Contacts.Contact contact;
            List<Common.Models.Settings.TagFilter> tagFilters;
            if (Request["start"] != null)
                start = Common.Utilities.UnixTimeStampToDateTime(double.Parse(Request["start"]));
            if (Request["stop"] != null)
                stop = Common.Utilities.UnixTimeStampToDateTime(double.Parse(Request["stop"]));

            if (!id.HasValue)
            {
                if (string.IsNullOrEmpty(Request["ContactId"]))
                    return null;
                else
                    id = int.Parse(Request["ContactId"]);
            }

            contact = Data.Contacts.Contact.Get(id.Value);

            tagFilters = Common.Settings.Manager.Instance.System.GlobalTaskTagFilters.ToUserSettingsModel();

            jsonList = new List<dynamic>();

            Data.Tasks.Task.GetTodoListFor(contact, tagFilters, start, stop).ForEach(x =>
            {
                if (x.DueDate.HasValue)
                {
                    jsonList.Add(new
                    {
                        id = x.Id.Value,
                        title = x.Title,
                        allDay = true,
                        start = Common.Utilities.DateTimeToUnixTimestamp(x.DueDate.Value.ToLocalTime()),
                        description = x.Description
                    });
                }
            });

            return Json(jsonList, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Time(long id)
        {
            List<ViewModels.Timing.TimeViewModel> viewModelList;
            ViewModels.Timing.TimeViewModel viewModel;
            Common.Models.Contacts.Contact contact;
            Common.Models.Tasks.Task task;
            Common.Models.Matters.Matter matter;

            viewModelList = new List<ViewModels.Timing.TimeViewModel>();

            Data.Timing.Time.ListForTask(id).ForEach(x =>
            {
                viewModel = Mapper.Map<ViewModels.Timing.TimeViewModel>(x);

                contact = Data.Contacts.Contact.Get(viewModel.Worker.Id.Value);

                viewModel.Worker = Mapper.Map<ViewModels.Contacts.ContactViewModel>(contact);
                viewModel.WorkerDisplayName = viewModel.Worker.DisplayName;

                viewModelList.Add(viewModel);
            });

            task = Data.Tasks.Task.Get(id);
            matter = Data.Tasks.Task.GetRelatedMatter(id);
            ViewBag.Task = task.Title;
            ViewBag.TaskId = task.Id;
            ViewBag.Matter = matter.Title;
            ViewBag.MatterId = matter.Id;

            return View(viewModelList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Contacts(long id)
        {
            List<ViewModels.Tasks.TaskAssignedContactViewModel> viewModelList;
            ViewModels.Tasks.TaskAssignedContactViewModel viewModel;
            Common.Models.Contacts.Contact contact;
            Common.Models.Tasks.Task task;
            Common.Models.Matters.Matter matter;

            viewModelList = new List<ViewModels.Tasks.TaskAssignedContactViewModel>();

            Data.Tasks.TaskAssignedContact.ListForTask(id).ForEach(x =>
            {
                viewModel = Mapper.Map<ViewModels.Tasks.TaskAssignedContactViewModel>(x);

                contact = Data.Contacts.Contact.Get(x.Contact.Id.Value);

                viewModel.Contact = Mapper.Map<ViewModels.Contacts.ContactViewModel>(contact);

                viewModelList.Add(viewModel);
            });

            task = Data.Tasks.Task.Get(id);
            matter = Data.Tasks.Task.GetRelatedMatter(id);
            ViewBag.Task = task.Title;
            ViewBag.TaskId = task.Id;
            ViewBag.Matter = matter.Title;
            ViewBag.MatterId = matter.Id;

            return View(viewModelList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Tags(long id)
        {
            Common.Models.Tasks.Task task;
            Common.Models.Matters.Matter matter;
            List<ViewModels.Tasks.TaskTagViewModel> viewModelList;

            viewModelList = new List<ViewModels.Tasks.TaskTagViewModel>();

            Data.Tasks.TaskTag.ListForTask(id).ForEach(x =>
            {
                viewModelList.Add(Mapper.Map<ViewModels.Tasks.TaskTagViewModel>(x));
            });

            task = Data.Tasks.Task.Get(id);
            matter = Data.Tasks.Task.GetRelatedMatter(id);
            ViewBag.Task = task.Title;
            ViewBag.TaskId = task.Id;
            ViewBag.Matter = matter.Title;
            ViewBag.MatterId = matter.Id;

            return View(viewModelList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult ResponsibleUsers(long id)
        {
            Common.Models.Tasks.Task task;
            Common.Models.Matters.Matter matter;
            List<ViewModels.Tasks.TaskResponsibleUserViewModel> viewModelList;
            Common.Models.Account.Users user;
            ViewModels.Tasks.TaskResponsibleUserViewModel viewModel;

            viewModelList = new List<ViewModels.Tasks.TaskResponsibleUserViewModel>();

            Data.Tasks.TaskResponsibleUser.ListForTask(id).ForEach(x =>
            {
                user = Data.Account.Users.Get(x.User.PId.Value);

                viewModel = Mapper.Map<ViewModels.Tasks.TaskResponsibleUserViewModel>(x);

                viewModel.User = Mapper.Map<ViewModels.Account.UsersViewModel>(user);

                viewModelList.Add(viewModel);
            });

            task = Data.Tasks.Task.Get(id);
            matter = Data.Tasks.Task.GetRelatedMatter(id);
            ViewBag.Task = task.Title;
            ViewBag.TaskId = task.Id;
            ViewBag.Matter = matter.Title;
            ViewBag.MatterId = matter.Id;

            return View(viewModelList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Notes(long id)
        {
            Common.Models.Tasks.Task task;
            Common.Models.Matters.Matter matter;
            List<ViewModels.Notes.NoteViewModel> viewModelList;
            ViewModels.Notes.NoteViewModel viewModel;

            viewModelList = new List<ViewModels.Notes.NoteViewModel>();

            Data.Notes.Note.ListForTask(id).ForEach(x =>
            {
                viewModel = Mapper.Map<ViewModels.Notes.NoteViewModel>(x);

                viewModelList.Add(viewModel);
            });

            task = Data.Tasks.Task.Get(id);
            matter = Data.Tasks.Task.GetRelatedMatter(id);
            ViewBag.Task = task.Title;
            ViewBag.TaskId = task.Id;
            ViewBag.Matter = matter.Title;
            ViewBag.MatterId = matter.Id;

            return View(viewModelList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Documents(long id)
        {
            Common.Models.Tasks.Task task;
            Common.Models.Matters.Matter matter;
            Common.Models.Documents.Version version;
            List<ViewModels.Documents.DocumentViewModel> viewModelList;
            ViewModels.Documents.DocumentViewModel viewModel;

            viewModelList = new List<ViewModels.Documents.DocumentViewModel>();

            Data.Documents.Document.ListForTask(id).ForEach(x =>
            {
                version = Data.Documents.Document.GetCurrentVersion(x.Id.Value);
                viewModel = Mapper.Map<ViewModels.Documents.DocumentViewModel>(x);
                viewModel.Extension = version.Extension;

                viewModelList.Add(viewModel);
            });

            task = Data.Tasks.Task.Get(id);
            matter = Data.Tasks.Task.GetRelatedMatter(id);
            ViewBag.Task = task.Title;
            ViewBag.TaskId = task.Id;
            ViewBag.Matter = matter.Title;
            ViewBag.MatterId = matter.Id;

            return View(viewModelList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Events(long id)
        {
            Common.Models.Tasks.Task task;
            Common.Models.Matters.Matter matter;
            ViewModels.Events.EventViewModel viewModel;
            List<ViewModels.Events.EventViewModel> viewModelList;

            viewModelList = new List<ViewModels.Events.EventViewModel>();

            Data.Events.Event.ListForTask(id).ForEach(x =>
            {
                viewModel = Mapper.Map<ViewModels.Events.EventViewModel>(x);

                viewModelList.Add(viewModel);
            });

            task = Data.Tasks.Task.Get(id);
            matter = Data.Tasks.Task.GetRelatedMatter(id);
            ViewBag.Task = task.Title;
            ViewBag.TaskId = task.Id;
            ViewBag.Matter = matter.Title;
            ViewBag.MatterId = matter.Id;

            return View(viewModelList);
        }

        #region Commented Out Sequential Predecessor Code

        //public static DBOs.Tasks.Task GetSequentialPredecessor(DBOs.Tasks.Task taskDbo, IDbConnection db)
        //{
        //    return db.Single<DBOs.Tasks.Task>(
        //        "SELECT * FROM \"task\" WHERE \"id\"=@SeqPredId AND \"utc_disabled\" is null",
        //        new { SeqPredId = taskDbo.SequentialPredecessorId });
        //}

        //public static string InsertTaskIntoSequence(DBOs.Tasks.Task taskToInsertDbo, long idOfPredecessor, IDbConnection db)
        //{
        //    DBOs.Tasks.Task nextTask = null, lastTask = null;

        //    // This handles if a member is selected
        //    DBOs.Tasks.Task groupingTaskDbo = GetParentTask(idOfPredecessor, db);

        //    // What if a member is not selected, but the sequential grouping task itself is selected?
        //    if (groupingTaskDbo == null)
        //    {
        //        groupingTaskDbo = db.SingleById<DBOs.Tasks.Task>(idOfPredecessor);
        //        if (!groupingTaskDbo.IsGroupingTask)
        //            return "Predecessor must be either a sequence member or grouping sequence.";
        //    }

        //    // Update the edited dbo
        //    taskToInsertDbo.SequentialPredecessorId = idOfPredecessor;
        //    taskToInsertDbo.ParentId = groupingTaskDbo.Id;
        //    db.UpdateOnly(taskToInsertDbo,
        //        fields => new
        //        {
        //            fields.Title,
        //            fields.Description,
        //            fields.ProjectedStart,
        //            fields.DueDate,
        //            fields.ProjectedEnd,
        //            fields.ActualEnd,
        //            fields.ParentId,
        //            fields.ModifiedByUserId,
        //            fields.UtcModified
        //        },
        //        where => where.Id == taskToInsertDbo.Id);

        //    DBOs.Tasks.Task tempTask;

        //    // Load task currently in the position
        //    tempTask = db.Single<DBOs.Tasks.Task>(
        //        "SELECT * FROM \"task\" WHERE \"sequential_predecessor_id\"=@Id AND \"utc_disabled\" is null",
        //        new { Id = idOfPredecessor });

        //    // We can improve this later - this layout helps with debugging
        //    if (tempTask != null)
        //    {
        //        // Update the seqpredid of taskToInsert
        //        db.UpdateOnly(new DBOs.Tasks.Task()
        //            {
        //                SequentialPredecessorId = idOfPredecessor
        //            },
        //            fields => new
        //            {
        //                fields.SequentialPredecessorId
        //            },
        //            where => where.Id == taskToInsertDbo.Id);

        //        // Load the next task to update into memory
        //        nextTask = db.Single<DBOs.Tasks.Task>(
        //            "SELECT * FROM \"task\" WHERE \"sequential_predecessor_id\"=@Id AND \"utc_disabled\" is null",
        //            new { Id = tempTask.Id });

        //        // Update tempTask's seqpred
        //        db.UpdateOnly(new DBOs.Tasks.Task()
        //            {
        //                SequentialPredecessorId = taskToInsertDbo.Id
        //            },
        //            fields => new
        //            {
        //                fields.SequentialPredecessorId
        //            },
        //            where => where.Id == tempTask.Id);

        //        while (nextTask != null)
        //        {
        //            // Make next task be temp task
        //            tempTask = nextTask;

        //            // Load new next task
        //            nextTask = db.Single<DBOs.Tasks.Task>(
        //                "SELECT * FROM \"task\" WHERE \"sequential_predecessor_id\"=@Id AND \"utc_disabled\" is null",
        //                new { Id = tempTask.Id });

        //            // Update tempTask's seqpred
        //            db.UpdateOnly(new DBOs.Tasks.Task()
        //                {
        //                    SequentialPredecessorId = taskToInsertDbo.Id
        //                },
        //                fields => new
        //                {
        //                    fields.SequentialPredecessorId
        //                },
        //                where => where.Id == tempTask.Id);
        //        }
        //    }
        //    else
        //    {
        //        db.UpdateOnly(new DBOs.Tasks.Task()
        //            {
        //                SequentialPredecessorId = idOfPredecessor
        //            },
        //            fields => new
        //            {
        //                fields.SequentialPredecessorId
        //            },
        //            where => where.Id == taskToInsertDbo.Id);
        //    }

        //    // Update group properties
        //    UpdateGroupingTaskProperties(groupingTaskDbo, db);
        //}

        //public static void RelocateTaskInSequence(DBOs.Tasks.Task taskToRelocate, long idOfPredecessor, IDbConnection db)
        //{
        //    // Below can be improved, but it will work for the time being.
        //    RemoveTaskFromSequence(taskToRelocate, db);
        //    InsertTaskIntoSequence(taskToRelocate, idOfPredecessor, db);
        //}

        //public static void RemoveTaskFromSequence(DBOs.Tasks.Task taskToRemove, IDbConnection db)
        //{
        //    // Remove taskToRelocate from its current sequence
        //    //   Set taskToRelocate.ParentId to null (yes, this must push to the db so the query will work right)
        //    //   Query for nextTask (being the task with taskToRelocate as its sequential predecessor)
        //    //   Update it to taskToRelocate's sequential predecessor
        //    //   Cascade this query and update down the chain to move all tasks after taskToRelocate up one position

        //    DBOs.Tasks.Task nextTask = null, lastTask = null;

        //    DBOs.Tasks.Task groupingTaskDbo = null;
        //    long? parentIdHolder = taskToRemove.ParentId;

        //    taskToRemove.ParentId = null;
        //    db.UpdateOnly(taskToRemove,
        //        fields => new
        //        {
        //            fields.ParentId
        //        },
        //        where => where.Id == taskToRemove.Id);

        //    nextTask = db.Single<DBOs.Tasks.Task>(
        //        "SELECT * FROM \"task\" WHERE \"sequential_predecessor_id\"=@Id AND \"utc_disabled\" is null",
        //        new { Id = taskToRemove.Id });

        //    lastTask = taskToRemove;

        //    while (nextTask != null)
        //    {
        //        nextTask.SequentialPredecessorId = lastTask.SequentialPredecessorId;

        //        db.UpdateOnly(nextTask,
        //            fields => new
        //            {
        //                fields.SequentialPredecessorId
        //            },
        //            where => where.Id == nextTask.Id);

        //        lastTask = nextTask;
        //        nextTask = db.Single<DBOs.Tasks.Task>(
        //            "SELECT * FROM \"task\" WHERE \"sequential_predecessor_id\"=@Id AND \"utc_disabled\" is null",
        //            new { Id = nextTask.Id });
        //    }

        //    db.Delete<DBOs.Tasks.Task>(taskToRemove);

        //    // Update group properties
        //    if (parentIdHolder.HasValue)
        //    {
        //        if ((groupingTaskDbo = GetParentTask(parentIdHolder.Value, db)) != null)
        //            UpdateGroupingTaskProperties(groupingTaskDbo, db);
        //    }
        //}

        #endregion Commented Out Sequential Predecessor Code
    }
}