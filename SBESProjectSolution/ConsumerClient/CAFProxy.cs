using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerClient
{
    public class CAFProxy : ChannelFactory<IClientService>, IClientService, IDisposable
    {
        IClientService factory;

        public CAFProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public bool CheckPP(string protocol, string port, Consumer c)
        {
            bool ret = false;
            try
            {
                ret = factory.CheckPP(protocol, port, c);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }

            return ret;
        }
    }
}
