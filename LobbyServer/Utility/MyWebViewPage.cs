using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LobbyServer.Utility
{
    public class MyWebViewPage<T> : WebViewPage<T>
    {
        UserAgent _userAgent;
        public UserAgent UserAgent
        {
            get
            {
                return _userAgent;
            }
            private set
            {
                _userAgent = value;
            }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();
        }

        protected override void InitializePage()
        {
            UserAgent = UserAgentHelper.GetUserAgent(Request.UserAgent);
            base.InitializePage();
        }

        public override void Execute()
        {
            Debug.WriteLine("Execute");
        }
    }
}