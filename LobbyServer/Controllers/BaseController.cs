using System;
using System.Collections.Concurrent;
using System.Configuration;
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
            /*if (Request.Url.Authority == "localhost:50731")
            {
                // 301 Redirect to 192.168.100.3 from localhost
                Response.Redirect("http://192.168.100.3:50731" + Request.Url.AbsolutePath);
                ViewBag.CanonicalUrl = "http://192.168.100.3:50731" + Request.Url.AbsolutePath;
                return;
            }
            if (Request.Url.Authority == "v2.xn--sckyeodz36l941b.com" || Request.Url.Authority == "v2.人狼ゲーム.com")
            {
                Response.Redirect("http://werewolfgame.apwei.com" + Request.Url.AbsolutePath);
                ViewBag.CanonicalUrl = "http://werewolfgame.apwei.com" + Request.Url.AbsolutePath;
            }*/

            if (RouteData.Values["culture"] != null)
            {
                var authority = ConfigurationManager.AppSettings["CanonicalAuthority"];// "localhost:50731";//"werewolfgame.apwei.com";
                if (RouteData.Values["culture"].Equals("ja-JP")
                    && !(RouteData.Values["controller"].Equals("Game") && RouteData.Values["action"].Equals("Play")))
                    authority = ConfigurationManager.AppSettings["CanonicalAuthority.ja-JP"]; //"192.168.100.3:50731";//"xn--sckyeodz36l941b.com";
                var canonicalUrl = "http://" + authority + Request.Url.AbsolutePath;
                //if (Request.Url.Authority != authority)
                //    Response.Redirect(canonicalUrl);
                ViewBag.CanonicalAuthority = authority;
                ViewBag.CanonicalUrl = canonicalUrl;

            }
            //if(Request)
            /*Debug.WriteLine("OnActionExecuting");

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

            Debug.WriteLine("Culture: {0}", Thread.CurrentThread.CurrentCulture);*/

            // Select culture from routing data.
            var culturePassed = (string)RouteData.Values["culture"];
            var controllerPassed = (string)RouteData.Values["controller"];
            var actionPassed = (string)RouteData.Values["action"];
            var postUrlPassed = (string)RouteData.Values["postUrl"];

            var culturesAcceptable = new[] { "ja-JP", "en-US" };
            var cultureSelected = culturesAcceptable.FirstOrDefault(c => c.Equals(culturePassed, System.StringComparison.OrdinalIgnoreCase))
                ?? culturesAcceptable.FirstOrDefault();

            // Set thread and controller culture.
            var cultureInfo = new CultureInfo(cultureSelected);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Culture = cultureInfo;

            // Redirect User if culture is not specified.
            if (cultureSelected != culturePassed)
            {
                filterContext.Result = RedirectToRoute("Default", new { culture = cultureSelected, controller = controllerPassed, action = actionPassed });
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Returns 503 Service Unavailable.
        /// Very similar with HttpNotFound().
        /// </summary>
        /// <returns></returns>
        protected ActionResult HttpServiceUnavailable()
        {
            Response.StatusCode = 503;
            Response.Status = "503 Service Unavailable";
            return null;
        }

        /// <summary>
        /// Remembers rendering states for SingletonAction().
        /// </summary>
        ConcurrentDictionary<string, int> _rendering = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Executes a singleton action in this instance.
        /// Especially useful for actions which has heavy workload like calling API.
        /// Not suitable for views which has heavy workload like calling another action in Razor.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected ActionResult SingletonAction(Func<ActionResult> func, params object[] keys)
        {
            var keyInController = String.Join("-", keys);
            var key = string.Format("{0}-{1}-{2}",
                RouteData.Values["controller"],
                RouteData.Values["action"],
                keyInController);
            Debug.WriteLine(key);
            if (_rendering.TryAdd(key, 1))
            {
                try
                {
                    return func();
                }
                finally
                {
                    int a = 0;
                    _rendering.TryRemove(key, out a);
                }
            }
            else
            {
                Debug.WriteLine("Discarding... (dic)");
                return HttpServiceUnavailable();
            }
        }
    }
}