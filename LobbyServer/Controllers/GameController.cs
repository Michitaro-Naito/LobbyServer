using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class GameController : PassAuthenticatedController
    {
        //
        // GET: /Game/
        public ActionResult Index()
        {
            RequirePass();
            return View();
        }
	}
}