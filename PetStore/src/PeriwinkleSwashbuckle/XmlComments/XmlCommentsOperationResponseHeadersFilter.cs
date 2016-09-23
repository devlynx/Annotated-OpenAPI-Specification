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
// limitations under the License.
#endregion

using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Annotations;
using Swashbuckle.SwaggerGen.Generator;

namespace Periwinkle.Swashbuckle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.XPath;
    using Mapster;
    using Microsoft.AspNetCore.Mvc.Controllers;

    public class XmlCommentsOperationResponseHeadersFilter : IOperationFilter
    {
        private const string ResponseHeaderName = "httpResponseHeader";
        private const string HttpResponseHeaderXPath = ResponseHeaderName + "[@name='{0}' type='{1}' required='{2}']";

        private const string GlobalResponseHeaderName = "globalHttpResponseHeader";
        private const string GlobalResponseHttpHeaderXPath = GlobalResponseHeaderName + "[@name='{0}' type='{1}' required='{2}']";

        private const string MemberXPath = "/doc/members/member[@name='{0}']";

        private readonly XPathNavigator _xmlNavigator;

        public XmlCommentsOperationResponseHeadersFilter(string xmlDocPath, bool allowDuplicates = true, bool throwExceptions = true)
        {
            var xmlDoc = new XPathDocument(xmlDocPath);
            _xmlNavigator = xmlDoc.CreateNavigator();
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null) return;

            ApplyGlobalHeaderComments(operation, controllerActionDescriptor);
            ApplyOperationHeaderComments(operation, controllerActionDescriptor);
        }

        private void ApplyOperationHeaderComments(Operation operation, ControllerActionDescriptor controllerActionDescriptor)
        {
            var commentId = XmlCommentsIdHelper.GetCommentIdForMethod(controllerActionDescriptor.MethodInfo);
            var commentNode = _xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            ApplyHeaderComments(operation, commentNode, ResponseHeaderName);
        }

        private void ApplyGlobalHeaderComments(Operation operation, ControllerActionDescriptor controllerActionDescriptor)
        {
            var commentId = XmlCommentsIdHelper.GetCommentIdForType(controllerActionDescriptor.ControllerTypeInfo.UnderlyingSystemType);
            var commentNode = _xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            ApplyHeaderComments(operation, commentNode, GlobalResponseHeaderName);
        }

        private static void ApplyHeaderComments(Operation operation, XPathNavigator commentNode, string headerName)
        {
            XPathNodeIterator paramNode = commentNode.SelectChildren(headerName, string.Empty);
            for (int i = 0; i < paramNode.Count; i++)
            {
                paramNode.MoveNext();
                AddHeaderToOperation(operation, paramNode);
            }
        }

        private static bool HeaderParamValid(Func<string, bool> isValid, string value)
        {
            return isValid(value);
        }

        private static void AddHeaderToOperation(Operation operation, XPathNodeIterator paramNode)
        {

            string uri = string.Empty; // the URI is always local for the header xml comments.

            if (paramNode.Current == null)
                return;

            string httpHeaderName = paramNode.Current.GetAttribute("name", uri);
            PartialSchema partialSchema = new PartialSchema();

            if (String.IsNullOrWhiteSpace(httpHeaderName))
            {
                HeaderParamValidationError(operation, new NonBodyParameter(), "Parameter name must not be blank.");
                return;
            }

            IEnumerable<IParameter> duplicateParams =
                from p in operation.Parameters
                where p.In == "header" && p.Name.ToLower() == httpHeaderName.ToLower()
                select p;

            if (duplicateParams.Count<IParameter>() == 0)
            {
                bool headerRequired = paramNode.Current.GetAttribute("required", uri).ToLower() == bool.TrueString.ToLower();

                StringBuilder description = new StringBuilder();

                description.AppendLine(XmlCommentsTextHelper.Humanize(paramNode.Current.InnerXml));

                partialSchema.Type = paramNode.Current.GetAttribute("type", uri); // Required
                if (!HeaderParamValid((v => new List<string>()
                        { "string", "number", "integer", "boolean", "array" }.Contains(v)), partialSchema.Type))
                {
                    HeaderParamValidationError(operation, new NonBodyParameter(),
                        "Type must be one of: string, number, integer, boolean, array");
                    return;
                }

                if (partialSchema.Type == "array")
                {
                    partialSchema.CollectionFormat = GetStringParamAttributeWithDescription("collectionFormat", paramNode, description, uri, "csv");
                    if (!HeaderParamValid((v => new List<string>()
                        { "csv", "ssv", "tsv", "pipes" }.Contains(v)), partialSchema.CollectionFormat))
                    {
                        HeaderParamValidationError(operation, new NonBodyParameter(),
                            "collectionFormat must be one of: csv, ssv, tsv, pipes");
                        return;
                    }

                    // TODO: finish items...
                    partialSchema.Items = null;
                }

                if (!headerRequired)
                {
                    partialSchema.Default = GetStringParamAttributeWithDescription("default", paramNode, description, uri);
                }

                partialSchema.Format = GetStringParamAttributeWithDescription("format", paramNode, description, uri);
                partialSchema.Maximum = GetIntParamAttributeWithDescription("maximum", paramNode, description, uri);
                partialSchema.ExclusiveMaximum = GetBoolParamAttributeWithDescription("exclusiveMaximum", paramNode, description, uri);
                partialSchema.Minimum = GetIntParamAttributeWithDescription("minimum", paramNode, description, uri);
                partialSchema.ExclusiveMinimum = GetBoolParamAttributeWithDescription("exclusiveMinimum", paramNode, description, uri);
                partialSchema.MaxLength = GetIntParamAttributeWithDescription("maxLength", paramNode, description, uri);
                partialSchema.MinLength = GetIntParamAttributeWithDescription("minLength", paramNode, description, uri);
                partialSchema.Pattern = GetStringParamAttributeWithDescription("pattern", paramNode, description, uri);
                partialSchema.MaxItems = GetIntParamAttributeWithDescription("maxItems", paramNode, description, uri);
                partialSchema.MinItems = GetIntParamAttributeWithDescription("minItems", paramNode, description, uri);
                partialSchema.UniqueItems = GetBoolParamAttributeWithDescription("uniqueItems", paramNode, description, uri);
                // TODO: implement Enum Attribute
                //headerParam.Enum = GetListParamAttributeWithDescription("enum", paramNode, description, uri);
                partialSchema.MultipleOf = GetIntParamAttributeWithDescription("multipleOf", paramNode, description, uri);

                Header header = TypeAdapter.Adapt<Header>(partialSchema);
                
                //partialSchema.Description = description.ToString();
                //partialSchema.In = "header";
                //partialSchema.Name = httpHeaderName;
                //partialSchema.Required = headerRequired;

                string attributeCode = paramNode.Current.GetAttribute("code", uri);
                Response response = operation.Responses[attributeCode];
                if (response == null)
                    return;
                if (response.Headers == null)
                    response.Headers = new Dictionary<string, Header>();
                var x = from name in response.Headers.Keys select name;
                if (x.Count<string>() == 0)
                {
                    header.Type = paramNode.Current.GetAttribute("type", uri);
                    header.Description = description.ToString();
                    response.Headers.Add(httpHeaderName, header);
                }
            }
        }

        private static bool? GetBoolParamAttributeWithDescription(string attributeName, XPathNodeIterator paramNode, StringBuilder description, string uri, string defaultValue = null)
        {
            string rawValue = paramNode.Current.GetAttribute(attributeName, uri);
            bool? value = default(bool?);

            if (!String.IsNullOrWhiteSpace(rawValue))
            {
                value = Convert.ToBoolean(rawValue);
            }

            BuildDescription(attributeName, description, rawValue, value);
            return value;
        }

        private static int? GetIntParamAttributeWithDescription(string attributeName, XPathNodeIterator paramNode, StringBuilder description, string uri, string defaultValue = null)
        {
            string rawValue = paramNode.Current.GetAttribute(attributeName, uri);
            int? value = default(int?);

            if (!String.IsNullOrWhiteSpace(rawValue))
            {
                value = Convert.ToInt32(rawValue);
            }

            BuildDescription(attributeName, description, rawValue, value);
            return value;
        }
        private static string GetStringParamAttributeWithDescription(string attributeName, XPathNodeIterator paramNode, StringBuilder description, string uri, string defaultValue = null)
        {
            string rawValue = paramNode.Current.GetAttribute(attributeName, uri);
            string value = default(string);

            if (!String.IsNullOrWhiteSpace(rawValue))
            {
                value = rawValue;
            }

            BuildDescription(attributeName, description, rawValue, value);
            return value;
        }

        private static void BuildDescription<T>(string attributeName, StringBuilder description, string rawValue, T value)
        {
            if (!String.IsNullOrWhiteSpace(rawValue))
            {
                description.AppendLine();
                description.AppendLine();
                description.AppendFormat("{0} = {1}", attributeName, value);
            }
        }

        private static void HeaderParamValidationError(Operation operation, NonBodyParameter headerParam, string whatsWrong)
        {
            headerParam.Required = true;
            headerParam.Name = "Swagger Validation Error";
            headerParam.Description = whatsWrong;
            operation.Parameters.Add(headerParam);
        }
    }
}