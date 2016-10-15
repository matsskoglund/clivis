using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clivis.Models;

namespace Clivis.Controllers
{
    [Route("api/[controller]")]
    public class ClimateController : Controller
    {
        public ClimateController(IClimateRepository climateItems)
        {
            ClimateItems = climateItems;
        }
        public IClimateRepository ClimateItems { get; set; }

        [HttpGet]
        public IEnumerable<ClimateItem> GetAll()
        {
            return ClimateItems.GetAll();
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            var item = ClimateItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }
    }
}