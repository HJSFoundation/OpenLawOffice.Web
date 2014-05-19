﻿// -----------------------------------------------------------------------
// <copyright file="TagFilterSettingElement.cs" company="Nodine Legal, LLC">
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

namespace OpenLawOffice.WebClient
{
    using System.Configuration;

    public class TagFilterSettingElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("category", IsRequired = false)]
        public string Category
        {
            get { return (string)this["category"]; }
            set { this["category"] = value; }
        }

        [ConfigurationProperty("tag", IsRequired = true)]
        public string Tag
        {
            get { return (string)this["tag"]; }
            set { this["tag"] = value; }
        }

        public TagFilterSettingElement()
        {
        }

        public TagFilterSettingElement(string category, string tag)
        {
            Category = category;
            Tag = tag;
        }
    }
}