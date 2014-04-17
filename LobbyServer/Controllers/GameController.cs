using LobbyServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public ActionResult Play(string host, int port)
        {
            var uri = new Uri(string.Format("http://{0}:{1}/signalr", host, port));
            Debug.WriteLine(uri.AbsoluteUri);
            return View(new GamePlayVM() { GameServerSignalREndpoint = uri });
        }
	}
}