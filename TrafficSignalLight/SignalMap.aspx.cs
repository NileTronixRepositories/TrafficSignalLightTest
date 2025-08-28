using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrafficSignalLight.DB;

namespace TrafficSignalLight
{
    public partial class SignalMap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string SetLocation(string obj)
        {
            return "Ok";
        }
    }
}