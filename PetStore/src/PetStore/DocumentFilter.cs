using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace PetStore
{
    public class DocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
        }
    }
}
