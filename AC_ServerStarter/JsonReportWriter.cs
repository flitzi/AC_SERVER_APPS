using AC_SessionReport;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace AC_ServerStarter
{
    public class JsonReportWriter : ISessionReportHandler
    {
        public void HandleReport(SessionReport report)
        {
            string output = JsonConvert.SerializeObject(report, Formatting.Indented);
            string dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "sessionresults");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            StreamWriter writer = new StreamWriter(Path.Combine(dir, new DateTime(report.TimeStamp, DateTimeKind.Utc).ToString("yyyyMMdd_HHmmss") + "_" + report.Type + ".json"));
            writer.Write(output);
            writer.Close();
            writer.Dispose();
        }
    }
}
