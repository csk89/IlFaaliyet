using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turkok.Core.Service.CrudService;
using Turkok.Model;

namespace Turkok.Core.Service
{
    public interface IClaimAuthorisationService : ICrudService<ClaimAuthorisation>
    {
        List<AuthorisationResource> GetRoleAuthorisations(string roleName);
        bool IsActionAuthorizedForRole(string roleName, string resourceName, string actionName);
        bool CheckActionAuthorisationVariables(string roleName, string resource, string action);
        ClaimAuthorisation FindRoleClaim(string roleName, string resourceName, string actionName);
    }
}
