using System.Web.Mvc;

namespace wsREST__Aires002.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Sistema ALPHILIA";

            return View();
        }
    }
}
