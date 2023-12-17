using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        [HttpGet("{fileId}")]
        public ActionResult GetFile(int fileId)
        {
            var pathToFile = "Love.Actually.subs.txt";

            if (!System.IO.File.Exists(pathToFile)) 
            {
                return NotFound();
            }

            return File(System.IO.File.ReadAllBytes(pathToFile), "text/plain");
        }
    }
}
