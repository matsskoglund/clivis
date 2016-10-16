using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clivis.Models;

namespace Clivis.Controllers
{
    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        // Repository for the Climate Items
        public IClimateRepository ClimateItems { get; set; }

        public ClimateController(IClimateRepository climateItems)
        {
            ClimateItems = climateItems;
        }
 

        [HttpGet]
        public IEnumerable<ClimateItem> GetAll()
        {
            return ClimateItems.GetAll();
        }

        [HttpGet("{id}", Name = "GetClimate")]
        public IActionResult GetById(string id)
        {
            var item = ClimateItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Create([FromBody]ClimateItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            ClimateItems.Add(item);
            return CreatedAtRoute("AddClimate", new { id = item.Key }, item);
        }
    }
}