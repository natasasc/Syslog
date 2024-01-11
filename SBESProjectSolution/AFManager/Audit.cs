using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFManager
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;           // u konstruktoru kreiramo objekat
        const string SourceName = "AFManager.Audit";
        const string LogName = "AFSecTest";       // mozemo bilo koji naziv

        static Audit()
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


        public static void AddPortSuccess(string val)
        {

            if (customLog != null)
            {
                string WCAddPortSuccess = AuditEvents.AddPortSuccess;   // preuzimanje poruke
                string message = String.Format(WCAddPortSuccess, val);    // ubacivanje username-a u poruku
                customLog.WriteEntry(message);                                          // upisivanje poruke u log
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AddPortSuccess));
            }
        }

        public static void RemovePortSuccess(string val)
        {

            if (customLog != null)
            {
                string WCRemovePortSuccess = AuditEvents.RemovePortSuccess;
                string message = String.Format(WCRemovePortSuccess, val);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.RemovePortSuccess));
            }
        }


        public static void AddProtocolSuccess(string val)
        {
            if (customLog != null)
            {
                string WCAddProtocolSuccess = AuditEvents.AddProtocolSuccess;
                string message = String.Format(WCAddProtocolSuccess, val);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AddProtocolSuccess));
            }
        }

        public static void RemoveProtocolSuccess(string val)
        {
            if (customLog != null)
            {
                string WCRemoveProtocolSuccess = AuditEvents.RemoveProtocolSuccess;
                string message = String.Format(WCRemoveProtocolSuccess, val);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.RemoveProtocolSuccess));
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
