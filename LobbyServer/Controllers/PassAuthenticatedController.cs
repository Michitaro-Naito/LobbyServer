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
        protected const string PassCookieName = "GamePass";

        /// <summary>
        /// Pass of User. If null, User is not authenticated.
        /// </summary>
        protected GamePass ValidPass { get; private set; }
        protected string ValidPassString { get; private set; }

        void GetPass()
        {
            GamePass pass = null;
            // Tries to retrieve Pass from Cookie.
            var passCookie = Request.Cookies[PassCookieName];
            string passString = null;
            if (passCookie != null)
            {
                passString = passCookie.Value;
                try
                {
                    pass = GamePass.FromBase64EncodedJson(passString);
                }
                catch
                {
                    pass = null;
                }
            }

            if (pass == null)
                // Not Authenticated
                return;

            /*if (!pass.IsValid)
            {
                // Invalid Pass. (Modified by Hacker?) Clears Cookie and return.
                //Debug.WriteLine("Invalid Pass. Discarding Cookie...");
                Response.Cookies.Remove(PassCookieName);
                return;
            }*/
            var publicKey = ConfigurationManager.AppSettings["PublicKeyXmlString"];
            if (pass.IsValid(publicKey, Url.Action("About", "Home", null, Request.Url.Scheme)))
            {
                // Pass is valid. Sets it to Controller.
                ValidPass = pass;
                ValidPassString = passString;
            }
        }

        protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)
        {
            GetPass();
            //Debug.WriteLine("Pass: " + ValidPass);
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

        protected void StoreGamePassToCookie(GamePass gamePass)
        {
            Response.Cookies.Set(new HttpCookie(PassCookieName, gamePass.ToBase64EncodedJson()));
            GetPass();
        }

        /// <summary>
        /// User gives Code (provided by Google+ OAuth2) to Server.
        /// Server gives Pass (signed User info, Cookie) to User.
        /// 
        /// ----- Pass -----
        /// Data:{
        ///     UserId: []  // Local User ID
        ///     Plus: {
        ///         // Google+ OAuth2, Authentication Object
        ///     }
        /// },
        /// Sign: []    // SHA512 Hash
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected void PlusLogin(string code)
        {
            // Manually perform the OAuth2 flow.
            string clientId = ConfigurationManager.AppSettings["GoogleApiClientId"];
            string clientSecret = ConfigurationManager.AppSettings["GoogleApiClientSecret"];
            var authObject = ManualCodeExchanger.ExchangeCode(clientId, clientSecret, code);

            // Generates Pass from AuthObject and stores it to Cookie.
            // (It describes User is who.)
            string hashSecret = ConfigurationManager.AppSettings["SecuritySalt"];
            var pass = Pass.GenerateFromAuthObject(hashSecret, authObject);
            var json = JsonConvert.SerializeObject(pass);
            Response.Cookies.Set(new HttpCookie(PassCookieName, AuthHelper.Encrypt(json)));
        }
    }
}