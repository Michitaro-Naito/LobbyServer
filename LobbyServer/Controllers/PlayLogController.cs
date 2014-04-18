﻿using ApiScheme.Scheme;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Controllers
{
    public class PlayLogController : BaseController
    {
        /// <summary>
        /// Shows PlayLogs.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="partial"></param>
        /// <returns></returns>
        [OutputCache(Duration=10, VaryByParam="page,partial")]
        public ActionResult Index(int page = 0, bool partial = false)
        {
            var key = string.Format("{0}-{1}", page, partial);
            return SingletonAction(_renderingIndex, key, () =>
            {
                var o = ApiScheme.Client.Api.Get<GetPlayLogsOut>(new GetPlayLogsIn() { page = page });
                if (partial)
                    return View("IndexPartial", o.playLogs);
                return View(o.playLogs);
            });
        }
        static ConcurrentDictionary<string, int> _renderingIndex = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Shows details of PlayLog.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OutputCache(Duration=60, VaryByParam="id")]
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

        /// <summary>
        /// Lets User download a PlayLog.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OutputCache(Duration=60, VaryByParam="id")]
        public ActionResult Download(int id = 0)
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

            return File(Encoding.UTF8.GetBytes(str), "application/octet-stream", o.playLog.fileName);
        }
	}
}