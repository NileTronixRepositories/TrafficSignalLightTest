using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrafficSignalLight.DB
{
    public class Repository
    {
        #region SigneControlBox

        public static List<SignWithLightPattern> GetSigneControlBoxList()
        {
            var list = new List<SignWithLightPattern>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    cont.DeferredLoadingEnabled = false;
                    list = cont.SigneControlBoxes.Select(s => new SignWithLightPattern { ID = s.ID, AreaID = s.AreaID, Address = s.Address, IPAddress = s.IPAddress, Latitude = s.Latitude, Longitude = s.Longitude, Name = s.Name, LightPatternID = s.LightPatternID }).ToList();
                    if (list.Any())
                    {
                        list.ForEach(l =>
                        {
                            var area = GetAreaList().FirstOrDefault(a => a.ID == l.AreaID);
                            if (area != null) l.GovernerateID = area.GovernorateID;

                            var signLightPattern = GetLightPattern(l.LightPatternID);
                            if (signLightPattern != null)
                            {
                                l.LightPatternID = signLightPattern.ID;
                                l.Red = signLightPattern.Red;
                                l.Green = signLightPattern.Green;
                                l.Amber = signLightPattern.Amber;
                            }

                            var temp = GetSignTemplate(l.ID);
                            if (temp != null) l.TemplateID = temp.ID;
                        });
                    }
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static int InsertSigneControlBox(SigneControlBox obj)
        {
            int ret = 0;
            bool alreadyExists = false;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var checkIPAddress = cont.SigneControlBoxes.Where(c => c.IPAddress == obj.IPAddress);
                    var checkOnMap = cont.SigneControlBoxes.Where(c => c.Latitude == obj.Latitude && c.Longitude == obj.Longitude);
                    if (checkIPAddress.Any())
                    {
                        ret = -1;
                        alreadyExists = true;
                    }
                    if (checkOnMap.Any())
                    {
                        ret = -2;
                        alreadyExists = true;
                    }

                    if (!alreadyExists == true)
                    {
                        cont.SigneControlBoxes.InsertOnSubmit(obj);
                        cont.SubmitChanges();
                        ret = obj.ID;
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int UpdateSigneControlBox(SigneControlBox obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.SigneControlBoxes.FirstOrDefault(c => c.ID == obj.ID);
                    if (check != null)
                    {
                        check.Name = obj.Name;
                        check.IPAddress = obj.IPAddress;
                        check.Longitude = obj.Longitude;
                        check.Latitude = obj.Latitude;
                        check.AreaID = obj.AreaID;
                        check.LightPatternID = obj.LightPatternID;
                        check.Address = obj.Address;

                        cont.SubmitChanges();
                        ret = obj.ID;
                    }
                    else
                        return InsertSigneControlBox(obj);
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int DeleteSigneControlBox(int id)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.SigneControlBoxes.FirstOrDefault(c => c.ID == id);
                    if (check != null)
                    {
                        ret = id;
                        cont.SigneControlBoxes.DeleteOnSubmit(check);
                        cont.SubmitChanges();
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        #endregion SigneControlBox

        #region LightPattern

        public static List<LightPattern> GetLightPatternList()
        {
            var list = new List<LightPattern>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    list = cont.LightPatterns.ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static LightPattern GetLightPattern(int id)
        {
            LightPattern obj = null;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    obj = cont.LightPatterns.FirstOrDefault(p => p.ID == id);
                }
            }
            catch (Exception ex) { }
            return obj;
        }

        public static int InsertLightPattern(LightPattern obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    cont.LightPatterns.InsertOnSubmit(obj);
                    cont.SubmitChanges();
                    ret = obj.ID;
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int UpdateLightPattern(LightPattern obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.LightPatterns.FirstOrDefault(c => c.ID == obj.ID /*|| c.Name == obj.Name*/);
                    if (check != null)
                    {
                        check.Red = obj.Red;
                        check.Green = obj.Green;
                        check.Amber = obj.Amber;
                        check.GreenAmberOverlab = obj.GreenAmberOverlab;
                        check.Pedstrain = obj.Pedstrain;
                        check.ShowSigneCounter = obj.ShowSigneCounter;
                        check.ShowPedstrainCounter = obj.ShowPedstrainCounter;

                        cont.SubmitChanges();
                        ret = check.ID;
                    }
                    else
                        return InsertLightPattern(obj);
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int DeleteLightPattern(int id)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.LightPatterns.FirstOrDefault(c => c.ID == id);
                    if (check != null)
                    {
                        ret = id;
                        cont.LightPatterns.DeleteOnSubmit(check);
                        cont.SubmitChanges();
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        #endregion LightPattern

        #region Template

        public static List<Template> GetTemplateList()
        {
            var list = new List<Template>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    list = cont.Templates.ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static int InsertTemplate(Template obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    cont.Templates.InsertOnSubmit(obj);
                    cont.SubmitChanges();
                    ret = obj.ID;
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int UpdateTemplate(Template obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.Templates.FirstOrDefault(c => c.ID == obj.ID);
                    if (check != null)
                    {
                        check.Name = obj.Name;

                        cont.SubmitChanges();
                        ret = check.ID;
                    }
                    else
                        return InsertTemplate(obj);
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int DeleteTemplate(int id)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.Templates.FirstOrDefault(c => c.ID == id);
                    if (check != null)
                    {
                        ret = id;
                        cont.Templates.DeleteOnSubmit(check);
                        cont.SubmitChanges();
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        #endregion Template

        #region TemplatePattern

        public static List<TemplatePattern> GetTemplatePatternList()
        {
            var list = new List<TemplatePattern>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    list = cont.TemplatePatterns.ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static List<TemplatePattern> GetTemplatePatternList(int templateID)
        {
            var list = new List<TemplatePattern>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    list = cont.TemplatePatterns.Where(tp => tp.TemplateID == templateID).ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static int InsertTemplatePattern(TemplatePattern obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    cont.TemplatePatterns.InsertOnSubmit(obj);
                    cont.SubmitChanges();
                    ret = obj.ID;
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int UpdateTemplatePatterns(int templateid, List<TemplatePattern> list)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var i = DeleteTemplatePatterns(templateid);
                    if (i > 0)
                    {
                        list.ForEach(tp => { InsertTemplatePattern(tp); });
                        return templateid;
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int DeleteTemplatePatterns(int templateid)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.TemplatePatterns.Where(c => c.TemplateID == templateid);
                    if (check != null)
                    {
                        cont.TemplatePatterns.DeleteAllOnSubmit(check);
                        cont.SubmitChanges();
                    }
                    ret = templateid;
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        #endregion TemplatePattern

        #region Governerate

        public static List<Governerate> GetGovernerateList()
        {
            var list = new List<Governerate>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    list = cont.Governerates.ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static Governerate GetGovernerate(string name)
        {
            Governerate obj = null;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    if (!String.IsNullOrEmpty(name))
                        obj = cont.Governerates.FirstOrDefault(g => g.Name.Trim() == name.Trim());
                }
            }
            catch (Exception ex) { }
            return obj;
        }

        public static int InsertGovernerate(Governerate obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    cont.Governerates.InsertOnSubmit(obj);
                    cont.SubmitChanges();
                    ret = obj.ID;
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int UpdateGovernerate(Governerate obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.Governerates.FirstOrDefault(c => c.ID == obj.ID);
                    if (check != null)
                    {
                        check.Name = obj.Name;
                        check.Latitude = obj.Latitude;
                        check.Longitude = obj.Longitude;

                        cont.SubmitChanges();
                        ret = check.ID;
                    }
                    else
                        return InsertGovernerate(obj);
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int DeleteGovernerate(int id)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.Governerates.FirstOrDefault(c => c.ID == id);
                    if (check != null)
                    {
                        ret = id;
                        cont.Governerates.DeleteOnSubmit(check);
                        cont.SubmitChanges();
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        #endregion Governerate

        #region Area

        public static List<Area> GetAreaList()
        {
            var list = new List<Area>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    list = cont.Areas.ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static Area GetArea(string name)
        {
            Area obj = null;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    if (!String.IsNullOrEmpty(name))
                        obj = cont.Areas.FirstOrDefault(a => a.Name.Trim() == name.Trim());
                }
            }
            catch (Exception ex) { }
            return obj;
        }

        public static int InsertArea(Area obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    cont.Areas.InsertOnSubmit(obj);
                    cont.SubmitChanges();
                    ret = obj.ID;
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int UpdateArea(Area obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.Areas.FirstOrDefault(c => c.ID == obj.ID);
                    if (check != null)
                    {
                        check.Name = obj.Name;
                        check.Latitude = obj.Latitude;
                        check.Longitude = obj.Longitude;

                        cont.SubmitChanges();
                        ret = check.ID;
                    }
                    else
                        return InsertArea(obj);
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int DeleteArea(int id)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.Areas.FirstOrDefault(c => c.ID == id);
                    if (check != null)
                    {
                        ret = id;
                        cont.Areas.DeleteOnSubmit(check);
                        cont.SubmitChanges();
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        #endregion Area

        #region SignTemplate

        public static List<SignTemplate> GetSignLightPatternsList(int signID)
        {
            var list = new List<SignTemplate>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    list = cont.SignTemplates.Where(st => st.SignID == signID).ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static List<SignTemplate> GetTemplateSignesList(int TemplateID)
        {
            var list = new List<SignTemplate>();
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    list = cont.SignTemplates.Where(st => st.TemplateID == TemplateID).ToList();
                }
            }
            catch (Exception ex) { }
            return list;
        }

        public static SignTemplate GetSignTemplate(int signID)
        {
            SignTemplate obj = null;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    obj = cont.SignTemplates.FirstOrDefault(st => st.SignID == signID);
                }
            }
            catch (Exception ex) { }
            return obj;
        }

        public static int InsertSignLightPattern(SignTemplate obj)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    cont.SignTemplates.InsertOnSubmit(obj);
                    cont.SubmitChanges();
                    ret = obj.ID;
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int DeleteSignLightPattern(int id)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.SignTemplates.FirstOrDefault(c => c.ID == id);
                    if (check != null)
                    {
                        ret = id;
                        cont.SignTemplates.DeleteOnSubmit(check);
                        cont.SubmitChanges();
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        public static int DeleteSignLightPatterns(int signID)
        {
            int ret = 0;
            try
            {
                using (TraficLightSignesDataContext cont = new TraficLightSignesDataContext())
                {
                    var check = cont.SignTemplates.FirstOrDefault(c => c.SignID == signID);
                    if (check != null)
                    {
                        ret = signID;
                        cont.SignTemplates.DeleteOnSubmit(check);
                        cont.SubmitChanges();
                    }
                }
            }
            catch (Exception ex) { }
            return ret;
        }

        #endregion SignTemplate

        #region Helpers

        public static string GetObjectJsonString(object _obj)
        {
            var _jsonArrString = JsonConvert.SerializeObject(_obj, Formatting.Indented,
                                new JsonSerializerSettings()
                                {
                                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                }
                            );
            return _jsonArrString;
        }

        public static T GetJsonObject<T>(string jsonString)
        {
            try
            {
                var list = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
                return list;
            }
            catch (Exception ex)
            { }
            var dummyList = JsonConvert.DeserializeObject<T>(string.Empty, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            return dummyList;
        }

        public static T GetJsonNestedObject<T>(string jsonString)
        {
            var list = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize
            });

            return list;
        }

        #endregion Helpers

        public class SignWithLightPattern
        {
            public int TemplateID { get; set; }
            public int LightPatternID { get; set; }
            public int GovernerateID { get; set; }
            public int AreaID { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
            public string IPAddress { get; set; }
            public string Address { get; set; }
            public string Longitude { get; set; }
            public string Latitude { get; set; }
            public int Red { get; set; }
            public int Green { get; set; }
            public int Amber { get; set; }
        }
    }
}