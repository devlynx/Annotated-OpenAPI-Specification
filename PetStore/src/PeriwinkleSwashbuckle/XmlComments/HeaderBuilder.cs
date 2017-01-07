using Swashbuckle.SwaggerGen.Annotations;

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

                string headerType = headerNode.Current.GetAttribute("type", uri); // Required
                try
                {
                    header.Type = (ParamType)Enum.Parse(typeof(ParamType), headerType);
                }
                catch (Exception exception)
                {
                    throw new SwaggerHeaderValidationException("Type must be one of: string, number, integer, boolean, array", exception);
                }

                if (headerType == "array")
                {
                    string headerCollectionFormat = GetStringParamAttributeWithDescription("collectionFormat", ParamCollectionFormat.csv.ToString());
                    try
                    {
                        var x = Enum.Parse(typeof(ParamCollectionFormat), headerCollectionFormat);
                        header.CollectionFormat = (ParamCollectionFormat)Enum.Parse(typeof(ParamCollectionFormat), headerCollectionFormat);
                    }
                    catch (Exception exception)
                    {
                        throw new SwaggerHeaderValidationException("Collection Format must be one of: csv, ssv, tsv, pipes", exception);
                    }

                    //    // TODO: finish items...
                    //    header.Items = null;
                }

                if (header.required == null || !header.required.Value)
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
            return value == null ? false : value;
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

        public string GetStringParamAttributeWithDescription(string attributeName, string defaultValue = default(string))
        {
            string rawValue = headerNode.Current.GetAttribute(attributeName, uri);

            string value = String.IsNullOrWhiteSpace(rawValue) ? defaultValue : rawValue;

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
    }
}
