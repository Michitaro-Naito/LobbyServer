using ApiScheme.Scheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class PlayLogController : BaseController
    {
        //
        // GET: /PlayLog/
        [OutputCache(Duration=60, VaryByParam="page")]
        public ActionResult Index(int page = 0)
        {
            var o = ApiScheme.Client.Api.Get<GetPlayLogsOut>(new GetPlayLogsIn() { page = page });
            return View(o.playLogs);
        }
	}
}