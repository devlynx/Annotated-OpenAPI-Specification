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

namespace PetStore
{
    using System.Linq;
    using System.Collections.Generic;
    using Swashbuckle.Swagger.Model;
    using Swashbuckle.SwaggerGen.Generator;
    using MicroserviceBoilerplate;
    using Newtonsoft.Json;
    using System;

    internal class ApplySwaggerDocumentModifications : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDocument, DocumentFilterContext context)
        {
            AddControllerTags(swaggerDocument);
            AddSchemaExamples(swaggerDocument, context);
        }

        private void AddControllerTags(SwaggerDocument swaggerDocument)
        {
            if (swaggerDocument.Tags == null)
                swaggerDocument.Tags = new List<Tag>();
            swaggerDocument.Tags.Add(new Tag() { Name = "pets", Description = "Everything about your Pets" });
            swaggerDocument.Tags.Add(new Tag() { Name = "store", Description = "Access to Petstore orders" });
            swaggerDocument.Tags.Add(new Tag() { Name = "users", Description = "Operations about users" });
        }
        private void AddSchemaExamples(SwaggerDocument swaggerDocument, DocumentFilterContext context)
        {
            PathItem pathItem = (from path in swaggerDocument.Paths
                                 where path.Key == "/pets"
                                 select path.Value).FirstOrDefault<PathItem>();
            if (pathItem != null && pathItem.Post != null)
            {
                Response invalidData = (from r in pathItem.Post.Responses
                                        where r.Key == "400"
                                        select r.Value).FirstOrDefault<Response>();

                List<Tuple<string, string>> data = null;
                data = new List<Tuple<string, string>>() {
                          new Tuple<string, string>("Name", "Name cannot be blank"),
                          new Tuple<string, string>("Name", "Name cannot contain whitespace characters only"),
                          new Tuple<string, string>("ID", "id must be in a GUID form")
                     };

                string jsend = JsonConvert.SerializeObject(new JSend<List<Tuple<string, string>>>()
                    .Error()
                    .Message("Request contained invalid data")
                    .Data(data));

                invalidData.Examples = data;
            }
        }
    }
}