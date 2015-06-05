namespace Chronozoom.AdlibImporter.Backend.Controllers
{
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Web.Http.Results;
    using Models;

    [EnableCors(origins:"*",headers:"*",methods:"*")]
    public class BatchController : ApiController
    {
        // POST api/values
        public IHttpActionResult Post(BatchCommand command)
        {
            if(command == null) return BadRequest("Post value is null");
            if (!ModelState.IsValid) return new InvalidModelStateResult(ModelState,this);
            BatchProcessor.BatchProcessor.StartNew(command);
            return Ok();
        }

    }
}
