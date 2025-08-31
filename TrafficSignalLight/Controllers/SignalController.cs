using Microsoft.AspNet.SignalR;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;
using TrafficSignalLight.Dto;

namespace TrafficSignalLight.Controllers
{
    //[Route("api/signal")]
    public class SignalController : ApiController
    {
        private IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();

        [HttpPost]
        [Route("api/signal/log")]
        public async Task Log(TrafficLightResponse response)
        {
            string message = JsonSerializer.Serialize(response);
            hub.Clients.All.ReceiveMessage("Server", message ?? "");
            //hub.Clients.All.TrafficSignal(message);
            var testMessage = "http://197.168.208.50/7B0100000A0005000D00000000000000007D";

            return;
        }

        [HttpGet]
        [Route("api/get/control-box")]
        public async Task<IHttpActionResult> Test()
        {
            using (HttpClient client = new HttpClient())
            {
                var requestgURL = "http://197.168.208.50/7B0100000A0005000D00000000000000007D";
                var requestErrorURL = "http://197.168.208.50/7B010007D";
                try
                {
                    HttpResponseMessage response = await client.GetAsync(requestgURL);
                    if (response.IsSuccessStatusCode)
                    {
                        string respContent = await response.Content.ReadAsStringAsync();
                    }
                }
                catch (Exception ex)
                {
                }
            }

            using (var db = new TraficLightSignesEntities2())
            {
                var entities = db.SigneControlBoxes
                    .Select(x => new SignleControlBox
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        IpAddress = x.IPAddress,
                        AreaId = x.AreaID
                    }).ToList();
                var test = entities;

                return Ok(entities);
            }
            return Ok();
        }

        private TraficLightSignesEntities2 CreateDb() => new TraficLightSignesEntities2();

        // ===================== DTOs =====================
        public class SetLocationDto
        {
            public int? ID { get; set; }
            public string Name { get; set; }
            public string IPAddress { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int? AreaID { get; set; }
            public int? GovernerateID { get; set; }
            public string AreaName { get; set; }
            public string GovernerateName { get; set; }
            public string Address { get; set; }
        }

        // يسمح بالنداء إمّا بـ SignId أو Ip
        public class ApplyCurrentRequest
        {
            public int? SignId { get; set; }
            public string Ip { get; set; }         // بديل لـ SignId (اختياري)

            public bool UseTcp { get; set; }       // افتراضي HTTP
            public int TcpPort { get; set; } = 9000;

            // Overrides اختيارية (كلها بايت واحد بالآخر)
            public int? BlinkMs { get; set; }      // لو حوّلته لعُشر ثانية: 500ms => 5

            public bool? BlinkRed { get; set; }
            public bool? BlinkAmber { get; set; }
            public bool? BlinkGreen { get; set; }
            public bool? ChangeMain { get; set; }

            // تقدر تبعت DeviceId صريح (بايت واحد) بدل اشتقاقه من SignId
            public int? DeviceId { get; set; }
        }

        // ================== Endpoints ===================

        [HttpPost, Route("locations/set")]
        public async Task<IHttpActionResult> SetLocation(SetLocationDto dto)
        {
            if (dto == null) return BadRequest("Body is required.");
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required.");
            if (string.IsNullOrWhiteSpace(dto.IPAddress)) return BadRequest("IPAddress is required.");

            using (var db = CreateDb())
            {
                // 1) Governorate
                int govId = dto.GovernerateID ?? 0;
                if (govId <= 0 && !string.IsNullOrWhiteSpace(dto.GovernerateName))
                {
                    var gname = (dto.GovernerateName ?? string.Empty).Trim();
                    var gov = await db.Governerates.FirstOrDefaultAsync(x => x.Name == gname);
                    if (gov == null)
                    {
                        gov = new Governerate { Name = gname };
                        db.Governerates.Add(gov);
                        await db.SaveChangesAsync();
                    }
                    govId = gov.ID;
                }

                // 2) Area
                int areaId = dto.AreaID ?? 0;
                if (areaId <= 0 && !string.IsNullOrWhiteSpace(dto.AreaName) && govId > 0)
                {
                    var aname = (dto.AreaName ?? string.Empty).Trim();
                    var area = await db.Areas.FirstOrDefaultAsync(x => x.Name == aname);
                    if (area == null)
                    {
                        area = new Area { Name = aname, GovernorateID = govId };
                        db.Areas.Add(area);
                        await db.SaveChangesAsync();
                    }
                    areaId = area.ID;
                }

                // 3) Upsert SigneControlBox
                SigneControlBox box = null;
                if (dto.ID.HasValue && dto.ID.Value > 0)
                    box = await db.SigneControlBoxes.FirstOrDefaultAsync(b => b.ID == dto.ID.Value);

                if (box == null)
                {
                    box = new SigneControlBox();
                    db.SigneControlBoxes.Add(box);
                }

                box.Name = (dto.Name ?? string.Empty).Trim();
                box.IPAddress = (dto.IPAddress ?? string.Empty).Trim();
                box.AreaID = areaId > 0 ? (int)areaId : 0;
                box.Address = dto.Address ?? string.Empty;
                box.Latitude = dto.Latitude;
                box.Longitude = dto.Longitude;

                await db.SaveChangesAsync();

                return Ok(new { boxId = box.ID, areaId = box.AreaID, ip = box.IPAddress });
            }
        }

        [HttpPost, Route("signals/apply-current")]
        public async Task<IHttpActionResult> ApplyCurrent(ApplyCurrentRequest req)
        {
            if ((req?.SignId ?? 0) <= 0 && string.IsNullOrWhiteSpace(req.Ip))
                return BadRequest("Provide SignId or Ip.");

            using (var db = CreateDb())
            {
                // 1) Resolve sign
                var q = db.SigneControlBoxes.AsQueryable();
                if ((req.SignId ?? 0) > 0) q = q.Where(s => s.ID == req.SignId.Value);
                else q = q.Where(s => s.IPAddress == req.Ip);

                var sign = await q.Select(s => new { s.ID, s.IPAddress, s.LightPatternID }).FirstOrDefaultAsync();
                if (sign == null) return NotFound();
                if (string.IsNullOrWhiteSpace(sign.IPAddress))
                    return BadRequest("Sign has no IPAddress.");

                // 2) Resolve template
                int? templateId = await db.SignTemplates
                    .Where(st => st.SignID == sign.ID)
                    .Select(st => (int?)st.TemplateID)
                    .FirstOrDefaultAsync();

                // 3) Pick active pattern (TimeSpan daily schedule)
                int? patternId = null;
                if (templateId.HasValue)
                {
                    var now = DateTime.Now.TimeOfDay;
                    patternId = await db.TemplatePatterns
                        .Where(tp => tp.TemplateID == templateId.Value
                                  && tp.StartFrom <= now
                                  && now < tp.FinishBy)
                        .OrderByDescending(tp => tp.StartFrom)
                        .Select(tp => (int?)tp.PetternID)
                        .FirstOrDefaultAsync();
                }

                if (!patternId.HasValue) patternId = sign.LightPatternID;
                if (!patternId.HasValue) return BadRequest("No LightPattern resolved for this sign.");
                int pid = patternId.Value;

                // 4) Load LightPattern values (كلها int لكن هتتقصّ لـ1 بايت في الفريم)
                var lp = await db.LightPatterns
                    .Where(p => p.ID == pid)
                    .Select(p => new { p.Red, p.Amber, p.Green })
                    .FirstOrDefaultAsync();

                if (lp == null) return BadRequest("Resolved LightPattern not found.");

                int blinkByte =
                    (req.BlinkMs.HasValue ? (req.BlinkMs.Value / 100) : 5); // Default=500ms

                bool bR = req.BlinkRed ?? false;
                bool bY = req.BlinkAmber ?? false;
                bool bG = req.BlinkGreen ?? false;
                bool ch = req.ChangeMain ?? false;

                // 6) DeviceId بايت: من الطلب أو من أقل 8 بت من SignId
                int deviceId = req.DeviceId.HasValue ? req.DeviceId.Value : (sign.ID & 0xFF);

                // 7) Build frame (V2) + Send
                bool sent;
                if (req.UseTcp)
                {
                    var frame = SignalProtocolV2.BuildFrameV2(
                        deviceId,
                        lp.Red, lp.Amber, lp.Green,
                        blinkByte,
                        bR, bY, bG,
                        0,          // display timer
                        0,          // cross as main
                        ch ? 1 : 0  // change main
                    );
                    sent = await SignalProtocolV2.SendTcpAsync(sign.IPAddress, req.TcpPort, frame);
                }
                else
                {
                    var hex = SignalProtocolV2.BuildHexV2(
                        deviceId,
                        lp.Red, lp.Amber, lp.Green,
                        blinkByte,
                        bR, bY, bG,
                        0,          // display timer
                        0,          // cross as main
                        ch ? 1 : 0  // change main
                    );
                    sent = await SignalProtocolV2.SendHttpAsync(sign.IPAddress, hex);
                }

                if (!sent)
                {
                    return Content(HttpStatusCode.BadGateway, new
                    {
                        message = "Failed to send command to device.",
                        signId = sign.ID,
                        patternId = pid,
                        ip = sign.IPAddress,
                        transportTried = req.UseTcp ? "tcp" : "http"
                    });
                }

                return Ok(new
                {
                    message = "Applied current pattern (V2 hex) successfully.",
                    signId = sign.ID,
                    patternId = pid,
                    ip = sign.IPAddress,
                    transport = req.UseTcp ? "tcp" : "http"
                });
            }
        }

        // (اختياري) Resolve سريع بالاسم أو الـIP
        [HttpGet, Route("signs/resolve")]
        public async Task<IHttpActionResult> Resolve(string ip = null, string name = null)
        {
            if (string.IsNullOrWhiteSpace(ip) && string.IsNullOrWhiteSpace(name))
                return BadRequest("Provide ip or name.");
            using (var db = CreateDb())
            {
                var q = db.SigneControlBoxes.AsQueryable();
                if (!string.IsNullOrWhiteSpace(ip)) q = q.Where(s => s.IPAddress == ip);
                if (!string.IsNullOrWhiteSpace(name)) q = q.Where(s => s.Name == name);

                var sign = await q.Select(s => new { s.ID, s.Name, s.IPAddress }).FirstOrDefaultAsync();
                if (sign == null) return NotFound();
                return Ok(new { signId = sign.ID, sign.Name, sign.IPAddress, deviceId = (sign.ID & 0xFF) });
            }
        }
    }
}