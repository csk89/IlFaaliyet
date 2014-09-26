using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Turkok.Core.Repository;
using Turkok.Model;

namespace Gvm.Infra
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            var roles = context.Principal.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            
            var resource = context.Resource.First();
            var actions = context.Action.ToList();

            using (var db = new DataContext())
            {
                foreach (var role in roles)
                {
                    var r = role;
                    var roleAuthorised = true;
                    foreach (var action in actions)
                    {
                        var a = action;
                        var isAuthorised = db.ClaimAuthorisations
                                             .Any(x => x.ClaimType == ClaimTypes.Role && x.Claim == r.Value && x.Resource == resource.Value && x.Action == a.Value);

                        if (isAuthorised == false)
                        {
                            roleAuthorised = false;
                            break;
                        }
                    }

                    if (roleAuthorised) return true;
                }
            }

            return false;
        }
    }
}