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
// limitations under the License.#endregion
#endregion

namespace PetStore
{
    using Swashbuckle.SwaggerGen.Generator;
    using Swashbuckle.Swagger.Model;
    using System;
    using System.Collections.Generic;

    internal class ApplySwaggerDocumentModifications : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDocument, DocumentFilterContext context)
        {
            AddControllerTags(swaggerDocument);
            SetInfoVersion(swaggerDocument);
        }

        private void SetInfoVersion(SwaggerDocument swaggerDocument)
        {
            swaggerDocument.Info.Version = "1.0.7";
        }

        private void AddControllerTags(SwaggerDocument swaggerDocument)
        {
            if (swaggerDocument.Tags == null)
                swaggerDocument.Tags = new List<Tag>();
            swaggerDocument.Tags.Add(new Tag() { Name = "pets", Description = "Everything about your Pets" });
        }
    }
}