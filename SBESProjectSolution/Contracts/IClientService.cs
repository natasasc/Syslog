using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IClientService
    {
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        bool CheckPP(string protocol, string port, Consumer c);
    }
}
