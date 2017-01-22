using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Periwinkle.Swashbuckle.SwaggerAttributes;

namespace Periwinkle.Swashbuckle.SwaggerAttributeFilters
{
    public class AttributesControllerSubTitles : IControllerSubTitles
    {
        public string GetControllerSubTitle(Type controllerType)
        {
            ControllerSubTitleAttribute subTitleAttribute =
                (from a in controllerType.GetTypeInfo().GetCustomAttributes<ControllerSubTitleAttribute>()
                select a).FirstOrDefault<ControllerSubTitleAttribute>();
            if (subTitleAttribute == null)
                return null;
            else
                return subTitleAttribute.SubTitle;
        }
    }
}
