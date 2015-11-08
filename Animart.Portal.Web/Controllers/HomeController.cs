using System.Web.Mvc;

namespace Animart.Portal.Web.Controllers
{
    public class HomeController : PortalControllerBase
    {
        public ActionResult Index()
        { 
            return View("~/App/Main/views/layout/layout.cshtml"); //Layout of the angular application.
        }
	}
}