﻿using ApiScheme.Scheme;
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
        public ActionResult TestLogin(string id)
        {
            var gamePass = new GamePass() { data = new GamePass.GamePassData() { userId = id } };
            Response.Cookies.Set(new HttpCookie(PassCookieName, gamePass.ToCipher(ConfigurationManager.AppSettings["AesKey"], ConfigurationManager.AppSettings["AesIv"])));
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
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