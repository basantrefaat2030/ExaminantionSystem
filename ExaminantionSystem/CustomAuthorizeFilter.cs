using ExaminationSystem.Models.Enums;
using ExaminationSystem.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ExaminationSystem.Filters
{
    public class CustomAuthorizeFilter : ActionFilterAttribute
    {
        RoleFeatureService _roleFeatureService;
        Feature _feature;
        public CustomAuthorizeFilter(RoleFeatureService roleFeatureService, Feature feature)
        {
            _roleFeatureService = roleFeatureService;
            _feature = feature;
        }  
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleID = context.HttpContext.User.FindFirst(ClaimTypes.Role);
            if (roleID is null || string.IsNullOrEmpty(roleID.Value))
            {
                throw new UnauthorizedAccessException("You are not authorized to access this resource.");
            }
            var role = (Models.Enums.Role)Enum.Parse(typeof(Models.Enums.Role), roleID.Value, true);
            
            if(! _roleFeatureService.CheckFeatureAccess(role, _feature))
            {
                throw new UnauthorizedAccessException("You do not have permission to access this feature.");
            }

            base.OnActionExecuting(context);
            
        }
    }
}
