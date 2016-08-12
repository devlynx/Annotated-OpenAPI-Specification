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

namespace PetStore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using MicroserviceBoilerplate;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.SwaggerGen.Annotations;

    [Route("[controller]")]
    [Produces("application/json")]
    public class petsController : Controller
    {
        public petsController() : base()
        {
            return;
        }

        /// <summary>
        /// Add a new pet to the store
        /// </summary>
        /// <param name="body">Pet object that needs to be added to the store</param>
        [HttpPost]
        [Produces("application/json")]
        [SwaggerOperation(OperationId = "addPet")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Pet Created")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(JSend<List<Tuple<string, string>>>), 
            Description = "Request data is invalid or improperly formated")]
        public void Create([FromBody, Required]Pet body)
        {
            return;
        }




        /*
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
        */
    }
}
