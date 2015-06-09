using System.Web;

namespace Chronozoom.AdlibImporter.Backend.Controllers
{
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Web.Http.Results;
    using Models;
    using BatchProcessor;
    using System.Diagnostics;

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BatchController : ApiController
    {
        // POST api/values
        public IHttpActionResult Post(BatchCommand command)
        {
            var filepath = HttpContext.Current.Server.MapPath("~/");

            if (command == null) return BadRequest("Post value is null");
            if (!ModelState.IsValid) return new InvalidModelStateResult(ModelState, this);
            BatchProcessor batch = new BatchProcessor(command);
            batch.StartNewAndWriteToFile(filepath);
            return Ok();
        }

    }
}
