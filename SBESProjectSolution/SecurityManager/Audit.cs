using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
   public class Audit : IDisposable
    {
        private static EventLog customLog = null;           // u konstruktoru kreiramo objekat
        const string SourceName = "SecurityManager.Audit";
        const string LogName = "MySecTest";       // mozemo bilo koji naziv

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


        public static void AuthenticationSuccess(string userName)
        {

            if (customLog != null)
            {
                string UserAuthenticationSuccess = AuditEvents.AuthenticationSuccess;   // preuzimanje poruke
                string message = String.Format(UserAuthenticationSuccess, userName);    // ubacivanje username-a u poruku
                customLog.WriteEntry(message);                                          // upisivanje poruke u log
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthenticationSuccess));
            }
        }

        public static string AuthorizationSuccess(string userName, string serviceName)
        {
            
            if (customLog != null)
            {
                string AuthorizationSuccess = AuditEvents.AuthorizationSuccess;
                string message = String.Format(AuthorizationSuccess, userName, serviceName);    // prosledjujemo i naziv metode
                customLog.WriteEntry(message);
                return message;
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthorizationSuccess));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="serviceName"> should be read from the OperationContext as follows: OperationContext.Current.IncomingMessageHeaders.Action</param>
        /// <param name="reason">permission name</param>
       public static void AuthorizationFailed(string userName, string serviceName, string reason)
       {
            if (customLog != null)
            {
                string AuthorizationFailed = AuditEvents.AuthorizationFailed;
                string message = String.Format(AuthorizationFailed, userName, serviceName, reason);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthorizationFailed));
            }
       }

        public static void EventSuccess(Event ev)
        {
            string source = "";
            if (ev.Source != null)
                source = ev.Source.ToString();
            if (customLog != null)
            {
                string sendEventSuccess = AuditEvents.EventSuccess;   // preuzimanje poruke
                string message = String.Format(sendEventSuccess, ev.Criticallity.ToString(), ev.Timestamp.ToString(),
                    source, ev.Message, ev.State.ToString());
                customLog.WriteEntry(message);                                          // upisivanje poruke u log
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.EventSuccess));
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
