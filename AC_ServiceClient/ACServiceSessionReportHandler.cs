using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AC_ServiceClient.ACServiceReference;
using AC_SessionReport;

namespace AC_ServiceClient
{
    public class ACServiceSessionReportHandler : ISessionReportHandler
    {
        public void HandleReport(SessionReport report)
        {
            ACServiceClient client = new ACServiceClient();
            client.PostResult(report);
        }
    }
}
