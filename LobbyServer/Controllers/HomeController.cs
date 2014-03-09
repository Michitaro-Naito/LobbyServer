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
            Debug.WriteLine(Request.Url.Authority);
            var uri = new Uri("http://amlitek.com:8080/foo/bar?pu=chan&abc=def");
            Debug.WriteLine(uri.Authority);
            //var url = ConfigurationManager.AppSettings["PortalServerAuthUrl"];
            //return Redirect(url + "?redirectUrl=" + Url.Action("About", "Home", null, Request.Url.Scheme));
            return View();
        }

        public ActionResult About(string gamePassString = null)
        {
            ViewBag.Message = "Your application description page.";
            /*var publicKey = ConfigurationManager.AppSettings["PublicKeyXmlString"];
            Debug.WriteLine(gamePassString);
            var pass = GamePass.FromBase64EncodedJson(gamePassString);
            Debug.WriteLine(pass);
            if (pass.IsValid(publicKey, Request.Url.Authority))
            {
                Debug.WriteLine("GamePass is valid. Storing to Cookie...");
                StoreGamePassToCookie(pass);
            }
            else
            {
                Debug.WriteLine("GamePass is not valid.");
            }*/
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