using AC_SessionReport;
using System;
using System.Configuration;
using System.Reflection;

namespace AC_SessionReportPlugin
{
    public static class ReportHandlerLoader
    {
        public static void LoadHandler(ReportPlugin plugin)
        {
            string sessionReportHandlerType = ConfigurationManager.AppSettings["SessionReportHandler"];
            if (!string.IsNullOrEmpty(sessionReportHandlerType))
            {
                foreach (string handlerTypeStr in sessionReportHandlerType.Split(';'))
                {
                    string[] typeInfo = handlerTypeStr.Split(',');
                    Assembly assembly = Assembly.Load(typeInfo[1]);
                    Type type = assembly.GetType(typeInfo[0]);
                    ISessionReportHandler reportHandler = (ISessionReportHandler)Activator.CreateInstance(type);
                    plugin.SessionReportHandlers.Add(reportHandler);
                }
            }
        }
    }
}
