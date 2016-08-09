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

namespace PetStore
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.SwaggerGen.Annotations;

    /// <summary>
    /// Everything about your Pets summary
    /// </summary>
    /// <example>Everything about your Pets Example</example>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// <remarks>Everything about your Pets remartks</remarks>
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class petsController : Controller
    {
        public petsController() : base()
        {
            return;
        }

        /// <summary>
        /// Deletes the specified Incident.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [Produces("application/json")]
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            return;
        }

        // GET api/owners
        /// <summary>
        /// Gets a list of Incidents.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        [Produces("application/json")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Pet>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(IEnumerable<Pet>))]
        public IActionResult Get()
        {
            return Ok("value");
        }

        // GET api/owners/5
        /// <summary>
        /// Gets the specified Incident.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        [Produces("application/json")]
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }

        /// <summary>
        /// Creates a new Incident
        /// </summary>
        /// <remarks> 
        /// ## Example 
        ///  
        ///     POST /incidents 
        ///     { 
        ///         "id": "123", 
        ///         "description": "Some product" 
        ///     } 
        ///  
        /// </remarks> 
        /// <param name="value">The value.</param>
        [Produces("application/json")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(Pet))]
        public void Create([FromBody]string value)
        {
            return;
        }

        // PUT api/owners/5
        /// <summary>
        /// Updates all properties of a specific Incident
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        [Produces("application/json")]
        [HttpPut("{id}")]
        public void UpdateReplace(Guid id, [FromBody]string value)
        {
            return;
        }
    }
}
