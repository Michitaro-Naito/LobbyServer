using ApiScheme.Client;
using ApiScheme.Scheme;
using AuthUtility;
using LobbyServer.Controllers.Helper;
using LobbyServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class HomeController : PassAuthenticatedController
    {
        public ActionResult TestLogin(string id)
        {
            var gamePass = new GamePass() { data = new GamePass.GamePassData() { userId = id } };
            Response.Cookies.Set(new HttpCookie(PassCookieName, gamePass.ToCipher(ConfigurationManager.AppSettings["AesKey"], ConfigurationManager.AppSettings["AesIv"])));
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Shows a Home.
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration=10)]
        public ActionResult Index()
        {
            return SingletonAction(() => {
                // Got lock
                GetStatisticsOut statistics = null;
                GetGameServersOut gameServers = null;
                GetPlayLogsOut playLogs = null;
                List<GameServerStatus> servers = null;

                Parallel.Invoke(
                    () => statistics = Api.Get<GetStatisticsOut>(new GetStatisticsIn()),
                    () => gameServers = Api.Get<GetGameServersOut>(new GetGameServersIn()),
                    () => playLogs = Api.Get<GetPlayLogsOut>(new GetPlayLogsIn() { page = 0 }));
                servers = GameServerHelper.OrderByRecommended(gameServers.servers);

                return View(new HomeIndexVM() { Statistics = statistics, Servers = servers, PlayLogs = playLogs.playLogs });
            });
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