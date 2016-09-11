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

using Swashbuckle.SwaggerGen.Annotations;

namespace PetStore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MicroserviceBoilerplate;
    using Microsoft.AspNetCore.Mvc;

    /// <controllerSubTitle name="pets">Everything about your pets</controllerSubTitle>
    /// <globalHttpHeader 
    ///     name="CorelationId" 
    ///     type="string"
    ///     pattern="[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}" 
    ///     required="true">Guid to help track request flow.
    /// </globalHttpHeader>
    [Route("[controller]")]
    [Produces("application/json")]
    public class petsController : Controller
    {
        /// <summary>
        /// Add a new pet to the store
        /// </summary>

        /// <response code="400">Pet data is invalid</response> 
        /// <response code="201">Returns the newly created pet id</response> 

        /// <param name="pet">**Pet to be added to the store**</param>

        /// <httpHeader name="Accept-Language" type="string" default="en-US">Preferred language</httpHeader>

        [HttpPost]
        [Produces("application/json")]
        [SwaggerOperation(OperationId = "addPet")]
        [ProducesResponseType(typeof(JSend<IdResponse>), 201)]
        [ProducesResponseType(typeof(JSend<IEnumerable<InvalidProperty>>), 400)]
        public IActionResult Create([FromBody, Required]Pet pet)
        {
            var abc = Request.Headers["xxx"];
            return Ok("value");
        }

        /// <summary>
        /// Deletes a pet
        /// </summary>

        /// <response code="400">Invalid ID supplied</response> 
        /// <response code="200">successful operation</response> 
        /// <response code="404">Pet not found</response> 

        /// <param name="id">Pet id to delete</param>
        /// <param name="api_key"></param>

        [Produces("application/json")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JSend<>), 200)]
        [ProducesResponseType(typeof(JSend<>), 404)]
        [ProducesResponseType(typeof(JSend<IEnumerable<InvalidProperty>>), 400)]
        public IActionResult Delete([Required, FromHeader] string api_key, [Required] string id)
        {
            return Ok("value");
        }

        /// <summary>
        /// Finds pets by tags
        /// </summary>

        /// <remarks>
        /// Will be removed in version 3. Multiple tags can be provided with comma separated strings. Must have at least 1 tag in the request.
        /// </remarks>

        /// <response code="400">Tag data is invalid</response> 
        /// <response code="200">successful operation</response> 
 
        /// <param name="tags">Tags to filter by</param>
 
        [Produces("application/json")]
        [HttpGet("/pet/findByTags")]
        [ProducesResponseType(typeof(JSend<IEnumerable<Pet>>), 200)]
        [ProducesResponseType(typeof(JSend<IEnumerable<InvalidProperty>>), 400)]
        [Obsolete]
        public IActionResult Get([FromQuery, Required]string tags)
        {
            return Ok("value");
        }

        /// <summary>
        /// Updates a pet in the store with form data
        /// </summary>

        /// <remarks>
        /// For the parameter status, provide multiple values in new lines.
        /// </remarks>

        /// <response code="400">Invalid ID supplied</response> 
        /// <response code="200">successful operation</response> 
        /// <response code="404">Pet not found</response> 

        /// <param name="id">ID of pet that needs to be updated</param>
        /// <param name="name">Updated name of the pet</param>
        /// <param name="status">Updated status of the pet</param>

        [Produces("application/json")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(JSend<>), 200)]
        [ProducesResponseType(typeof(JSend<>), 404)]
        [ProducesResponseType(typeof(JSend<IEnumerable<InvalidProperty>>), 400)]
        public IActionResult UpdateReplace([Required] string id, [FromForm]string name, [FromForm]List<string> status)
        {
            return Ok("value");
        }
    }
}
