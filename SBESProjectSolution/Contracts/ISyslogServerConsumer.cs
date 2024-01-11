using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ISyslogServer
    {
        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Subscribe();

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        Dictionary<int, Event> Read();

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Update(int key, MessageState ms);  // prosledjujemo kljuc i izmenjen objekat

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Delete(int key);  // prosledjujemo kljuc

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void ManagePermission(bool isAdd, string rolename, params string[] permissions);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void ManageRoles(bool isAdd, string rolename);
    }
}
