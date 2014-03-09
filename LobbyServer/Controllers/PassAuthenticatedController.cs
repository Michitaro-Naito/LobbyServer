using AuthUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    /// <summary>
    /// Authenticated by a signed Pass object.
    /// </summary>
    public class PassAuthenticatedController : BaseController
    {
        /// <summary>
        /// Cookie name of Pass object. User's browser remembers serialized Pass object for this name.
        /// </summary>
        protected const string PassCookieName = "EntryPass";

        /// <summary>
        /// Pass of User. If null, User is not authenticated.
        /// </summary>
        protected EntryPass ValidPass { get; private set; }
        protected string ValidPassString { get; private set; }



        protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)
        {
            GetPass();
            base.OnAuthentication(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Debug.WriteLine("OnException");

            if (!filterContext.ExceptionHandled)
            {
                // Handle Exception...
                var type = filterContext.Exception.GetType();
                if (type == typeof(InvalidPassException))
                {
                    Debug.WriteLine("InvalidPassException");
                    filterContext.Result = View("InvalidPassException");
                    filterContext.ExceptionHandled = true;
                }
            }

            base.OnException(filterContext);
        }

        public void TryLogin(string entryPassString)
        {
            // Tries to login using a token.
            EntryPass pass;
            try
            {
                pass = EntryPass.FromBase64EncodedJson(entryPassString);
            }
            catch
            {
                pass = null;
            }

            if (pass == null)
                return;

            if (pass.IsValid(ConfigurationManager.AppSettings["PublicKeyXmlString"], Request.Url.Authority))
            {
                // Valid. Stores it to Cookie.
                Debug.WriteLine("Valid. Storing...");
                Response.Cookies.Set(new HttpCookie(PassCookieName, pass.ToBase64EncodedJson()));
            }
        }

        void GetPass()
        {
            EntryPass pass = null;
            string passString = null;

            /*// Tries to retrieve Pass from QueryString.
            passString = Request["entryPassString"];
            if (passString != null)
            {
                Debug.WriteLine("gamePassString exists. Trying to retrieve...");
                try
                {
                    pass = EntryPass.FromBase64EncodedJson(passString);
                    Debug.WriteLine("Got GamePass from QueryString. Storing it to Cookie...");
                    Response.Cookies.Set(new HttpCookie(PassCookieName, pass.ToBase64EncodedJson()));
                }
                catch
                {
                    pass = null;
                    Debug.WriteLine("Failed to get GamePass from QueryString.");
                }
            }

            if (pass == null)
            {
                // Tries to retrieve Pass from Cookie.
                var passCookie = Request.Cookies[PassCookieName];
                if (passCookie != null)
                {
                    passString = passCookie.Value;
                    try
                    {
                        pass = EntryPass.FromBase64EncodedJson(passString);
                    }
                    catch
                    {
                        pass = null;
                    }
                }
            }*/

            // Tries to retrieve Pass from Cookie.
            var passCookie = Request.Cookies[PassCookieName];
            if (passCookie != null)
            {
                passString = passCookie.Value;
                try
                {
                    pass = EntryPass.FromBase64EncodedJson(passString);
                }
                catch
                {
                    pass = null;
                }
            }

            if (pass == null)
                // Not Authenticated
                return;

            var publicKey = ConfigurationManager.AppSettings["PublicKeyXmlString"];
            if (pass.IsValid(publicKey, Request.Url.Authority))
            {
                // Pass is valid. Sets it to Controller.
                ValidPass = pass;
                ValidPassString = passString;
                ViewBag.UserId = pass.data.userId;
            }
        }

        protected void RedirectIfNoPass()
        {
            if (ValidPass != null)
                return;
            var uri = new Uri(ConfigurationManager.AppSettings["PortalServerAuthUrl"]);
            Response.Redirect(uri.AbsoluteUri + "?redirectUrl=" + Url.Action("ExternalLogin", "User", null, Request.Url.Scheme) + "&finalDestination=" + Request.Url.AbsoluteUri, true);
        }

        protected void ClearPass()
        {
            Response.Cookies.Set(new HttpCookie(PassCookieName){ Expires = DateTime.Now.AddDays(-1) });
            ValidPass = null;
            ValidPassString = null;
        }
    }
}