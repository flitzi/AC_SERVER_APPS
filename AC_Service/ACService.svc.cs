using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using AC_DBFillerEF;
using AC_SessionReport;

namespace AC_Service
{
    public class ACService : IACService
    {
        public void PostResult(SessionReport report)
        {
            new DBFiller().HandleReport(report);
        }
    }
}
