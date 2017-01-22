#region Copyright (c) 2016-2017 Periwinkle Software Limited
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
using Swashbuckle.SwaggerGen.Generator;

namespace Periwinkle.Swashbuckle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc;

    public class ControllerSubTitleTags : IDocumentFilter
    {
        private readonly Assembly controllerAssembly;

        private readonly IControllerSubTitles controllerSubTitles;
        private readonly IXmlCommentTools xmlCommentTools;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerSubTitleTags"/> class.
        /// </summary>
        /// <param name="controllerSubTitles">IControllerSubTitles implementation which obtains the subtitle text.</param>
        /// <param name="controllerAssembly">The controller assembly.</param>
        public ControllerSubTitleTags(IControllerSubTitles controllerSubTitles, Assembly controllerAssembly = null)
        {
            if (controllerAssembly == null)
                this.controllerAssembly = Assembly.GetEntryAssembly();
            else
                this.controllerAssembly = controllerAssembly;
            this.controllerSubTitles = controllerSubTitles; 
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
                string subTitle = controllerSubTitles.GetControllerSubTitle(type);
                swaggerDocument.Tags.Add(new Tag() { Name = GetDefaultControllerName(type.Name), Description = subTitle });
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
