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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MicroserviceBoilerplate;
    using Newtonsoft.Json;
    using Swashbuckle.Swagger.Model;
    using Swashbuckle.SwaggerGen.Generator;

    internal class ApplySwaggerDocumentModifications : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDocument, DocumentFilterContext context)
        {
            AddControllerTagDescriptions(swaggerDocument);

            // if you want to set the base path manually, use this...
            SetBasePath(swaggerDocument, "/v1");

            // response examples are not supported by Swagger-ui so we comment them out for now...
            // AddResponseExamples(swaggerDocument, context);
        }

        private void SetBasePath(SwaggerDocument swaggerDocument, string basePath)
        {
            swaggerDocument.BasePath = basePath;
        }

        private void AddControllerTagDescriptions(SwaggerDocument swaggerDocument)
        {
            if (swaggerDocument.Tags == null)
                swaggerDocument.Tags = new List<Tag>();
            swaggerDocument.Tags.Add(new Tag() { Name = "pets", Description = "Everything about your Pets" });
        }

        private void AddResponseExamples(SwaggerDocument swaggerDocument, DocumentFilterContext context)
        {
            PathItem pathItem = (from path in swaggerDocument.Paths
                                 where path.Key == "/pets"
                                 select path.Value).FirstOrDefault<PathItem>();
            if (pathItem != null && pathItem.Post != null)
            {
                Response invalidData = (from r in pathItem.Post.Responses
                                        where r.Key == "400"
                                        select r.Value).FirstOrDefault<Response>();

                if (invalidData != null)
                {
                    List<Tuple<string, string>> data = null;
                    data = new List<Tuple<string, string>>() {
                          new Tuple<string, string>("Name", "Name cannot be blank"),
                          new Tuple<string, string>("Name", "Name cannot contain only whitespace characters"),
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
}



//namespace PetStore
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using MicroserviceBoilerplate;
//    using Newtonsoft.Json;
//    using Swashbuckle.Swagger.Model;
//    using Swashbuckle.SwaggerGen.Generator;

//    internal class ApplySwaggerDocumentModifications : IDocumentFilter
//    {
//        public void Apply(SwaggerDocument swaggerDocument, DocumentFilterContext context)
//        {
//            AddControllerTagDescriptions(swaggerDocument);

//        }

//        private void AddControllerTagDescriptions(SwaggerDocument swaggerDocument)
//        {
//            if (swaggerDocument.Tags == null)
//                swaggerDocument.Tags = new List<Tag>();
//            swaggerDocument.Tags.Add(new Tag() { Name = "pets", Description = "Everything about your Pets" });
//        }
//    }
//}