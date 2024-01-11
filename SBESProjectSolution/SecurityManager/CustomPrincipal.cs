using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class CustomPrincipal : IPrincipal
    {
        WindowsIdentity identity = null;
        public CustomPrincipal(WindowsIdentity windowsIdentity)
        {
            identity = windowsIdentity;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }

        public bool IsInRole(string permission)
        {
            foreach (IdentityReference group in this.identity.Groups)       // prolazimo kroz grupe korisnika
            {
                // Preko SecurityIdentifier-a dolazimo do imena grupe
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));
                string groupName = Formatter.ParseName(name.ToString());
                
                string[] permissions;
                if (RolesConfig.GetPermissions(groupName, out permissions)) // preko grupa dolazimo do permisija
                {
                    foreach (string item in permissions)
                    {
                        if (item.Equals(permission))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
