using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using AC_SessionReport;

namespace AC_Service
{
    [ServiceContract]
    public interface IACService
    {
        [OperationContract]
        void PostResult(SessionReport report);
    }
}
