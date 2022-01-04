using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseServiceProductConfigurator.Controllers {
    public abstract class AController<T, K> : ControllerBase where T : class {

        DbContext context;

        protected AController( DbContext context ) {
            this.context = context;
        }


        // GET:
        [HttpGet]
        public IActionResult Get() {
            List<T> toReturn = context.Set<T>().ToList();
            if(toReturn.Count == 0)
                return NotFound();
            return Ok(toReturn);
        }

        // GET by ID
        [HttpGet("{id}")]
        public virtual IActionResult Get( K id ) {
            T? toReturn = context.Set<T>().Find(id);
            if ( toReturn == null )
                return NotFound();
            else
                return Ok(toReturn);
        }

        // POST
        [HttpPost]
        public IActionResult Post( [FromBody] T value ) {
            try {
                context.Set<T>().Add(value);
                context.SaveChanges();
                return Accepted();
            }
            catch ( Exception e ) {
                return BadRequest(e.Message);
            }
        }

        // PUT
        [HttpPut("{id}")]
        public IActionResult Put( K id, [FromBody] T value ) {
            try {
                context.Set<T>().Remove((T) Get(id));
                Post(value);
                context.SaveChanges();
                return Accepted();
            }
            catch ( Exception e ) {
                return BadRequest(e.Message);
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete( K id ) {
            try {
                context.Set<T>().Remove((T) Get(id));
                context.SaveChanges();
                return Ok();
            }
            catch ( Exception e ) {
                return BadRequest(e.Message);
            }
        }
    }
}
