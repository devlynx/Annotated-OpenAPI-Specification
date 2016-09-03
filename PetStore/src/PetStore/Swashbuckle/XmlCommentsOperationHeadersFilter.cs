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

namespace PetStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.XPath;
    using Microsoft.AspNetCore.Mvc.Controllers;

    public class XmlCommentsOperationHeadersFilter : IOperationFilter
    {
        private const string HeaderName = "httpHeader"; // NOTE: must match the value in HttpHeaderXPath

        private const string MemberXPath = "/doc/members/member[@name='{0}']";

        private const string HttpHeaderXPath = "httpHeader[@name='{0}' type='{1}' required='{2}']";

        private readonly XPathNavigator _xmlNavigator;

        public XmlCommentsOperationHeadersFilter(XPathDocument xmlDoc, bool allowDuplicates = true)
        {
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
            ApplyHeaderComments(operation, commentNode);
        }

        private void ApplyGlobalHeaderComments(Operation operation, ControllerActionDescriptor controllerActionDescriptor)
        {
            var commentId = XmlCommentsIdHelper.GetCommentIdForType(controllerActionDescriptor.ControllerTypeInfo.UnderlyingSystemType);
            var commentNode = _xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));
            ApplyHeaderComments(operation, commentNode);
        }

        private static void ApplyHeaderComments(Operation operation, XPathNavigator commentNode)
        {
            XPathNodeIterator paramNode = commentNode.SelectChildren(HeaderName, string.Empty);
            for (int i = 0; i < paramNode.Count; i++)
            {
                paramNode.MoveNext();
                AddHeaderToOperation(operation, paramNode);
            }
        }

        private static void AddHeaderToOperation(Operation operation, XPathNodeIterator paramNode)
        {

            string uri = string.Empty; // the URI is always local for the header xml comments.

            if (paramNode.Current == null)
                return;

            string httpHeaderName = paramNode.Current.GetAttribute("name", uri);
            IEnumerable<IParameter> duplicateParams = from p in operation.Parameters
                                                      where p.In == "header" && p.Name.ToLower() == httpHeaderName.ToLower()
                                                      select p;

            if (duplicateParams.Count<IParameter>() == 0)
            {
                NonBodyParameter headerParam = new NonBodyParameter();

                //TODO: Finish all of the validation and fields for http headers.

                headerParam.In = "header";
                headerParam.Name = httpHeaderName; // Required
                headerParam.Description = XmlCommentsTextHelper.Humanize(paramNode.Current.InnerXml);
                headerParam.Type = paramNode.Current.GetAttribute("type", uri); // Required
                headerParam.Name = httpHeaderName;
                headerParam.Default = paramNode.Current.GetAttribute("default", uri);
                headerParam.Required = paramNode.Current.GetAttribute("required", uri).Trim().ToLower() == "true";
                headerParam.Pattern = paramNode.Current.GetAttribute("pattern", uri);

                operation.Parameters.Add(headerParam);
            }
        }
    }
    enum httpHeaderType
    {
         @string,
         number, 
         integer, 
         boolean,
         array
    }
}