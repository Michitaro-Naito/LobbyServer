using ApiScheme.Scheme;
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
        /*//
        // GET: /Game/
        public ActionResult Index()
        {
            RequirePass();
            return View();
        }*/

        /// <summary>
        /// Shows a list of GameServers to User.
        /// GoodServers above. BadServers below.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(bool partial = false)
        {
            var o = ApiScheme.Client.Api.Get<GetGameServersOut>(new GetGameServersIn());

            // Finds a recommended GameServer.
            var goodServers = o.servers
                .OrderByDescending(s => s.players)
                .Where(s =>
                    s.framesPerInterval / s.reportIntervalSeconds > 60      // Fast. (Has > 60 frames per second in average.)
                    && s.maxElapsedSeconds < 10);                           // No great lag. (Not stopped over 10 seconds.)

            var goodAvailableServers = goodServers
                .Where(s => s.players < 0.66 * s.maxPlayers);

            var goodFullServers = goodServers
                .Where(s => s.players >= 0.66 * s.maxPlayers);

            var badServers = o.servers.Where(s => !goodServers.Contains(s));

            var servers = new List<GameServerStatus>();
            servers.AddRange(goodAvailableServers);     // Good and Can Join.
            servers.AddRange(goodFullServers);          // Good but Mostly Full.
            servers.AddRange(badServers);               // Bad. Laggy or Buggy... :(

            if (partial)
                return View("IndexPartial", servers);
            return View(servers);
        }

        /// <summary>
        /// Shows a statistics of this game.
        /// </summary>
        /// <returns></returns>
        public ActionResult Statistics()
        {
            var o = ApiScheme.Client.Api.Get<GetStatisticsOut>(new GetStatisticsIn());
            return View("StatisticsPartial", o);
        }

        /// <summary>
        /// Lets User join a GameServer.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public ActionResult Play(string host, int port)
        {
            RequirePass();

            var uri = new Uri(string.Format("http://{0}:{1}/signalr", host, port));
            Debug.WriteLine(uri.AbsoluteUri);
            return View(new GamePlayVM() { GameServerSignalREndpoint = uri });
        }

        /*/// <summary>
        /// Redirects User to recommended GameServer.
        /// </summary>
        /// <returns></returns>
        public ActionResult JoinAuto()
        {
            var o = ApiScheme.Client.Api.Get<GetGameServersOut>(new GetGameServersIn());

            // Finds a recommended GameServer.
            GameServerStatus server;
            server = o.servers
                .OrderByDescending(s => s.players)  // More players are better.
                .FirstOrDefault(s =>
                    s.players < 0.66 * s.maxPlayers  // Not full. (Players are < 66% of the capacity.)
                    && s.framesPerInterval / s.reportIntervalSeconds > 100    // Fast. (Has > 60 frames per second in average.)
                    && s.maxElapsedSeconds < 10);   // No great lag. (Not stopped over 10 seconds.)

            if (server == null)
                // Not found a nice one. Unavoidable to return damn one... :(
                server = o.servers
                    .OrderByDescending(s => s.players)
                    .FirstOrDefault(s => s.players < 0.8 * s.maxPlayers);

            if (server == null)
                // Not found.
                return HttpNotFound();

            // Redirects User.
            return RedirectToRoute(new { controller = "Game", action = "Play", host = server.host, port = server.port });
        }*/
	}
}