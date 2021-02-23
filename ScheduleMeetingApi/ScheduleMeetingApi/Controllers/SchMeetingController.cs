using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ScheduleMeetingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchMeetingController : ControllerBase
    {
        public AppDb Db { get; }
        private IConfiguration configuration;
        public SchMeetingController(AppDb db, IConfiguration iConfig)
        {
            Db = db;
            configuration = iConfig;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SchMeetingModel body)
        {
            try
            {
                await Db.Connection.OpenAsync();
                body.Db = Db;
                await body.InsertAsync();
                ResponseCl rs = new ResponseCl();
                if (body.Id > 0)
                {
                    rs.Id = body.Id;
                    rs.ResponseCode = "000";
                    rs.ResponseStatus = "Success";
                }
                else
                {
                    rs.Id = 0;
                    rs.ResponseCode = "111";
                    rs.ResponseStatus = "Failed";
                }
                return new OkObjectResult(rs); // Api Response
            }
            catch (Exception ex)
            {
                return new OkObjectResult(ex.Message);
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetLatest()
        {
            await Db.Connection.OpenAsync();
            var query = new SchMeetingModel(Db);
            var result = await query.LatestPostsAsync();

            return new OkObjectResult(result);
        }
    }
}
