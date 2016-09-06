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
    using Microsoft.AspNetCore.Mvc.Controllers;

     public class XmlCommentsOperationHeadersFilter : IOperationFilter
    {
        private const string HeaderName = "httpHeader";
        private const string HttpHeaderXPath = HeaderName + "[@name='{0}' type='{1}' required='{2}']";

        private const string GlobalHeaderName = "globalHttpHeader";
        private const string GlobalHttpHeaderXPath = GlobalHeaderName + "[@name='{0}' type='{1}' required='{2}']";

        private const string MemberXPath = "/doc/members/member[@name='{0}']";

        private readonly XPathNavigator _xmlNavigator;

        public XmlCommentsOperationHeadersFilter(string xmlDocPath, bool allowDuplicates = true, bool throwExceptions = false)
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
            ApplyHeaderComments(operation, commentNode, false);
        }

        private void ApplyGlobalHeaderComments(Operation operation, ControllerActionDescriptor controllerActionDescriptor)
        {
            var commentId = XmlCommentsIdHelper.GetCommentIdForType(controllerActionDescriptor.ControllerTypeInfo.UnderlyingSystemType);
            var commentNode = _xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            ApplyHeaderComments(operation, commentNode, true);
        }

        private static void ApplyHeaderComments(Operation operation, XPathNavigator commentNode, bool isGlobal)
        {
            XPathNodeIterator paramNode = commentNode.SelectChildren(isGlobal ? GlobalHeaderName : HeaderName, string.Empty);
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
                //TODO: Finish all of the validation and fields for http headers.

                StringBuilder description = new StringBuilder(XmlCommentsTextHelper.Humanize(paramNode.Current.InnerXml));

                headerParam.Type = paramNode.Current.GetAttribute("type", uri); // Required
                if (!HeaderParamValid((v => new List<string>()
                        { "string", "number", "integer", "boolean", "array" }.Contains(v)), headerParam.Type))
                {
                    HeaderParamValidationError(operation, headerParam,
                        "Type must be one of: string, number, integer, boolean, array");
                    return;
                }

                headerParam.Required = paramNode.Current.GetAttribute("required", uri).Trim().ToLower() == "true";

                if (!headerParam.Required)
                    headerParam.Default = paramNode.Current.GetAttribute("default", uri);

                
                headerParam.Pattern = paramNode.Current.GetAttribute("pattern", uri);

                headerParam.Description = description.ToString();

                operation.Parameters.Add(headerParam);
            }
        }

        private static void HeaderParamValidationError(Operation operation, NonBodyParameter headerParam, string whatsWrong)
        {
            headerParam.Name = "Swagger Validation Error";
            headerParam.Description = whatsWrong;
            operation.Parameters.Add(headerParam);
        }

        private static string GetHeaderParameterDescription(XPathNodeIterator paramNode, NonBodyParameter headerParam)
        {
           // headerParam.Name = "Hi There...";
            return "hello";

            StringBuilder headerBuilder = new StringBuilder(XmlCommentsTextHelper.Humanize(paramNode.Current.InnerXml));

            headerBuilder.AppendLine();
            headerBuilder.AppendLine();
            headerBuilder.AppendFormat("Pattern = {0}", headerParam.Pattern);

            headerBuilder.AppendLine();
            headerBuilder.AppendLine();
            headerBuilder.AppendFormat("maximum = {0}", 25);
            return headerBuilder.ToString();
        }
    }
}