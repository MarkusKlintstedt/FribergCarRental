using FribergCarRental.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FribergCarRental.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IAuthService authService;

        public BaseController(IAuthService authService)
        {
            this.authService = authService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.CurrentUser = authService.GetUserName();
            base.OnActionExecuting(context);
        }
    }
}
