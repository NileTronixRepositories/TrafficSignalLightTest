using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TrafficSignalLight.DB;

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
        public string GetTemplates()
        {
            var temps = Repository.GetTemplateList();
            return Repository.GetObjectJsonString(temps);
        }

        [HttpGet]
        [Route("api/Pattern/list")]
        public string GetPatterns()
        {
            var patterns = Repository.GetLightPatternList();
            return Repository.GetObjectJsonString(patterns);
        }

        [HttpGet]
        [Route("api/TemplatePattern/list")]
        public string GetTemplatePatterns()
        {
            var Temppatterns = Repository.GetTemplatePatternList();
            return Repository.GetObjectJsonString(Temppatterns);
        }

        [HttpGet]
        [Route("api/Governorates/list")]
        public string GetGoverorates()
        {
            var govs = Repository.GetGovernerateList();
            return Repository.GetObjectJsonString(govs);
        }

        [HttpGet]
        [Route("api/Areas/list")]
        public string GetAreas()
        {
            var areas = Repository.GetAreaList();
            return Repository.GetObjectJsonString(areas);
        }

        //[HttpPost]
        [HttpGet]
        [Route("api/Pattern/Set")]
        public string SetPattern()
        {
            var x = Request.RequestUri;
            string ID = HttpUtility.ParseQueryString(x.Query).Get("ID");
            string Name = HttpUtility.ParseQueryString(x.Query).Get("Name");
            string RedInterval = HttpUtility.ParseQueryString(x.Query).Get("R");
            string AmberInterval = HttpUtility.ParseQueryString(x.Query).Get("A");
            string GreenInterval = HttpUtility.ParseQueryString(x.Query).Get("G");

            int patternid = 0;
            int green = 0;
            int amber = 0;
            int red = 0;

            int.TryParse(ID, out patternid);
            int.TryParse(GreenInterval, out green);
            int.TryParse(AmberInterval, out amber);
            int.TryParse(RedInterval, out red);

            if (patternid < 0)
            {
                Repository.DeleteLightPattern(patternid * -1);
                return "Ok";
            }
            else
            {
                if (!String.IsNullOrEmpty(Name) && !(red == 0 && green == 0 && amber == 0))
                {
                    patternid = Repository.UpdateLightPattern(new LightPattern() { ID = patternid, Name = Name, Amber = amber, Green = green, Red = red, GreenAmberOverlab = false, Pedstrain = 0, ShowPedstrainCounter = false, ShowSigneCounter = false });
                }
                else
                    return "Incorrect pattern data!";

                if (patternid > 0)
                {
                    return "Ok";
                }
            }

            return "Failed to save pattern";
        }

        [HttpGet]
        [Route("api/Template/Set")]
        public string Template()
        {
            var x = Request.RequestUri;
            string ID = HttpUtility.ParseQueryString(x.Query).Get("ID");
            string Name = HttpUtility.ParseQueryString(x.Query).Get("Name");

            int id = 0;
            int.TryParse(ID, out id);

            if (id < 0)
            {
                Repository.DeleteTemplate(id * -1);
                return "Template deleted successfully";
            }

            id = Repository.UpdateTemplate(new Template() { ID = id, Name = Name });
            if (id > 0)
            {
                return "Ok#" + id.ToString();
            }

            return "Failed to save pattern";
        }

        [HttpGet]
        [Route("api/TemplatePattern/Set")]
        public string TemplatePattern()
        {
            var x = Request.RequestUri;
            string ID = HttpUtility.ParseQueryString(x.Query).Get("ID");
            string TemplateID = HttpUtility.ParseQueryString(x.Query).Get("TemplateID");
            string PatternID = HttpUtility.ParseQueryString(x.Query).Get("PatternID");
            string StartFrom = HttpUtility.ParseQueryString(x.Query).Get("StartFrom");
            string FinishBy = HttpUtility.ParseQueryString(x.Query).Get("FinishBy");

            int id = 0;
            int patternid = 0;
            int templateid = 0;
            TimeSpan startfrom = new TimeSpan(0, 0, 0);
            TimeSpan finishby = new TimeSpan(0, 0, 0);

            int.TryParse(ID, out id);
            int.TryParse(TemplateID, out templateid);
            int.TryParse(PatternID, out patternid);
            TimeSpan.TryParse(StartFrom, out startfrom);
            TimeSpan.TryParse(FinishBy, out finishby);

            //patternid = Repository.UpdateTemplatePatterns(new LightPattern() { ID = patternid, Name = Name, Amber = amber, Green = green, Red = red, GreenAmberOverlab = false, Pedstrain = 0, ShowPedstrainCounter = false, ShowSigneCounter = false });

            if (patternid > 0)
            {
                return "Ok";
            }

            return "Failed to save pattern";
        }

        [HttpGet]
        [Route("api/TemplatePatterns/Set")]
        public string TemplatePatterns()
        {
            var x = Request.RequestUri;
            string TemplateID = HttpUtility.ParseQueryString(x.Query).Get("TemplateID");
            string Data = HttpUtility.ParseQueryString(x.Query).Get("Data");

            int templateid = 0;
            int.TryParse(TemplateID, out templateid);

            var list = Repository.GetJsonObject<List<TemplatePattern>>(Data);

            if (list.Any())
            {
                templateid = Repository.UpdateTemplatePatterns(templateid, list);
            }
            if (templateid > 0)
            {
                return "Ok";
            }

            return "Failed To Save Template Pattern Schedule";
        }

        //[HttpPost]
        [HttpGet]
        [Route("api/Locations/Set")]
        public async Task<string> SetLocation()
        {
            var x = Request.RequestUri;
            string ID = HttpUtility.ParseQueryString(x.Query).Get("ID");
            string AreaID = HttpUtility.ParseQueryString(x.Query).Get("AreaID");
            string GovernerateID = HttpUtility.ParseQueryString(x.Query).Get("AGovernerateID");
            string AreaName = HttpUtility.ParseQueryString(x.Query).Get("AreaName");
            string GovernerateName = HttpUtility.ParseQueryString(x.Query).Get("GovernerateName");
            string Name = HttpUtility.ParseQueryString(x.Query).Get("Name");
            string IPAddress = HttpUtility.ParseQueryString(x.Query).Get("IPAddress");
            string Latitude = HttpUtility.ParseQueryString(x.Query).Get("Latitude");
            string Longitude = HttpUtility.ParseQueryString(x.Query).Get("Longitude");

            int signID = 0;
            int areaID = 0;
            int govID = 0;
            int.TryParse(ID, out signID);
            int.TryParse(AreaID, out areaID);
            int.TryParse(GovernerateID, out govID);

            if (areaID <= 0)
            {
                var govObj = Repository.GetGovernerate(GovernerateName.TrimEnd().TrimStart());
                if (govObj == null)
                {
                    govID = Repository.UpdateGovernerate(new Governerate() { Name = GovernerateName });
                }
                else
                {
                    govID = govObj.ID;
                }

                var areaObj = Repository.GetArea(AreaName.TrimEnd().TrimStart());
                if (areaObj == null && govID > 0)
                {
                    areaID = Repository.UpdateArea(new Area() { Name = AreaName, GovernorateID = govID });
                }
                else
                {
                    areaID = areaObj.ID;
                }
            }

            SigneControlBox signeControlBox = new SigneControlBox()
            { ID = signID, AreaID = areaID, IPAddress = IPAddress, Latitude = Latitude, Longitude = Longitude, Name = Name, Address = "" };

            int boxid = Repository.UpdateSigneControlBox(signeControlBox);

            if (boxid > 0)
            {
                string RedInterval = HttpUtility.ParseQueryString(x.Query).Get("R");
                string AmberInterval = HttpUtility.ParseQueryString(x.Query).Get("A");
                string GreenInterval = HttpUtility.ParseQueryString(x.Query).Get("G");

                string BlinkRed = HttpUtility.ParseQueryString(x.Query).Get("bR");
                string BlinkAmber = HttpUtility.ParseQueryString(x.Query).Get("bA");
                string BlinkGreen = HttpUtility.ParseQueryString(x.Query).Get("bG");

                string ChangeMain = HttpUtility.ParseQueryString(x.Query).Get("cM");

                string LightPatternID = HttpUtility.ParseQueryString(x.Query).Get("LightPatternID");
                string TemplateID = HttpUtility.ParseQueryString(x.Query).Get("TemplateID");

                int green = 0;
                int amber = 0;
                int red = 0;

                bool bgreen = false;
                bool bamber = false;
                bool bred = false;

                bool changeMain = false;

                int templateID = 0;
                int patternid = 0;

                int.TryParse(GreenInterval, out green);
                int.TryParse(AmberInterval, out amber);
                int.TryParse(RedInterval, out red);

                bool.TryParse(BlinkGreen, out bgreen);
                bool.TryParse(BlinkAmber, out bamber);
                bool.TryParse(BlinkRed, out bred);

                bool.TryParse(ChangeMain, out changeMain);

                int.TryParse(TemplateID, out templateID);
                int.TryParse(LightPatternID, out patternid);

                if (templateID == 0)
                {
                    patternid = Repository.UpdateLightPattern(new LightPattern() { ID = patternid, Name = Name, Amber = amber, Green = green, Red = red, GreenAmberOverlab = false, Pedstrain = 0, ShowPedstrainCounter = false, ShowSigneCounter = false });
                    if (patternid > 0)
                    {
                        signeControlBox.LightPatternID = patternid;
                        Repository.UpdateSigneControlBox(signeControlBox);
                    }
                }
                else { }

                /*Compose here*/
                {
                    string R_Value = ByteArrayToString(GetBytes(red));
                    string A_Value = ByteArrayToString(GetBytes(amber));
                    string G_Value = ByteArrayToString(GetBytes(green));

                    string blink500ml = "01F4";

                    byte[] redBytes = BitConverter.GetBytes(red);
                    byte[] yellowBytes = BitConverter.GetBytes(amber);
                    byte[] greenBytes = BitConverter.GetBytes(green);

                    // send to GSM Or TCP
                    string CMD = "7B01AC" + R_Value + A_Value + G_Value + "0000007D";
                    var requestgURL = "http://197.168.208.50/" + CMD;

                    using (HttpClient client = new HttpClient())
                    {
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

                    //TCPClient.Send(IPAddress.Trim(), CMD);

                    byte[] data = { 0x7B, 0x01, 0xAC,/*Red*/0x0, redBytes[0],/*Yellow*/ 0x0, yellowBytes[0], /*Green*/0x0, greenBytes[0],/*blink timer*/ 0x01, 0xF4,/*blink red*/ bred ? (byte)1 : (byte)0,/*blink yellow*/  bamber ? (byte)1 : (byte)0,/*blink green*/  bgreen ? (byte)1 : (byte)0,/*display timer*/ 0x0,/*cross as main*/ 0x0,/*change main*/changeMain ? (byte)1 : (byte)0, 0x7D };
                    // TCPClient.Send(IPAddress.Trim(), data);
                }
            }
            return "Ok";
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
    }
}