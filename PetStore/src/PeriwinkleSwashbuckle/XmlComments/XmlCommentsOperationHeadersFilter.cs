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
    using System.Reflection;
    using System.Text;
    using System.Xml.XPath;
    using Microsoft.AspNetCore.Http.Headers;
    using Microsoft.AspNetCore.Mvc.Controllers;

    public class XmlCommentsOperationHeadersFilter : IOperationFilter
    {
        private const string RequestHeaderName = "httpRequestHeader";
        private const string HttpRequestHeaderXPath = RequestHeaderName + "[@name='{0}' type='{1}' required='{2}']";

        private const string GlobalRequestHeaderName = "globalHttpRequestHeader";
        private const string GlobalRequestHttpHeaderXPath = GlobalRequestHeaderName + "[@name='{0}' type='{1}' required='{2}']";

        private const string ResponseHeaderName = "httpResponseHeader";
        private const string HttpResponseHeaderXPath = ResponseHeaderName + "[@name='{0}' type='{1}' required='{2}']";

        private const string GlobalResponseHeaderName = "globalHttpResponseHeader";
        private const string GlobalResponseHttpHeaderXPath = GlobalResponseHeaderName + "[@name='{0}' type='{1}' required='{2}']";

        private const string MemberXPath = "/doc/members/member[@name='{0}']";

        private readonly XPathNavigator _xmlNavigator;

        public XmlCommentsOperationHeadersFilter(string xmlDocPath, bool allowDuplicates = true, bool throwExceptions = true)
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
            ApplyHeaderComments(operation, commentNode, RequestHeaderName, false);
            ApplyHeaderComments(operation, commentNode, ResponseHeaderName, true);
        }

        private void ApplyGlobalHeaderComments(Operation operation, ControllerActionDescriptor controllerActionDescriptor)
        {
            var commentId = XmlCommentsIdHelper.GetCommentIdForType(controllerActionDescriptor.ControllerTypeInfo.UnderlyingSystemType);
            var commentNode = _xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            ApplyHeaderComments(operation, commentNode, GlobalRequestHeaderName, false);
            ApplyHeaderComments(operation, commentNode, GlobalResponseHeaderName, true);
        }

        private static void ApplyHeaderComments(Operation operation, XPathNavigator commentNode, string headerName, bool isResponse)
        {
            XPathNodeIterator paramNode = commentNode.SelectChildren(headerName, string.Empty);
            for (int i = 0; i < paramNode.Count; i++)
            {
                paramNode.MoveNext();
                AddHeaderToOperation(operation, paramNode, isResponse);
            }
        }

        private static bool HeaderParamValid(Func<string, bool> isValid, string value)
        {
            return isValid(value);
        }

        private static void AddHeaderToOperation(Operation operation, XPathNodeIterator paramNode, bool isResponse)
        {

            string uri = string.Empty; // the URI is always local for the header xml comments.

            if (paramNode.Current == null)
                return;

            string httpHeaderName = paramNode.Current.GetAttribute("name", uri);
            NonBodyParameter headerParam = new NonBodyParameter();
            headerParam.In = "header";
            headerParam.Name = httpHeaderName;

            if (String.IsNullOrWhiteSpace(httpHeaderName))
            {
                HeaderParamValidationError(operation, headerParam, "Parameter name must not be blank.");
                return;
            }

            IEnumerable<IParameter> duplicateParams =
                from p in operation.Parameters
                where p.In == "header" && p.Name.ToLower() == httpHeaderName.ToLower()
                select p;

            if (duplicateParams.Count<IParameter>() == 0)
            {
                headerParam.Required = paramNode.Current.GetAttribute("required", uri).ToLower() == bool.TrueString.ToLower();

                StringBuilder description = new StringBuilder();

                description.AppendLine(XmlCommentsTextHelper.Humanize(paramNode.Current.InnerXml));

                headerParam.Type = paramNode.Current.GetAttribute("type", uri); // Required
                if (!HeaderParamValid((v => new List<string>()
                        { "string", "number", "integer", "boolean", "array" }.Contains(v)), headerParam.Type))
                {
                    HeaderParamValidationError(operation, headerParam,
                        "Type must be one of: string, number, integer, boolean, array");
                    return;
                }

                if (headerParam.Type == "array")
                {
                    headerParam.CollectionFormat = GetStringParamAttributeWithDescription("collectionFormat", paramNode, description, uri, "csv");
                    if (!HeaderParamValid((v => new List<string>()
                        { "csv", "ssv", "tsv", "pipes" }.Contains(v)), headerParam.CollectionFormat))
                    {
                        HeaderParamValidationError(operation, headerParam,
                            "collectionFormat must be one of: csv, ssv, tsv, pipes");
                        return;
                    }

                    // TODO: finish items...
                    headerParam.Items = null;
                }

                if (!headerParam.Required)
                {
                    headerParam.Default = GetStringParamAttributeWithDescription("default", paramNode, description, uri);
                }

                headerParam.Format = GetStringParamAttributeWithDescription("format", paramNode, description, uri);
                headerParam.Maximum = GetIntParamAttributeWithDescription("maximum", paramNode, description, uri);
                headerParam.ExclusiveMaximum = GetBoolParamAttributeWithDescription("exclusiveMaximum", paramNode, description, uri);
                headerParam.Minimum = GetIntParamAttributeWithDescription("minimum", paramNode, description, uri);
                headerParam.ExclusiveMinimum = GetBoolParamAttributeWithDescription("exclusiveMinimum", paramNode, description, uri);
                headerParam.MaxLength = GetIntParamAttributeWithDescription("maxLength", paramNode, description, uri);
                headerParam.MinLength = GetIntParamAttributeWithDescription("minLength", paramNode, description, uri);
                headerParam.Pattern = GetStringParamAttributeWithDescription("pattern", paramNode, description, uri);
                headerParam.MaxItems = GetIntParamAttributeWithDescription("maxItems", paramNode, description, uri);
                headerParam.MinItems = GetIntParamAttributeWithDescription("minItems", paramNode, description, uri);
                headerParam.UniqueItems = GetBoolParamAttributeWithDescription("uniqueItems", paramNode, description, uri);
                // TODO: implement Enum Attribute
                //headerParam.Enum = GetListParamAttributeWithDescription("enum", paramNode, description, uri);
                headerParam.MultipleOf = GetIntParamAttributeWithDescription("multipleOf", paramNode, description, uri);

                headerParam.Description = description.ToString();

                if (isResponse)
                {
                    string attributeCode = paramNode.Current.GetAttribute("code", uri);
                    Response response = operation.Responses[attributeCode];
                    if (response == null)
                        return;
                    if (response.Headers == null)
                        response.Headers = new Dictionary<string, Header>();
                    var x = from name in response.Headers.Keys select name;
                    if (x.Count<string>() == 0)
                    {
                        Header header = new Header();
                        header.Type = paramNode.Current.GetAttribute("type", uri);
                        header.Description = XmlCommentsTextHelper.Humanize(paramNode.Current.InnerXml);
                        response.Headers.Add(paramNode.Current.GetAttribute("name", uri), header);
                    }
                }
                else
                    operation.Parameters.Add(headerParam);
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
            headerParam.Name = "Swagger Validation Error";
            headerParam.Description = whatsWrong;
            operation.Parameters.Add(headerParam);
        }
    }
}