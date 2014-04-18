﻿using System;
using System.Collections.Concurrent;
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
        /// Executes a singleton action in this instance.
        /// Especially useful for actions which has heavy workload like calling API.
        /// Not suitable for views which has heavy workload like calling another action in Razor.
        /// </summary>
        /// <param name="locked"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        protected ActionResult SingletonAction(ref int locked, Func<ActionResult> func)
        {
            if (0 == Interlocked.Exchange(ref locked, 1))
            {
                try
                {
                    return func();
                }
                finally
                {
                    Interlocked.Exchange(ref locked, 0);
                }
            }
            else
            {
                Debug.WriteLine("Discarding...");
                return HttpServiceUnavailable();
            }
        }

        protected ActionResult SingletonAction(ConcurrentDictionary<string, int> locked, string key, Func<ActionResult> func)
        {
            if (locked.TryAdd(key, 1))
            {
                try
                {
                    return func();
                }
                finally
                {
                    int a = 0;
                    locked.TryRemove(key, out a);
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