using System.Reflection;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace Periwinkle.Swashbuckle
{
    public class ControllerSubTitleAnnotations : IDocumentFilter
    {
        private readonly Assembly controllerAssembly;

        public ControllerSubTitleAnnotations(Assembly controllerAssembly = null)
        {
            if (controllerAssembly == null)
                this.controllerAssembly = Assembly.GetEntryAssembly();
            else
                this.controllerAssembly = controllerAssembly;
        }
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            ApplyControllerSubTitleAttributes(swaggerDoc, context);
        }

        private static void ApplyControllerSubTitleAttributes(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            //var attribute = context.ApiDescription.GetActionAttributes()
            //    .OfType<ControllerSubTitleAttribute>()
            //    .FirstOrDefault();
            //if (attribute == null) return;



            //if (attribute.OperationId != null)
            //    operation.OperationId = attribute.OperationId;

            //if (attribute.Tags != null)
            //    operation.Tags = attribute.Tags;

            //if (attribute.Schemes != null)
            //    operation.Schemes = attribute.Schemes;
        }
    }
}
