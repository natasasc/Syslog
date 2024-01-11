using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace SyslogServer
{
    public class SyslogServer : ISyslogServer
    {

        [PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public void Subscribe() 
        {
            WindowsIdentity windowsIdentity = Thread.CurrentPrincipal.Identity as WindowsIdentity;

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (!Database.subscribers.ContainsKey(windowsIdentity.User.ToString()))
            {
                Database.subscribers[windowsIdentity.User.ToString()] = 
                    new Consumer(windowsIdentity.Name, windowsIdentity.User.ToString());
                Console.WriteLine("Subscription successfully executed.");

            }
            else {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format("User {0} is already subscribed (time : {1}).", name, time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public void Delete(int key)
        {
            if (Database.events.ContainsKey(key))
            {
                Database.events.Remove(key);
            }
            else {

                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format("Event log with key {0} doesn't exist. (time : {1}).", key, time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
            
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Update")]
        public void Update(int key, MessageState ms)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Database.events.ContainsKey(key))
            {
                Database.events[key].Update(ms);
                Console.WriteLine("Update successfully executed.");


            }
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format("Event key doesn't exist (time : {0}).", time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }

        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public Dictionary<int, Event> Read()
        {
            WindowsIdentity windowsIdentity = Thread.CurrentPrincipal.Identity as WindowsIdentity;

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Database.subscribers.ContainsKey(windowsIdentity.User.ToString()))
            {
                Console.WriteLine("Read successfully executed.");
                return Database.events;

            }
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format("User {0} is not subscribed (time : {1}).", name, time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }


        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManagePermission(bool isAdd, string rolename, params string[] permissions)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Administrate"))     // provera da li korisnik ima odgovarajucu permisiju
            {
                Console.WriteLine("ManagePermission successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);

                    if (isAdd) // u pitanju je dodavanje
                    {
                        RolesConfig.AddPermissions(rolename, permissions);
                    }
                    else // u pitanju je brisanje
                    {
                        RolesConfig.RemovePermissions(rolename, permissions);
                    }
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
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "ManagePermission method needs Administrate permission.");
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " tried to call ManagePermission method. ManagePermission method needs Administrate permission.");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManageRoles(bool isAdd, string rolename)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Administrate"))     // provera da li korisnik ima odgovarajucu permisiju
            {
                Console.WriteLine("ManageRoles successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);

                    if (isAdd) // u pitanju je dodavanje
                    {
                        RolesConfig.AddRole(rolename);
                    }
                    else // u pitanju je brisanje
                    {
                        RolesConfig.RemoveRole(rolename);
                    }
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
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "ManageRoles method needs Administrate permission.");
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " tried to call ManageRoles method. ManageRoles method needs Administrate permission.");
            }
        }



    }
 }
