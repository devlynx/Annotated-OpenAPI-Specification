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

namespace Periwinkle.Swashbuckle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.XPath;
    using Microsoft.AspNetCore.Mvc;

    public class XmlCommentsControllerSubTitleTags : IDocumentFilter
    {
        private readonly XPathNavigator xmlNavigator;
        private readonly Assembly controllerAssembly;
        private const string MemberXPath = "/doc/members/member[@name='{0}']";
        private const string SubTitleXPath = "controllerSubTitle";

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlCommentsControllerSubTitleTags"/> class.
        /// </summary>
        /// <param name="xmlDocPath">The XML document path.</param>
        /// <param name="controllerAssembly">The assembly containing the controller to document.
        /// If the controllerAssembly is null it will use Assembly.GetEntryAssembly().</param>
        public XmlCommentsControllerSubTitleTags(string xmlDocPath, Assembly controllerAssembly = null)
        {
            XPathDocument xmlDoc = new XPathDocument(xmlDocPath);
            xmlNavigator = xmlDoc.CreateNavigator();
            if (controllerAssembly == null)
                this.controllerAssembly = Assembly.GetEntryAssembly();
            else
                this.controllerAssembly = controllerAssembly;
        }

        public void Apply(SwaggerDocument swaggerDocument, DocumentFilterContext context)
        {
            AddControllerTagDescriptions(swaggerDocument, context);
        }

        private void AddControllerTagDescriptions(SwaggerDocument swaggerDocument, DocumentFilterContext context)
        {
            string uri = String.Empty;

            IEnumerable<Type> controllers = from g in context.ApiDescriptionsGroups.Items
                from type in controllerAssembly.GetTypes()
                                            where typeof(Controller).IsAssignableFrom(type)
                                            //&& type.Name == context.ApiDescriptionsGroups.
                                            select type;

            if (controllers.Count<Type>() == 0)
                return;

            if (swaggerDocument.Tags == null)
                swaggerDocument.Tags = new List<Tag>();

            foreach (Type type in controllers)
            {
                var commentId = XmlCommentsIdHelper.GetCommentIdForType(type);
                var commentNode = xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));

                if (commentNode == null)
                    continue;

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
