using AFManager;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationFirewall
{
    public class ClientService : IClientService
    {
        string protocol;
        int port;

        public bool CheckPP(string protocol, string port, Consumer c)
        {
            bool ret = false;

            this.protocol = protocol.ToUpper();

            this.port = Int32.Parse(port);

            if (WhitelistConfig.validPorts.Contains(this.port) && WhitelistConfig.validProtocols.Contains(this.protocol))
                ret = true;
            else
            {
                // ovde ide logovanje
                Event ev = new Event(CriticallityLevel.YELLOW_ALERT, DateTime.Now, c, "DetectedConsumerClientEvent", MessageState.CLOSE);

                if (!WhitelistConfig.validPorts.Contains(this.port))
                {
                    try
                    {
                        AuditCC.InvalidPort(port.ToString());
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

                if (!WhitelistConfig.validProtocols.Contains(this.protocol))
                {
                    try
                    {
                        AuditCC.InvalidProtocol(protocol);
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

                Program.afccProxy.sendEvent(ev);
            }

            return ret;
        }


    }
}
