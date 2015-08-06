using System;
using System.IO;
using System.Reflection;
using AC_SessionReport;
using Newtonsoft.Json;

namespace AC_SessionReportPlugin
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
            StreamWriter writer =
                new StreamWriter(
                    Path.Combine(
                        dir,
                        new DateTime(report.Timestamp, DateTimeKind.Utc).ToString("yyyyMMdd_HHmmss") + "_" + report.TrackName + "_"
                        + report.SessionName + ".json"));
            writer.Write(output);
            writer.Close();
            writer.Dispose();
        }
    }
}