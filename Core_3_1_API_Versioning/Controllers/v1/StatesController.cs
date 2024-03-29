﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace APP.API.Controllers.v1
{
    /// <summary>
    /// State Management
    /// </summary>
    /// <remarks>
    /// Author:
    /// 
    ///     Afolabi Gmatt Matthew
    ///     
    /// Copyright:
    /// 
    ///     Copyright (c) Phareztech Code Lab. All rights reserved.
    /// 
    /// </remarks>
    [Authorize(AuthenticationSchemes = "OAuth")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        // GET: api/Todo
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public string GetByID(int id)
        {
            return "value";
        }

        // POST: api/Todo
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}