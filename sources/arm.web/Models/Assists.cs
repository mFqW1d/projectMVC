using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace arm_repairs_project.Models
{
    public static class Assists
    {
        public static List<ApplicationUser> GetUsersInRole(string roleName)
        {
            List<ApplicationUser> usersInRole = new List<ApplicationUser>();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var role = roleManager.FindByName(roleName);
            if (role.Users.Any())
            {
                var usersByRole = roleManager.FindByName(roleName).Users.First();
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    usersInRole = db.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(usersByRole.RoleId)).ToList();
                }
            }
            return usersInRole;
        }

        public class DecimalModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                ValueProviderResult valueResult = bindingContext.ValueProvider
                    .GetValue(bindingContext.ModelName);

                ModelState modelState = new ModelState { Value = valueResult };

                object actualValue = null;

                if (valueResult.AttemptedValue != string.Empty)
                {
                    try
                    {
                        actualValue = Convert.ToDecimal(valueResult.AttemptedValue, CultureInfo.CurrentCulture);
                    }
                    catch (FormatException e)
                    {
                        modelState.Errors.Add(e);
                    }
                }

                bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

                return actualValue;
            }
        }
    }
}