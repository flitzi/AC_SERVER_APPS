using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using AC_DBFillerEF;

namespace AC_Service
{
    public class AuthService : IAuthService
    {
        public string Auth(string minSR, string id)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            try
            {
                int minSRnum;
                if (!int.TryParse(minSR, out minSRnum))
                {
                    minSRnum = 0; //TODO find a good default
                }

                using (AC_DBEntities entities = new AC_DBEntities())
                {
                    Driver driver = entities.Drivers.FirstOrDefault(d => d.SteamId == id);

                    if (driver != null && driver.IncidentCount > 0 && driver.Distance > 200000
                        && driver.Distance / driver.IncidentCount < minSRnum)
                    {
                        return "Deny|Your Safety Rating is too low to enter this server.";
                    }
                }
            }
            finally { }
            return "Allow";
        }
    }
}
