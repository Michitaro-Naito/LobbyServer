using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class UserController : PassAuthenticatedController
    {
        public ActionResult ExternalLogin(string entryPassString = null, string finalDestination = null)
        {
            TryLogin(entryPassString);

            Debug.WriteLine("EntryPassString: " + entryPassString);
            Debug.WriteLine("FinalDestination: " + finalDestination);

            var uri = new Uri(finalDestination);
            if (uri.Authority != Request.Url.Authority)
                throw new Exception(string.Format("Authority not match. finalDestination: {0} RequestUrl: {1}", finalDestination, Request.Url.AbsoluteUri));
            return Redirect(uri.LocalPath);
        }

        public ActionResult Logout()
        {
            ClearPass();
            //return Redirect("http://accounts.google.com/Logout");
            return View();
            //return RedirectToAction("Index", "Home");
        }
	}
}