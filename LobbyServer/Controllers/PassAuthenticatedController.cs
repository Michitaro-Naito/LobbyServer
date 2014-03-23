using AuthUtility;
using LobbyServer.Models;
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
        protected const string PassCookieName = "GamePass";

        /// <summary>
        /// Pass of User. If null, User is not authenticated.
        /// </summary>
        protected GamePass ValidPass { get; private set; }
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
            EntryPass entryPass;
            try
            {
                entryPass = EntryPass.FromBase64EncodedJson(entryPassString);
            }
            catch
            {
                entryPass = null;
            }

            if (entryPass == null)
                return;

            if (entryPass.IsValid(ConfigurationManager.AppSettings["PublicKeyXmlString"], Request.Url.Authority))
            {
                // EntryPass is valid.
                // Generates a GamePass and stores it to Cookie...
                Debug.WriteLine("Valid. Storing...");
                var gamePass = GamePass.FromValidEntryPass(entryPass);
                Response.Cookies.Set(new HttpCookie(PassCookieName, gamePass.ToCipher(ConfigurationManager.AppSettings["AesKey"], ConfigurationManager.AppSettings["AesIv"])));
            }
        }

        void GetPass()
        {
            GamePass pass = null;
            string passString = null;

            // Tries to retrieve Pass from Cookie.
            var passCookie = Request.Cookies[PassCookieName];
            if (passCookie != null)
            {
                passString = passCookie.Value;
                try
                {
                    pass = GamePass.FromCipher(passString, ConfigurationManager.AppSettings["AesKey"], ConfigurationManager.AppSettings["AesIv"]);
                }
                catch
                {
                    pass = null;
                }
            }

            if (pass == null)
                // Not Authenticated
                return;

            // GamePass is valid. Sets it to Controller.
            ValidPass = pass;
            ValidPassString = passString;
            ViewBag.UserId = pass.data.userId;
            ViewBag.ValidPassString = ValidPassString;
        }

        protected void RequirePass()
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