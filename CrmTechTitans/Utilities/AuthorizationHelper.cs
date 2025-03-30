using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrmTechTitans.Utilities
{
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Checks if the current user can edit data (Administrator or Editor)
        /// </summary>
        public static bool CanEditData(this Controller controller)
        {
            if (controller.User == null)
                return false;

            return controller.User.IsInRole(UserRoles.Administrator) || 
                   controller.User.IsInRole(UserRoles.Editor);
        }

        /// <summary>
        /// Checks if the current user is an administrator
        /// </summary>
        public static bool IsAdministrator(this Controller controller)
        {
            if (controller.User == null)
                return false;

            return controller.User.IsInRole(UserRoles.Administrator);
        }
    }
} 