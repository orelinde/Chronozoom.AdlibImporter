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
            return Ok(filepath);
        }

        public IHttpActionResult Get()
        {
            BatchCommand command = new BatchCommand();
            command.Title = "TEST TITLE";
            command.Description = "TEST DESCRIPTION";
            command.Actions = new GroupAction[2];
            command.Actions[0] = new GroupAction() {CategoryName = "Creator", GroupBy = "creator"};
            command.Actions[1] = new GroupAction() { CategoryName = "material", GroupBy = "material" };
            command.Mappings = new XmlMappings();
            command.Mappings.Title = "title";
            command.Mappings.Description = "description";
            command.Mappings.Begindate = "production.date.start";
            command.Mappings.Enddate = "production.date.end";
            command.Mappings.Id = "priref";
            command.Mappings.Images = "reproduction.reference";
            command.BaseUrl = "http://amdata.adlibsoft.com/";
            command.Database = "AMCollect";
            command.ImagesLocation = "http://ahm.adlibsoft.com/ahmimages/";
            var filepath = HttpContext.Current.Server.MapPath("~/");

            if (command == null) return BadRequest("Post value is null");
            if (!ModelState.IsValid) return new InvalidModelStateResult(ModelState, this);
            BatchProcessor batch = new BatchProcessor(command);
            batch.StartNewAndWriteToFile(filepath);
            return Ok(filepath);
        }

    }
}
