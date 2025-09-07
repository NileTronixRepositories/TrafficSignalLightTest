using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using TrafficSignalLight.DB;
using TrafficSignalLight.Dto;

namespace TrafficSignalLight.Controllers
{
    public class LocationsController : ApiController
    {
        private static bool tcpServerInit = false;

        // GET: api/Locations
        public string Get()
        {
            //if (TCPServer.ServerStarted)
            //    TCPServer.Stop();

            //else
            //if (!TCPServer.ServerStarted)
            //    TCPServer.Start();

            return Repository.GetObjectJsonString(Repository.GetSigneControlBoxList());
        }

        //[HttpPost]
        [HttpGet]
        [Route("api/Template/list")]
        public async Task<IHttpActionResult> GetTemplates()
        {
            using (var db = new TraficLightSignesEntities2())
            {
                var entities = db.Templates
                    .Select(x => new { x.ID, x.Name })
                    .ToList();
                return Ok(entities);
            }
        }

        [HttpGet]
        [Route("api/Pattern/list")]
        public async Task<IHttpActionResult> GetPatterns()
        {
            using (var db = new TraficLightSignesEntities2())
            {
                var entities = db.LightPatterns
                    .Select(x => new
                    {
                        x.ID,
                        x.Name,
                        x.Red,
                        x.Green,
                        x.Amber,
                        x.GreenAmberOverlab,
                        x.Pedstrain,
                        x.ShowPedstrainCounter,
                        x.SignTemplates
                    })
                    .ToList();

                return Ok(entities);
            }
        }

        [HttpGet]
        [Route("api/TemplatePattern/list")]
        public async Task<IHttpActionResult> GetTemplatePatterns()
        {
            using (var db = new TraficLightSignesEntities2())
            {
                var entities = db.TemplatePatterns
                    .Select(x => new { x.ID, x.TemplateID, x.PetternID, x.StartFrom, x.FinishBy })
                    .ToList();

                var test = entities;

                return Ok(entities);
            }
        }

        [HttpGet]
        [Route("api/SelectTemplatePattern")]
        public async Task<IHttpActionResult> GetSelectTemplatePatterns(int id)
        {
            using (var db = new TraficLightSignesEntities2())
            {
                var entities = db.TemplatePatterns.Where(x => x.TemplateID == id)
                    .Select(x => new { x.LightPattern.Name, x.StartFrom, x.FinishBy })
                    .ToList();
                return Ok(entities);
            }
        }

        [HttpGet]
        [Route("api/Governorates/list")]
        public async Task<IHttpActionResult> GetGoverorates()
        {
            using (var db = new TraficLightSignesEntities2())
            {
                var entities = db.Governerates
                    .Select(x => new { x.ID, x.Name, x.Latitude, x.Longitude })
                    .ToList();

                return Ok(entities);
            }
        }

        [HttpGet]
        [Route("api/Areas/list")]
        public async Task<IHttpActionResult> GetAreas()
        {
            using (var db = new TraficLightSignesEntities2())
            {
                var entities = db.Areas
                    .Select(x => new { x.ID, x.Name, x.Latitude, x.Longitude })
                    .ToList();

                return Ok(entities);
            }
        }

        //[HttpPost]
        [HttpGet]
        [Route("api/Pattern/Set")]
        public string SetPattern()
        {
            //var x = Request.RequestUri;
            //string ID = HttpUtility.ParseQueryString(x.Query).Get("ID");
            //string Name = HttpUtility.ParseQueryString(x.Query).Get("Name");
            //string RedInterval = HttpUtility.ParseQueryString(x.Query).Get("R");
            //string AmberInterval = HttpUtility.ParseQueryString(x.Query).Get("A");
            //string GreenInterval = HttpUtility.ParseQueryString(x.Query).Get("G");

            //int patternid = 0;
            //int green = 0;
            //int amber = 0;
            //int red = 0;

            //int.TryParse(ID, out patternid);
            //int.TryParse(GreenInterval, out green);
            //int.TryParse(AmberInterval, out amber);
            //int.TryParse(RedInterval, out red);

            //if (patternid < 0)
            //{
            //    Repository.DeleteLightPattern(patternid * -1);
            //    return "Ok";
            //}
            //else
            //{
            //    if (!String.IsNullOrEmpty(Name) && !(red == 0 && green == 0 && amber == 0))
            //    {
            //        patternid = Repository.UpdateLightPattern(new LightPattern() { ID = patternid, Name = Name, Amber = amber, Green = green, Red = red, GreenAmberOverlab = false, Pedstrain = 0, ShowPedstrainCounter = false, ShowSigneCounter = false });
            //    }
            //    else
            //        return "Incorrect pattern data!";

            //    if (patternid > 0)
            //    {
            //        return "Ok";
            //    }
            //}

            return "Failed to save pattern";
        }

        [HttpGet]
        [Route("api/Template/Set")]
        public string Template()
        {
            //var x = Request.RequestUri;
            //string ID = HttpUtility.ParseQueryString(x.Query).Get("ID");
            //string Name = HttpUtility.ParseQueryString(x.Query).Get("Name");

            //int id = 0;
            //int.TryParse(ID, out id);

            //if (id < 0)
            //{
            //    Repository.DeleteTemplate(id * -1);
            //    return "Template deleted successfully";
            //}

            //id = Repository.UpdateTemplate(new Template() { ID = id, Name = Name });
            //if (id > 0)
            //{
            //    return "Ok#" + id.ToString();
            //}

            return "Failed to save pattern";
        }

        [HttpGet]
        [Route("api/TemplatePattern/Set")]
        public string TemplatePattern()
        {
            //var x = Request.RequestUri;
            //string ID = HttpUtility.ParseQueryString(x.Query).Get("ID");
            //string TemplateID = HttpUtility.ParseQueryString(x.Query).Get("TemplateID");
            //string PatternID = HttpUtility.ParseQueryString(x.Query).Get("PatternID");
            //string StartFrom = HttpUtility.ParseQueryString(x.Query).Get("StartFrom");
            //string FinishBy = HttpUtility.ParseQueryString(x.Query).Get("FinishBy");

            //int id = 0;
            //int patternid = 0;
            //int templateid = 0;
            //TimeSpan startfrom = new TimeSpan(0, 0, 0);
            //TimeSpan finishby = new TimeSpan(0, 0, 0);

            //int.TryParse(ID, out id);
            //int.TryParse(TemplateID, out templateid);
            //int.TryParse(PatternID, out patternid);
            //TimeSpan.TryParse(StartFrom, out startfrom);
            //TimeSpan.TryParse(FinishBy, out finishby);

            ////patternid = Repository.UpdateTemplatePatterns(new LightPattern() { ID = patternid, Name = Name, Amber = amber, Green = green, Red = red, GreenAmberOverlab = false, Pedstrain = 0, ShowPedstrainCounter = false, ShowSigneCounter = false });

            //if (patternid > 0)
            //{
            //    return "Ok";
            //}

            return "Failed to save pattern";
        }

        [HttpGet]
        [Route("api/TemplatePatterns/Set")]
        public string TemplatePatterns()
        {
            //var x = Request.RequestUri;
            //string TemplateID = HttpUtility.ParseQueryString(x.Query).Get("TemplateID");
            //string Data = HttpUtility.ParseQueryString(x.Query).Get("Data");

            //int templateid = 0;
            //int.TryParse(TemplateID, out templateid);

            //var list = Repository.GetJsonObject<List<TemplatePattern>>(Data);

            //if (list.Any())
            //{
            //    templateid = Repository.UpdateTemplatePatterns(templateid, list);
            //}
            //if (templateid > 0)
            //{
            //    return "Ok";
            //}

            return "Failed To Save Template Pattern Schedule";
        }

        [HttpPost]
        //[HttpGet]
        [Route("api/Locations/Set")]
        public async Task<IHttpActionResult> SetLocation([FromBody] SetLocationRequest req)
        {
            if (req == null) return BadRequest("Empty request.");
            if (string.IsNullOrWhiteSpace(req.Name)) return BadRequest("Name is required.");
            if (req.Latitude < -90 || req.Latitude > 90) return BadRequest("Latitude out of range.");
            if (req.Longitude < -180 || req.Longitude > 180) return BadRequest("Longitude out of range.");

            using (var db = new TraficLightSignesEntities2())
            {
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                // 1) Resolve Area/Governorate (نفس المنطق اللي عندك)
                int areaId = req.AreaID ?? 0;
                int govId = req.GovernerateID ?? 0;

                if (areaId <= 0)
                {
                    if (govId <= 0 && !string.IsNullOrWhiteSpace(req.GovernerateName))
                    {
                        var gov = await db.Governerates.FirstOrDefaultAsync(g => g.Name == req.GovernerateName);
                        if (gov == null)
                        {
                            gov = new Governerate { Name = req.GovernerateName.Trim() };
                            db.Governerates.Add(gov);
                            await db.SaveChangesAsync();
                        }
                        govId = gov.ID;
                    }

                    if (govId > 0 && !string.IsNullOrWhiteSpace(req.AreaName))
                    {
                        var area = await db.Areas.FirstOrDefaultAsync(a => a.Name == req.AreaName && a.GovernorateID == govId);
                        if (area == null)
                        {
                            area = new Area { Name = req.AreaName.Trim(), GovernorateID = govId };
                            db.Areas.Add(area);
                            await db.SaveChangesAsync();
                        }
                        areaId = area.ID;
                    }
                }

                if (areaId <= 0) return BadRequest("Area could not be resolved/created.");

                // 2) Upsert SigneControlBox
                SigneControlBox box = null;
                if (req.ID.HasValue && req.ID.Value > 0)
                    box = await db.SigneControlBoxes.FirstOrDefaultAsync(b => b.ID == req.ID.Value);

                if (box == null)
                {
                    box = new SigneControlBox();
                    db.SigneControlBoxes.Add(box);
                }

                box.AreaID = areaId;
                box.IPAddress = (req.IPAddress ?? "").Trim();
                box.Latitude = req.Latitude.ToString(CultureInfo.InvariantCulture);
                box.Longitude = req.Longitude.ToString(CultureInfo.InvariantCulture);
                box.Name = req.Name?.Trim();
                if (string.IsNullOrWhiteSpace(box.Address)) box.Address = "";

                await db.SaveChangesAsync();

                // 3) LightPattern/Template logic + اختيار القيم التي سنرسلها
                int red = req.R, yel = req.A, grn = req.G;   // default من الطلب
                int patternId = req.LightPatternID;

                if (req.TemplateID == 0)
                {
                    // Upsert LightPattern على نفس قيم R/A/G القادمة من الطلب
                    LightPattern pattern = null;
                    if (patternId > 0)
                        pattern = await db.LightPatterns.FirstOrDefaultAsync(p => p.ID == patternId);

                    if (pattern == null)
                    {
                        pattern = new LightPattern();
                        db.LightPatterns.Add(pattern);
                    }

                    pattern.Name = box.Name;
                    pattern.Red = red;
                    pattern.Amber = yel;
                    pattern.Green = grn;
                    pattern.GreenAmberOverlab = false;
                    pattern.Pedstrain = 0;
                    pattern.ShowPedstrainCounter = false;
                    pattern.ShowSigneCounter = false;

                    await db.SaveChangesAsync();
                    box.LightPatternID = pattern.ID;
                    await db.SaveChangesAsync();
                    patternId = pattern.ID; // لأغراض الردّ
                }
                else
                {
                    // عند وجود TemplateID: خزّنه في الصندوق
                    //  box.TemplateID = req.TemplateID;
                    await db.SaveChangesAsync();

                    // اختياري: لو حابب تبعت قيم النمط النشط من القالب (جدول TemplatePatterns),
                    // تقدر تحلّه زى ApplyCurrent؛ هنا نخلي الإرسال يعتمد على قيم الطلب R/A/G إن حابب تبقى واضحة:
                    // أو (مثال) تحميل قيم patternId (لو عندك) من DB بدل R/A/G:
                    // var lp = await db.LightPatterns.Where(p => p.ID == patternId).Select(p => new { p.Red, p.Amber, p.Green}).FirstOrDefaultAsync();
                    // if (lp != null) { red = lp.Red; yel = lp.Amber; grn = lp.Green; }
                }

                // 4) Build & Send V2 (زي ApplyCurrent بالضبط)
                int blinkMs = req.BlinkMs ?? 500;
                int blinkByte = Math.Max(0, Math.Min(255, blinkMs / 100)); // 500ms => 5

                bool bR = req.BlinkRed ?? false;
                bool bY = req.BlinkAmber ?? false;
                bool bG = req.BlinkGreen ?? false;
                bool ch = req.ChangeMain ?? false;

                // DeviceId: من الطلب أو من أقل 8 بت من SignId
                int signId = box.ID;
                int deviceId = req.DeviceId.HasValue ? req.DeviceId.Value : (signId & 0xFF);

                bool sent;
                string transport;
                string hexPreview = SignalProtocolV2.BuildHexV2(
                    deviceId, red, yel, grn,
                    blinkByte, bR, bY, bG,
                    0, 0, ch ? 1 : 0
                );

                if (req.UseTcp)
                {
                    var frame = SignalProtocolV2.BuildFrameV2(
                        deviceId, red, yel, grn,
                        blinkByte, bR, bY, bG,
                        0, 0, ch ? 1 : 0
                    );
                    sent = await SignalProtocolV2.SendTcpAsync(box.IPAddress, req.TcpPort, frame);
                    transport = "tcp";
                }
                else
                {
                    sent = await SignalProtocolV2.SendHttpAsync(box.IPAddress, hexPreview);
                    transport = "http";
                }

                if (!sent)
                {
                    return Content(HttpStatusCode.BadGateway, new
                    {
                        message = "Failed to send command to device.",
                        signId = signId,
                        patternId = patternId,
                        ip = box.IPAddress,
                        transportTried = transport,
                        hex = hexPreview
                    });
                }

                // 5) Response
                return Ok(new
                {
                    message = "Saved and applied (V2).",
                    signId = signId,
                    patternId = patternId,
                    ip = box.IPAddress,
                    transport,
                    hex = hexPreview
                });
            }
        }

        private byte[] GetBytes(int i)
        {
            byte b0 = (byte)i,
                b1 = (byte)(i >> 8);
            return new byte[] { b1, b0 };
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        private static byte[] GetBytesBE(int n)
        {
            // 16-bit big-endian (00 00 .. FF FF) حسب بروتوكولك
            return new byte[] { (byte)((n >> 8) & 0xFF), (byte)(n & 0xFF) };
        }

        private static string ByteArrayToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        private static string ToHex16BE(int value) => ByteArrayToHex(GetBytesBE(value));

        public class SetLocationRequest
        {
            public int? ID { get; set; }

            public int? AreaID { get; set; }
            public string AreaName { get; set; }

            public int? GovernerateID { get; set; }
            public string GovernerateName { get; set; }

            public string Name { get; set; }
            public string IPAddress { get; set; }

            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public int R { get; set; }     // red seconds (0..255 final)
            public int A { get; set; }     // amber/yellow
            public int G { get; set; }     // green

            public bool? BlinkRed { get; set; }
            public bool? BlinkAmber { get; set; }
            public bool? BlinkGreen { get; set; }
            public int? BlinkMs { get; set; }      // default 500ms → يحوَّل إلى بايت

            public bool? ChangeMain { get; set; }  // 0/1

            public int TemplateID { get; set; }
            public int LightPatternID { get; set; }

            // خيارات الإرسال:
            public bool UseTcp { get; set; } = false;

            public int TcpPort { get; set; } = 502; // حسب جهازك
            public int? DeviceId { get; set; }      // إن ما أُرسِلش، هنشتقّه من SignId (ID)
        }
    }
}