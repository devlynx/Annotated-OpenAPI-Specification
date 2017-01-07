using System;
using System.Collections.Generic;
using Swashbuckle.Swagger.Model;

namespace Periwinkle.Swashbuckle
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SwaggerRequestHeaderAttribute : Attribute, IPartialSchema
    {
        public SwaggerRequestHeaderAttribute(string name, ParamType type)
        {
            Required = false;
            Name = name;
            Type = type;
        }

        public bool Required { get; set; }

        public string Name { get; set; }

        public ParamCollectionFormat CollectionFormat { get; set; }

        public object Default { get; set; }

        public IList<object> Enum { get; set; }

        public bool? ExclusiveMaximum { get; set; }

        public bool? ExclusiveMinimum { get; set; }

        public string Format { get; set; }

        public PartialSchema Items { get; set; }

        public int? Maximum { get; set; }

        public int? MaxItems { get; set; }

        public int? MaxLength { get; set; }

        public int? Minimum { get; set; }

        public int? MinItems { get; set; }

        public int? MinLength { get; set; }

        public int? MultipleOf { get; set; }

        public string Pattern { get; set; }

        public ParamType Type { get; set; }

        public bool? UniqueItems { get; set; }
    }
}
