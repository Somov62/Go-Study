using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using System.Web.Http.Description;
using API_Project.Models.Log;
using LoggerLib;

namespace API_Project.Controllers.ServerLogs
{
    [RoutePrefix("api/log")]
    public class LogController : ApiController
    {
        private readonly Logger _logger = Logger.GetContext();

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(LogModel))]
        public IHttpActionResult GetDateLog(string accessToken, string date = "")
        {
            if (!ValidateToken(accessToken)) return BadRequest("Incorrect token");

            if (date == string.Empty) date = DateTime.Now.ToString("d");
            if (!DateTime.TryParse(date, out DateTime dateTime))
            {
                return BadRequest();
            }
            string logname = dateTime.ToString("d") + ".txt";
            var content = _logger.Directory.GetLog(logname);
            if (content == null)
            {
                return NotFound();
            }

            LogModel response = new LogModel()
            {
                FileName = logname,
                Content = content
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("dates")]
        [ResponseType(typeof(List<LogModel>))]
        public IHttpActionResult GetAllDateLogs(string accessToken)
        {
            if (!ValidateToken(accessToken)) return BadRequest("Incorrect token");

            var logs = _logger.Directory.GetLogsDateList();

            List<LogModel> response = new List<LogModel>();

            foreach (var item in logs)
            {
                response.Add(new LogModel() { FileName = item });
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("all")]
        [ResponseType(typeof(List<LogModel>))]
        public IHttpActionResult GetAllLogs(string accessToken)
        {
            if (!ValidateToken(accessToken)) return BadRequest("Incorrect token");

            var logs = _logger.Directory.GetAllLogs();

            List<LogModel> response = new List<LogModel>();

            foreach (var item in logs)
            {
                response.Add(new LogModel() 
                { 
                    FileName = item.FileName,
                    Content = item.Content
                });
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("clear")]
        [ResponseType(typeof(IHttpActionResult))]
        public IHttpActionResult ClearAllLogs(string accessToken)
        {
            if (!ValidateToken(accessToken)) return BadRequest("Incorrect token");

            _logger.Directory.ClearAll();
            return Ok();
        }

        private bool ValidateToken(string accessToken)
        {
            string projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "\\";
            string filePath = Path.Combine(System.Web.HttpRuntime.AppDomainAppPath.Replace(projectName, ""), "Credentilas", "LogAccess.txt");
            if (!File.Exists(filePath)) return false;
            return File.ReadAllText(filePath) == accessToken;
        }
    }
}
