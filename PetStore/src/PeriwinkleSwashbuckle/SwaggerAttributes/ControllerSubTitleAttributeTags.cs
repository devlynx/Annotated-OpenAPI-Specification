using System;

namespace Periwinkle.Swashbuckle.SwaggerAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ControllerSubTitleAttribute : Attribute
    {
        public ControllerSubTitleAttribute(string subTitle)
        {
            SubTitle = subTitle;
        }

        public string SubTitle { get; set; }
    }
}
