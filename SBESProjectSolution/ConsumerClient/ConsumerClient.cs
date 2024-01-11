using Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace ConsumerClient
{
    public class ConsumerClient: ChannelFactory<ISyslogServer>, ISyslogServer, IDisposable
    {
        ISyslogServer factory;
        public ConsumerClient(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public void Subscribe()
        {
            try
            {
                factory.Subscribe();
                Console.WriteLine("Successfuly subscribed to security event logs.");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Subscribe : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Subscribe : {0}", e.Message);
            }
        }
        public void Update(int key, MessageState ms)
        {
            
            try
            {
                try
                {
                    Dictionary<int, Event> events = factory.Read();
                    Console.WriteLine("Recieved logs:\n");
                    foreach (var e in events)
                    {
                        Console.WriteLine(e.Key.ToString() + ":" + e.Value);
                    }
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine("Error while trying to Read : {0}", e.Detail.Message);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while trying to Read : {0}", e.Message);
                    return;
                }

                Console.WriteLine("Choose key of event log: ");
                while (!Int32.TryParse(Console.ReadLine(), out key))
                {
                    Console.WriteLine("Key value must be a number...");
                }

                Console.WriteLine("Chose new State of log(OPEN/CLOSE):");
                string state = Console.ReadLine();
                while(state.ToLower() != "open" && state.ToLower() != "close") 
                {
                    Console.WriteLine("State must be Close or Open!"); 
                    state = Console.ReadLine();
                }

                if (state.ToLower() == "open") ms = MessageState.OPEN;
                else ms = MessageState.CLOSE;
                
                factory.Update(key,ms);
                Console.WriteLine("Updated event with key:{0} to state {1} successfully",key,ms);
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Update : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Update : {0}", e.Message);
            }
        }
        public void Delete(int key)
        {

            try
            {
                try
                {
                    Dictionary<int, Event> events = factory.Read();
                    Console.WriteLine("Recieved logs:\n");
                    foreach (var e in events)
                    {
                        Console.WriteLine(e.Key.ToString() + ":" + e.Value);
                    }
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine("Error while trying to Read : {0}", e.Detail.Message);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while trying to Read : {0}", e.Message);
                    return;
                }

                Console.WriteLine("Choose key of event log to delete:");
                while (!Int32.TryParse(Console.ReadLine(), out key))
                    {
                    Console.WriteLine("Key value must be a number..."); 
                    }
                factory.Delete(key);
                Console.WriteLine("Deleted event log with key {0} successfully.",key);
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Delete : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Delete : {0}", e.Message);
            }
        }
        public void ManagePermission(bool isAdd, string rolename, params string[] permissions)
        {
            try
            {
                factory.ManagePermission(isAdd, rolename, permissions);
                Console.WriteLine("Manage allowed");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to MenagePermission : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ManagePermission : {0}", e.Message);
            }
        }
        public void ManageRoles(bool isAdd, string rolename)
        {
            try
            {
                factory.ManageRoles(isAdd, rolename);
                Console.WriteLine("Manage allowed");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to ManageRoles : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ManageRoles : {0}", e.Message);
            }
        }
        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }
        public Dictionary<int, Event> Read()
        {
            try
            {
                Dictionary<int, Event> events = factory.Read();
                Console.WriteLine("Recieved logs:\n");
                foreach (var e in events)
                {
                    Console.WriteLine(e.Key.ToString() + ":" + e.Value);
                }
                return events;
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Read : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Read : {0}", e.Message);
            }
            return null;
        }
    }
    
}
