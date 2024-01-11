using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class CustomAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            //ReadOnlyCollection<IAuthorizationPolicy> policies = GetAuthorizationPolicies(operationContext);
            //ServiceSecurityContext ssc = new ServiceSecurityContext(policies);
            //operationContext.IncomingMessageProperties.Security.ServiceSecurityContext = ssc;
            //CustomPrincipal principal = ssc.AuthorizationContext.Properties["Principal"] as CustomPrincipal;
           
            
            // Ovde ne mozemo da skidamo sa thread-a jer se ova metoda poziva pre kreiranja thread-a

            CustomPrincipal principal = operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"] as CustomPrincipal;
            

            return principal.IsInRole("Read");

            //if (!retValue)
            //{
            //    try
            //    {
            //        Audit.AuthorizationFailed(Formatter.ParseName(principal.Identity.Name),
            //            OperationContext.Current.IncomingMessageHeaders.Action, "Need Read permission.");
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //    }
            //}

            //return retValue;

        }
    }
}
