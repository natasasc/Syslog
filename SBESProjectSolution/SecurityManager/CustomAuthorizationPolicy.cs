using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class CustomAuthorizationPolicy : IAuthorizationPolicy
    {
        public CustomAuthorizationPolicy()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }
        public string Id
        {
            get;
        }

        // Evaluate se poziva pre IsInRole
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            // Na kontekst tj thread wcf konekcije postavlja identitet klijenta
            // Klijenta kastovati na CustomPrincipal - > sto se dalje koristi u IsInRole

            // Da li na kontekstu uopste postoje neki identiteti?
            if (!evaluationContext.Properties.TryGetValue("Identities", out object list))
                return false;

            IList<IIdentity> identities = list as IList<IIdentity>;
            if (list == null || identities.Count <= 0)
            {
                return false;
            }

            // Na kontekst postavljamo novo polje (principal) i u njega stavljamo nas CustomPrincipal
            evaluationContext.Properties["Principal"] = new CustomPrincipal((WindowsIdentity)identities[0]);
            return true;
        }
    }
}
