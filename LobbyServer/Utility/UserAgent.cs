using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LobbyServer.Utility
{
    public enum UserAgent
    {
        PC,
        iPhone,             // "iPhone"
        iPad,               // "iPad"
        AndroidMobile,      // "Android" and "Mobile"
        AndroidTablet,      // "Android" only
        WindowsPhone        // "Windows Phone"
    }

    public static class UserAgentExtension
    {
        public static bool IsPC(this UserAgent ua)
        {
            return ua == UserAgent.PC;
        }

        public static bool IsSmartphone(this UserAgent ua)
        {
            return ua != UserAgent.PC;
        }

        public static bool IsMobile(this UserAgent ua)
        {
            return new UserAgent[] { UserAgent.iPhone, UserAgent.AndroidMobile, UserAgent.WindowsPhone }.Contains(ua);
        }

        public static bool IsTablet(this UserAgent ua)
        {
            return new UserAgent[] { UserAgent.iPad, UserAgent.AndroidTablet }.Contains(ua);
        }

        public static bool IsIOS(this UserAgent ua)
        {
            return new UserAgent[] { UserAgent.iPhone, UserAgent.iPad }.Contains(ua);
        }

        public static bool IsAndroid(this UserAgent ua)
        {
            return new UserAgent[] { UserAgent.AndroidMobile, UserAgent.AndroidTablet }.Contains(ua);
        }
    }

    public static class UserAgentHelper
    {
        public static UserAgent GetUserAgent(string userAgentString)
        {
            if (string.IsNullOrEmpty(userAgentString)) return UserAgent.PC;

            if (userAgentString.Contains("iPhone"))
            {
                return UserAgent.iPhone;
            }
            else if (userAgentString.Contains("iPad"))
            {
                return UserAgent.iPad;
            }
            else if (userAgentString.Contains("Android"))
            {
                if (userAgentString.Contains("Mobile")) return UserAgent.AndroidMobile;
                else return UserAgent.AndroidTablet;
            }
            else if (userAgentString.Contains("Windows Phone"))
            {
                return UserAgent.WindowsPhone;
            }

            return UserAgent.PC;
        }
    }
}