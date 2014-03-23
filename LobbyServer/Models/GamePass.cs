using AuthUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LobbyServer.Models
{
    /*public class GamePass
    {
        public class GamePassData
        {
            // Auth (Server trusts it.)
            public string userId;

            // Cache (Server doesn't trust it.)
            //public string displayName;
        }

        public GamePassData data = new GamePassData();

        public static GamePass FromValidEntryPass(EntryPass entryPass)
        {
            var pass = new GamePass();
            pass.data.userId = entryPass.data.userId;
            return pass;
        }

        public string ToCipher(string key, string iv)
        {
            var json = JsonConvert.SerializeObject(this);
            return AuthHelper.Encrypt(json, key, iv);
        }

        public static GamePass FromCipher(string cipher, string key, string iv)
        {
            var json = AuthHelper.Decrypt(cipher, key, iv);
            return JsonConvert.DeserializeObject<GamePass>(json);
        }
    }*/
}