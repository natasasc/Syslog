using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFManager
{
    public class AuditCC : IDisposable
    {
        private static EventLog customLog = null;           // u konstruktoru kreiramo objekat
        const string SourceName = "AFManager.AuditCC";
        const string LogName = "AFCCSecTest";       // mozemo bilo koji naziv

        static AuditCC()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }


        public static void InvalidPort(string val)
        {

            if (customLog != null)
            {
                string WCInvalidPort = AuditCCEvents.InvalidPort;   // preuzimanje poruke
                string message = String.Format(WCInvalidPort, val);    // ubacivanje port-a u poruku
                customLog.WriteEntry(message);                                          // upisivanje poruke u log
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditCCEventTypes.InvalidPort));
            }
        }
        public static void InvalidProtocol(string val)
        {
            if (customLog != null)
            {
                string WCInvalidProtocol = AuditCCEvents.InvalidProtocol;
                string message = String.Format(WCInvalidProtocol, val);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditCCEventTypes.InvalidProtocol));
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
