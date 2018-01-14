using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebAPI.Models;

namespace WebAPI.Filters
{
    public class UserAuthorizationFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            IEnumerable<string> auths;
            var auth = string.Empty;
            if (actionContext.Request.Headers.TryGetValues("AuthToken", out auths))
                auth = auths.FirstOrDefault();

            User userWithToken = UserRepository.Users.Select(t => t.Value)
                .FirstOrDefault(u => u.AuthToken == auth);

            int id = Convert.ToInt32(actionContext.ControllerContext.RouteData.Values["id"]);

            if (userWithToken == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No rights to access this resource");
            }
            else if (userWithToken.Id != id)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, "Forbidden");
            }
        }
    }
}