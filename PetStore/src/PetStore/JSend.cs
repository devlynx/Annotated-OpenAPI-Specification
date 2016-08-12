#region Copyright (c) 2016 Periwinkle Software Limited
// MIT License -- https://opensource.org/licenses/MIT
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

namespace MicroserviceBoilerplate
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class JSend<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSend&lt;T&gt;"/> class.
        /// </summary>
        public JSend()
        {
            status = JSendStatus.success;
            data = default(T);
            message = null;
            code = null;
        }

        /// <summary>
        /// Add an optional error code to the JSendBuilder instance. Should only be used in an error status.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>JSendBuilder.</returns>
        public JSend<T> Code(string code)
        {
            this.code = code;
            return this;
        }

        /// <summary>
        /// Add data content to the JSendBuilder instance.
        /// </summary>
        /// <param name="data">The data to add.</param>
        /// <returns>JSendBuilder.</returns>
        public JSend<T> Data(T data)
        {
            this.data = data;
            return this;
        }

        public JSend<T> Error()
        {
            status = JSendStatus.error;
            return this;
        }

        public JSend<T> Exception(Exception exception)
        {
            status = JSendStatus.error;
            message = String.Format("{0} exception: {1}", exception.GetType(), exception.Message);
            return this;
        }

        public JSend<T> Fail()
        {
            status = JSendStatus.fail;
            return this;
        }

        /// <summary>
        /// Add an error message to the JSendBuilder instance. Should only be used in an error status.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>JSendBuilder.</returns>
        public JSend<T> Message(string message)
        {
            this.message = message;
            return this;
        }

        public JSend<T> Success()
        {
            status = JSendStatus.success;
            return this;
        }

        /// <summary>
        /// Only for status code: A numeric code corresponding to the error, if applicable
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// **For status success:** A wrapper for any data returned by the API call. If the call returns no data, data will be set to null.
        /// **For status fail:** A wrapper for the details of why the request failed.
        /// **For status error:** A wrapper for any other information about the error, e.g. the conditions that caused the error, stack traces, etc
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// Only for status error: A meaningful, end-user-readable message, explaining what went wrong.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Status of the response
        /// </summary>
        [Required]
        public JSendStatus status { get; set; }
    }

    public class InvalidProperty
    {
        /// <summary>
        /// Property name that has a problem.
        /// </summary>
        public string property { get; set; }
        /// <summary>
        /// What is wrong with this property?
        /// </summary>
        public string ValidationError { get; set; }
    }
}
