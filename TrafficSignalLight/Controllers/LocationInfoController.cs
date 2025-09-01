using System.Collections.Generic;
using System.Web.Http;

namespace TrafficSignalLight.Controllers
{
    public class LocationInfoController : ApiController
    {
        [HttpGet]
        [Route("get/all/governorates")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        //public string Get(int id)
        //{
        //    using (var db = new TraficLightSignesEntities2())
        //    {
        //        var entities = db.Governerates
        //            .Select(x => new
        //            {
        //                ID = x.ID,
        //                Name = x.Name,
        //                Latitude = x.Latitude,
        //                Longitude = x.Longitude,
        //                IpAddress = x.IPAddress,
        //                AreaId = x.AreaID
        //            }).ToList();
        //        var test = entities;

        //        return Ok(entities);
        //    }
        //}

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}