using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class ErrorController : BaseController
    {
        [OutputCache(Duration=60)]
        public ActionResult NotFound()
        {
            return View();
        }
	}
}