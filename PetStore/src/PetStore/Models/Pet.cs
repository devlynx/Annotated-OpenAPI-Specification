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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///
    /// </summary>
    public class Pet
    {
        /// <summary>
        /// Automatically generated id in the form of a GUID
        /// </summary>
        /// <remarks>Some Remarks</remarks>
        [Required]
        public string id { get; set; }

        /// <summary>
        /// Pet Name
        /// </summary>
        /// <value>The name.</value>
        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        /// <summary>
        /// Canine Breed.
        /// </summary>       
       [Required]
        public Breed Breed { get; set; }

        /// <summary>
        /// A list of tags used for filtering pets.
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
