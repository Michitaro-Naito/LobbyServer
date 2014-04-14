using ApiScheme.Scheme;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class PlayLogController : BaseController
    {
        //
        // GET: /PlayLog/
        //[OutputCache(Duration=60, VaryByParam="page")]
        public ActionResult Index(int page = 0)
        {
            var o = ApiScheme.Client.Api.Get<GetPlayLogsOut>(new GetPlayLogsIn() { page = page });
            return View(o.playLogs);
        }

        //[OutputCache(Duration=60, VaryByParam="id")]
        public ActionResult Details(int id = 0)
        {
            if (id == 0)
                return HttpNotFound();

            var o = ApiScheme.Client.Api.Get<GetPlayLogByIdOut>(new GetPlayLogByIdIn() { id = id });
            if (o.playLog == null)
                return HttpNotFound();

            var url = ConfigurationManager.AppSettings["PlayLogUrl"] + o.playLog.fileName;
            var req = WebRequest.Create(url);
            string str;
            try
            {
                Debug.WriteLine("Getting...");
                using (var res = req.GetResponse())
                {
                    using (var rs = res.GetResponseStream())
                    using (var reader = new StreamReader(rs, Encoding.UTF8))
                    {
                        str = reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return HttpNotFound();
            }

            //return View(o.playLog);
            return Content(str);
        }
	}
}