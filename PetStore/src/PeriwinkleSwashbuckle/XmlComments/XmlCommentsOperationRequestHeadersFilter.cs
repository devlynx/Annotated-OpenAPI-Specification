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

    public class XmlCommentsOperationRequestHeadersFilter : IOperationFilter
    {
        private const string RequestHeaderName = "httpRequestHeader";
        private const string HttpRequestHeaderXPath = RequestHeaderName + "[@name='{0}' type='{1}']";

        private const string GlobalRequestHeaderName = "globalHttpRequestHeader";
        private const string GlobalRequestHttpHeaderXPath = GlobalRequestHeaderName + "[@name='{0}' type='{1}']";

        private const string MemberXPath = "/doc/members/member[@name='{0}']";

        private readonly XPathNavigator _xmlNavigator;

        public XmlCommentsOperationRequestHeadersFilter(string xmlDocPath, bool allowDuplicates = true, bool throwExceptions = true)
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
            string commentId = XmlCommentsIdHelper.GetCommentIdForMethod(controllerActionDescriptor.MethodInfo);
            if (String.IsNullOrWhiteSpace(commentId))
                return;
            XPathNavigator commentNode = _xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            if (commentNode == null)
                return;

            ApplyHeaderComments(operation, commentNode, RequestHeaderName);
        }

        private void ApplyGlobalHeaderComments(Operation operation, ControllerActionDescriptor controllerActionDescriptor)
        {
            string commentId = XmlCommentsIdHelper.GetCommentIdForType(controllerActionDescriptor.ControllerTypeInfo.UnderlyingSystemType);
            if (String.IsNullOrWhiteSpace(commentId))
                return;
            XPathNavigator commentNode = _xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            if (commentNode == null)
                return;
            ApplyHeaderComments(operation, commentNode, GlobalRequestHeaderName);
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

                HeaderBuilder headerBuilder = new HeaderBuilder();
                PartialHeader header = headerBuilder.GetHeader(paramNode);

                NonBodyParameter headerParam = TypeAdapter.Adapt<NonBodyParameter>(header);

                headerParam.In = "header";
                headerParam.Name = httpHeaderName;
                headerParam.Description = headerBuilder.ToString();
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