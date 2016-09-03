#region Copyright (c) 2016 Periwinkle Software Limited
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Annotations;
using Swashbuckle.SwaggerGen.Generator;

namespace PetStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.XPath;
    using Microsoft.AspNetCore.Mvc;

    internal class ControllerSubTitleTags : IDocumentFilter
    {
        private readonly XPathNavigator _xmlNavigator;
        private const string MemberXPath = "/doc/members/member[@name='{0}']";
        private const string SubTitleXPath = "controllerSubTitle";

        public ControllerSubTitleTags(string xmlDocPath)
        {
            var xmlDoc = new XPathDocument(xmlDocPath);
            _xmlNavigator = xmlDoc.CreateNavigator();
        }

        public void Apply(SwaggerDocument swaggerDocument, DocumentFilterContext context)
        {
            AddControllerTagDescriptions(swaggerDocument);
        }

        private void AddControllerTagDescriptions(SwaggerDocument swaggerDocument)
        {
            string uri = String.Empty;

            var controllers = from t in Assembly.GetEntryAssembly().GetTypes()
                              where typeof(Controller).IsAssignableFrom(t)
                              select t;
            if (controllers.Count<Type>() == 0)
                return;

            if (swaggerDocument.Tags == null)
                swaggerDocument.Tags = new List<Tag>();

            foreach (Type type in controllers)
            {
                var commentId = XmlCommentsIdHelper.GetCommentIdForType(type);
                var commentNode = _xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));
                var subtitleNode = commentNode.SelectSingleNode(SubTitleXPath);

                if (subtitleNode == null)
                    continue;

                string subTitleName = subtitleNode.GetAttribute("name", uri);

                string controllerName = !String.IsNullOrWhiteSpace(subTitleName) ? subTitleName : GetDefaultControllerName(type.Name);

                swaggerDocument.Tags.Add(new Tag() { Name = controllerName, Description = XmlCommentsTextHelper.Humanize(subtitleNode.InnerXml) });
            }
        }

        private string GetDefaultControllerName(string name)
        {
            const string suffix = "controller";
            if (name.ToLower().EndsWith(suffix))
                return name.Remove(name.Length - suffix.Length);
            else
                return name;
        }
    }
}
