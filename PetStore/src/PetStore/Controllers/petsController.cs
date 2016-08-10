﻿#region Copyright (c) 2016 Periwinkle Software Limited
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
// limitations under the License.#endregion
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
