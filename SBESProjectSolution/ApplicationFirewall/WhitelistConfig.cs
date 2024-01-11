using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AFManager;
using Contracts;

namespace ApplicationFirewall
{
    public class WhitelistConfig
    {
        internal static List<int> validPorts = new List<int>();
        internal static List<string> validProtocols = new List<string>();

        static WhitelistConfig()
        {

            validPorts.Add(9999);
            validPorts.Add(9998);
            validPorts.Add(9997);

            validProtocols.Add("TCP");
            validProtocols.Add("UDP");
        }

        public bool AddPort(int port, out Event ev)
        {
            ev = null;

            if (!validPorts.Contains(port))
            {
                validPorts.Add(port);
                Consumer c = new Consumer(Formatter.ParseName(WindowsIdentity.GetCurrent().Name), WindowsIdentity.GetCurrent().User.ToString());
                ev = new Event(CriticallityLevel.GREEN_ALERT, DateTime.Now, c, "AddPort" + port.ToString(), MessageState.OPEN);

                try
                {
                    Audit.AddPortSuccess(port.ToString());
                    return true;
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        public bool RemovePort(int port, out Event ev)
        {
            ev = null;
            if (validPorts.Contains(port))
            {
                validPorts.Remove(port);
                Consumer c = new Consumer(Formatter.ParseName(WindowsIdentity.GetCurrent().Name), WindowsIdentity.GetCurrent().User.ToString());
                ev = new Event(CriticallityLevel.YELLOW_ALERT, DateTime.Now, c, "RemovePort" + port.ToString(), MessageState.CLOSE);

                try
                {
                    Audit.RemovePortSuccess(port.ToString());
                    return true;
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        public bool AddProtocol(string protocol, out Event ev)
        {
            ev = null;
            protocol = protocol.ToUpper();

            if (!validProtocols.Contains(protocol))
            {
                validProtocols.Add(protocol);
                Consumer c = new Consumer(Formatter.ParseName(WindowsIdentity.GetCurrent().Name), WindowsIdentity.GetCurrent().User.ToString());
                ev = new Event(CriticallityLevel.GREEN_ALERT, DateTime.Now, c, "AddProtocol" + protocol, MessageState.OPEN);

                try
                {
                    Audit.AddProtocolSuccess(protocol);
                    return true;
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }

        public bool RemoveProtocol(string protocol, out Event ev)
        {
            ev = null;
            protocol = protocol.ToUpper();

            if (validProtocols.Contains(protocol))
            {
                validProtocols.Remove(protocol);
                Consumer c = new Consumer(Formatter.ParseName(WindowsIdentity.GetCurrent().Name), WindowsIdentity.GetCurrent().User.ToString());
                ev = new Event(CriticallityLevel.RED_ALERT, DateTime.Now, c, "RemoveProtocol" + protocol, MessageState.CLOSE);

                try
                {
                    Audit.RemoveProtocolSuccess(protocol);
                    return true;
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }
    }
}
