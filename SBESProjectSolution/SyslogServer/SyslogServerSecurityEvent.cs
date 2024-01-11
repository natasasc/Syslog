using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SecurityManager;

namespace SyslogServer
{
    public class SyslogServerSecurityEvent : ISyslogServerSecurityEvent
    {
        public static Mutex mutex = new Mutex();
        public void sendEvent(Event ev)
        {
            mutex.WaitOne();
            
            Database.events[Database.eventKey] = ev;
            Console.WriteLine("Event successfully added to database.");
            Database.eventKey++;
            mutex.ReleaseMutex();

            string source = "";
            if (ev.Source != null)
                source = ev.Source.ToString();
            string message = String.Format("{0}, {1}, {2}, {3}, {4}", ev.Criticallity.ToString(), ev.Timestamp.ToString(), source, ev.Message, ev.State.ToString());


            //TO DO: zakomentarisati liniju gde dodaje u formatedEvents
            //Program.sharedMutex.WaitOne();
            //Database.formatedEvents.Add(message);
            //Program.sharedMutex.ReleaseMutex();



            try
            {
                Audit.EventSuccess(ev);
                
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
