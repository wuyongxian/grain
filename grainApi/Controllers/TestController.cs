using grainApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace grainApi.Controllers
{
    public class TestController : ApiController
    {
        Test[] products = new Test[]{
        new Test{id=1,name="name001"},
        new Test{id=2,name="name002"},
        new Test{id=3,name="name003"}
        };

        public IEnumerable<Test> GetAllProducts() {
            return products;
        }

        public IHttpActionResult GetProductByID(int id) {
            var product = products.FirstOrDefault((p) => p.id == id); 
            if (product == null) {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult PostTest([FromBody]Test t) {
            var product = products.FirstOrDefault((p) => p.id == t.id);
            if (product == null)
            {
                return NotFound();
            }
            else {
                return Ok(product);
            }
        }
    }
}
