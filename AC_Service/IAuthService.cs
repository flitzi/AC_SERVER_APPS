using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace AC_Service
{
    [ServiceContract]
    public interface IAuthService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "Auth/{minSR}/{id}")]
        string Auth(string minSR, string id);
    }
}