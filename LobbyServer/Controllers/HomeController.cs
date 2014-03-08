using AuthUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class HomeController : PassAuthenticatedController
    {
        public ActionResult Index()
        {
            //var url = ConfigurationManager.AppSettings["PortalServerAuthUrl"];
            //return Redirect(url + "?redirectUrl=" + Url.Action("About", "Home", null, Request.Url.Scheme));
            return View();
        }

        public ActionResult About(string gamePassString = null)
        {
            ViewBag.Message = "Your application description page.";
            var publicKey = ConfigurationManager.AppSettings["PublicKeyXmlString"];
            var urlMaybeRequested = Url.Action("About", "Home", null, Request.Url.Scheme);
            Debug.WriteLine(gamePassString);
            var pass = GamePass.FromBase64EncodedJson(gamePassString);
            Debug.WriteLine(pass);
            Debug.WriteLine(pass.IsValid(publicKey, urlMaybeRequested));
            if (pass.IsValid(publicKey, urlMaybeRequested))
            {
                Debug.WriteLine("GamePass is valid. Storing to Cookie...");
                StoreGamePassToCookie(pass);
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            Debug.WriteLine(ValidPass);
            return View();
        }
    }
}