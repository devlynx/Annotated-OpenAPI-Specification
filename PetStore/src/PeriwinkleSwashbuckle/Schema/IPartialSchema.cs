using System.Collections.Generic;
using Swashbuckle.Swagger.Model;

namespace Periwinkle.Swashbuckle
{
    public interface IPartialSchema
    {
        ParamCollectionFormat CollectionFormat { get; set; }
        object Default { get; set; }
        IList<object> Enum { get; set; }
        bool? ExclusiveMaximum { get; set; }
        bool? ExclusiveMinimum { get; set; }
        string Format { get; set; }
        PartialSchema Items { get; set; }
        int? Maximum { get; set; }
        int? MaxItems { get; set; }
        int? MaxLength { get; set; }
        int? Minimum { get; set; }
        int? MinItems { get; set; }
        int? MinLength { get; set; }
        int? MultipleOf { get; set; }
        string Pattern { get; set; }
        ParamType Type { get; set; }
        bool? UniqueItems { get; set; }
    }
}