using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class BackupAudit : IDisposable
    {
        private static EventLog customLog = null;           // u konstruktoru kreiramo objekat
        const string SourceName = "SecurityManager.BackupAudit";
        const string LogName = "BackupTest";       // mozemo bilo koji naziv

        static BackupAudit()
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

        public static void EventSuccess(string formatedEv)
        {

            if (customLog != null)
            {
                //string sendEventSuccess = BackupAuditEvents.EventSuccess;   // preuzimanje poruke
                //string message = String.Format(sendEventSuccess, formatedEv);

                //nema potrebe za onim gore jer vec preuzimamo formatiranu poruku
                customLog.WriteEntry(formatedEv);                                          // upisivanje poruke u log
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)BackupAuditEventTypes.EventSuccess)); 
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
