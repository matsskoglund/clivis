using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using Temperatures;

namespace Clivis.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
  //      private Temperature temps = new Temperature();
               
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { "Matte", "Skoglund" };

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
