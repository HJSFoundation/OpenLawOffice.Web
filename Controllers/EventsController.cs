﻿// -----------------------------------------------------------------------
// <copyright file="EventsController.cs" company="Nodine Legal, LLC">
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
    using System.Web.Mvc;
    using AutoMapper;
    using System.Collections.Generic;
    using System.Web.Profile;
    using System.Web.Security;

    // NOT MAINTAINED
    public class EventsController : BaseController
    {
        [Authorize(Roles = "Login, User")]
        public ActionResult SelectUser()
        {
            List<ViewModels.Account.UsersViewModel> usersList;

            usersList = new List<ViewModels.Account.UsersViewModel>();

            Data.Account.Users.List().ForEach(x =>
            {
                usersList.Add(Mapper.Map<ViewModels.Account.UsersViewModel>(x));
            });

            return View(usersList);
        } 

        [Authorize(Roles = "Login, User")]
        public ActionResult SelectContact()
        {
            List<ViewModels.Contacts.ContactViewModel> contactList;

            contactList = new List<ViewModels.Contacts.ContactViewModel>();

            Data.Contacts.Contact.List().ForEach(x =>
            {
                contactList.Add(Mapper.Map<ViewModels.Contacts.ContactViewModel>(x));
            });

            return View(contactList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult ContactAgenda(int id)
        {
            List<ViewModels.Events.EventViewModel> list;
            Common.Models.Contacts.Contact contact;

            contact = Data.Contacts.Contact.Get(id);

            list = new List<ViewModels.Events.EventViewModel>();

            Data.Events.Event.ListForContact(id, DateTime.Now, null).ForEach(x =>
            {
                list.Add(Mapper.Map<ViewModels.Events.EventViewModel>(x));
            });

            ViewData["ContactDisplayName"] = Mapper.Map<ViewModels.Contacts.ContactViewModel>(contact).DisplayName;

            return View(list);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult UserAgenda()
        {
            Guid userId;
            int contactId;
            List<ViewModels.Events.EventViewModel> list;
            Common.Models.Account.Users user;


            list = new List<ViewModels.Events.EventViewModel>();

            if (RouteData.Values["Id"] != null)
            {
                userId = Guid.Parse((string)RouteData.Values["Id"]);

                Data.Events.Event.ListForUser(userId, DateTime.Now, null).ForEach(x =>
                {
                    list.Add(Mapper.Map<ViewModels.Events.EventViewModel>(x));
                });

                user = Data.Account.Users.Get(userId);
            }
            else
            {
                userId = (Guid)Membership.GetUser().ProviderUserKey;

                user = Data.Account.Users.Get(userId);

                dynamic profile = ProfileBase.Create(Membership.GetUser(userId).UserName);
                contactId = int.Parse(profile.ContactId);

                Data.Events.Event.ListForUserAndContact(userId, contactId, DateTime.Now, null).ForEach(x =>
                {
                    list.Add(Mapper.Map<ViewModels.Events.EventViewModel>(x));
                });
            }

            ViewData["Username"] = Mapper.Map<ViewModels.Account.UsersViewModel>(user).Username;

            return View(list);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Create(ViewModels.Events.EventViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Events.Event model;

            currentUser = Data.Account.Users.Get(User.Identity.Name);

            model = Mapper.Map<Common.Models.Events.Event>(viewModel);

            model = Data.Events.Event.Create(model, currentUser);

            if (Request["MatterId"] != null)
            {
                Guid matterId = Guid.Parse(Request["MatterId"]);
                Data.Events.Event.RelateToMatter(model, matterId, currentUser);
            }

            if (Request["TaskId"] != null)
            {
                long taskId = long.Parse(Request["TaskId"]);
                Data.Events.Event.RelateToTask(model, taskId, currentUser);
            }

            return RedirectToAction("Details", new { Id = model.Id });
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(Guid id)
        {
            ViewModels.Events.EventViewModel viewModel;

            viewModel = Mapper.Map<ViewModels.Events.EventViewModel>(Data.Events.Event.Get(id));

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(Guid id, ViewModels.Events.EventViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Events.Event model;

            currentUser = Data.Account.Users.Get(User.Identity.Name);

            model = Mapper.Map<Common.Models.Events.Event>(viewModel);

            model = Data.Events.Event.Edit(model, currentUser);

            return RedirectToAction("Details", new { Id = id });
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Details(Guid id)
        {
            ViewModels.Events.EventViewModel viewModel;

            viewModel = Mapper.Map<ViewModels.Events.EventViewModel>(Data.Events.Event.Get(id));
            
            PopulateCoreDetails(viewModel);

            return View(viewModel);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Contacts(Guid id)
        {
            List<ViewModels.Events.EventAssignedContactViewModel> list;

            list = new List<ViewModels.Events.EventAssignedContactViewModel>();

            Data.Events.EventAssignedContact.ListForEvent(id).ForEach(x =>
            {
                ViewModels.Events.EventAssignedContactViewModel vm = Mapper.Map<ViewModels.Events.EventAssignedContactViewModel>(x);
                vm.Contact = Mapper.Map<ViewModels.Contacts.ContactViewModel>(Data.Contacts.Contact.Get(x.Contact.Id.Value));
                list.Add(vm);
            });

            return View(list);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Matters(Guid id)
        {
            List<ViewModels.Matters.MatterViewModel> list;

            list = new List<ViewModels.Matters.MatterViewModel>();

            Data.Events.EventMatter.ListForEvent(id).ForEach(x =>
            {
                ViewModels.Matters.MatterViewModel vm = Mapper.Map<ViewModels.Matters.MatterViewModel>(x);
                list.Add(vm);
            });

            return View(list);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Tasks(Guid id)
        {
            List<ViewModels.Tasks.TaskViewModel> list;

            list = new List<ViewModels.Tasks.TaskViewModel>();

            Data.Events.EventTask.ListForEvent(id).ForEach(x =>
            {
                ViewModels.Tasks.TaskViewModel vm = Mapper.Map<ViewModels.Tasks.TaskViewModel>(x);
                list.Add(vm);
            });

            return View(list);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Notes(Guid id)
        {
            List<ViewModels.Notes.NoteViewModel> list;

            list = new List<ViewModels.Notes.NoteViewModel>();

            Data.Events.EventNote.ListForEvent(id).ForEach(x =>
            {
                ViewModels.Notes.NoteViewModel vm = Mapper.Map<ViewModels.Notes.NoteViewModel>(x);
                list.Add(vm);
            });

            return View(list);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Tags(Guid id)
        {
            List<ViewModels.Events.EventTagViewModel> list;

            list = new List<ViewModels.Events.EventTagViewModel>();

            Data.Events.EventTag.ListForEvent(id).ForEach(x =>
            {
                ViewModels.Events.EventTagViewModel vm = Mapper.Map<ViewModels.Events.EventTagViewModel>(x);
                list.Add(vm);
            });

            return View(list);
        }
    }
}