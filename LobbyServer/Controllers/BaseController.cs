using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class BaseController : Controller
    {
        CultureInfo _culture;
        public CultureInfo Culture
        {
            get { return _culture; }
            private set { _culture = value; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Debug.WriteLine("OnActionExecuting");

            // Select culture from routing data.
            var culturePassed = (string)RouteData.Values["culture"];
            var culturesAcceptable = new[] { "ja-JP", "en-US" };
            var cultureSelected = "";
            if (culturesAcceptable.Contains(culturePassed))
                cultureSelected = culturePassed;
            else
                cultureSelected = culturesAcceptable.FirstOrDefault();
            Debug.WriteLine("Culture Passed: {0}, Cultures Acceptable: {1}, Culture Selected: {2}",
                culturePassed, culturesAcceptable, cultureSelected);

            // Set thread and controller culture.
            var cultureInfo = new CultureInfo(cultureSelected);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Culture = cultureInfo;

            // Redirect User if culture is not specified.
            if (cultureSelected != culturePassed)
            {
                Debug.WriteLine("Redirecting...");
                filterContext.Result = RedirectToRoute("Default", new { culture = cultureSelected, controller = "Home", action = "Index" });
                return;
            }

            Debug.WriteLine("Culture: {0}", Thread.CurrentThread.CurrentCulture);

            base.OnActionExecuting(filterContext);
        }
    }
}