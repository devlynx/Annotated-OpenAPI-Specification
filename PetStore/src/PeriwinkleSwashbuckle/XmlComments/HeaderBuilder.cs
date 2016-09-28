using System;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Annotations;
using Swashbuckle.SwaggerGen.Generator;

namespace Periwinkle.Swashbuckle
{
    using System;
    using System.Text;
    using System.Xml.XPath;

    public class HeaderBuilder
    {
        private StringBuilder description = new StringBuilder();
        private bool buildDescription;
        private string uri;
        private XPathNodeIterator headerNode;

        public HeaderBuilder(string uri = "", bool buildDescription = true)
        {
            this.uri = uri;
            this.buildDescription = buildDescription;
        }

        public PartialHeader GetHeader(XPathNodeIterator headerNode)
        {
            this.headerNode = headerNode;
            PartialHeader header;
            try
            {
                header = new PartialHeader();
                header.name = headerNode.Current.GetAttribute("name", uri);
                header.required = GetBoolAttribute("required");

                description.AppendLine(XmlCommentsTextHelper.Humanize(headerNode.Current.InnerXml));

                header.Type = headerNode.Current.GetAttribute("type", uri); // Required
                //if (!HeaderParamValid((v => new List<string>()
                //        { "string", "number", "integer", "boolean", "array" }.Contains(v)), header.Type))
                //{
                //    HeaderParamValidationError(operation, new NonBodyParameter(),
                //        "Type must be one of: string, number, integer, boolean, array");
                //    return;
                //}

                //if (partialSchema.Type == "array")
                //{
                //    partialSchema.CollectionFormat = GetStringParamAttributeWithDescription("collectionFormat", paramNode, description, uri, "csv");
                //    if (!HeaderParamValid((v => new List<string>()
                //        { "csv", "ssv", "tsv", "pipes" }.Contains(v)), header.CollectionFormat))
                //    {
                //        HeaderParamValidationError(operation, new NonBodyParameter(),
                //            "collectionFormat must be one of: csv, ssv, tsv, pipes");
                //        return;
                //    }

                //    // TODO: finish items...
                //    header.Items = null;
                //}

                if (header.required != null && header.required.Value)
                {
                    header.Default = GetStringParamAttributeWithDescription("default");
                }

                header.Format = GetStringParamAttributeWithDescription("format");
                header.Maximum = GetIntParamAttributeWithDescription("maximum");
                header.ExclusiveMaximum = GetBoolAttributeWithDescription("exclusiveMaximum");
                header.Minimum = GetIntParamAttributeWithDescription("minimum");
                header.ExclusiveMinimum = GetBoolAttributeWithDescription("exclusiveMinimum");
                header.MaxLength = GetIntParamAttributeWithDescription("maxLength");
                header.MinLength = GetIntParamAttributeWithDescription("minLength");
                header.Pattern = GetStringParamAttributeWithDescription("pattern");
                header.MaxItems = GetIntParamAttributeWithDescription("maxItems");
                header.MinItems = GetIntParamAttributeWithDescription("minItems");
                header.UniqueItems = GetBoolAttributeWithDescription("uniqueItems");
                // TODO: implement Enum Attribute
                //header.Enum = GetListParamAttributeWithDescription("enum", headerNode, description, uri);
                header.MultipleOf = GetIntParamAttributeWithDescription("multipleOf");
            }
            finally
            {
                this.headerNode = null;
            }
            return header;
        }

        public bool? GetBoolAttributeWithDescription(string attributeName)
        {
            bool? value = GetBoolAttribute(attributeName);

            BuildDescription(attributeName, headerNode.Current.GetAttribute(attributeName, uri), value);
            return value;
        }

        public bool? GetBoolAttribute(string attributeName)
        {
            string rawValue = headerNode.Current.GetAttribute(attributeName, uri);
            bool? value = default(bool?);

            if (!String.IsNullOrWhiteSpace(rawValue))
            {
                value = Convert.ToBoolean(rawValue);
            }
            return value;
        }

        public int? GetIntParamAttributeWithDescription(string attributeName)
        {
            string rawValue = headerNode.Current.GetAttribute(attributeName, uri);
            int? value = default(int?);

            if (!String.IsNullOrWhiteSpace(rawValue))
            {
                value = Convert.ToInt32(rawValue);
            }

            BuildDescription(attributeName, rawValue, value);
            return value;
        }

        public string GetStringParamAttributeWithDescription(string attributeName)
        {
            string rawValue = headerNode.Current.GetAttribute(attributeName, uri);
            string value = default(string);

            if (!String.IsNullOrWhiteSpace(rawValue))
            {
                value = rawValue;
            }

            BuildDescription(attributeName, rawValue, value);
            return value;
        }

        public override string ToString()
        {
            return description.ToString();
        }

        private void BuildDescription<T>(string attributeName, string rawValue, T value)
        {
            if (!String.IsNullOrWhiteSpace(rawValue))
            {
                description.AppendLine();
                description.AppendLine();
                description.AppendFormat("{0} = {1}", attributeName, value);
            }
        }

        internal void AppendLine(string v)
        {
            throw new NotImplementedException();
        }
    }
}
