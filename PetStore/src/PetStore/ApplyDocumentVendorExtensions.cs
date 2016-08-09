using Swashbuckle.SwaggerGen.Generator;
using Swashbuckle.Swagger.Model;
using System;
using System.Collections.Generic;

namespace PetStore
{
    internal class ApplyDocumentVendorExtensions : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<Tag>();
            swaggerDoc.Tags.Add(new Tag() { Name = "pets", Description = "Everything about your pets" });
            swaggerDoc.Info.Version = "1.0.7";
        }
    }
}