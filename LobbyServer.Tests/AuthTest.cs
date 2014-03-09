using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuthUtility;
using System.Configuration;

namespace LobbyServer.Tests
{
    [TestClass]
    public class AuthTest
    {
        [TestMethod]
        public void EntryPassDeserializeTest()
        {
            var gamePassString = "eyJkYXRhIjp7InVzZXJJZCI6IlQ1OG5UMmNtcXJwdjhod3Y1ZFZyZGc9PSIsInJlZGlyZWN0VXJsIjoiaHR0cDovL2xvY2FsaG9zdDo1MDczMS9Ib21lL0Fib3V0IiwiZXhwaXJlcyI6IlwvRGF0ZSgxMzk0MjkyMTc3Njc3KVwvIn0sInNpZ24iOiJOUHh1YVllMmEzU0k4MHZ4NHJLTkRFQmJuOGpkZThLUzd5Mk4vdDlEUHBSdWVSaWh6OEQ5N1F2eURzRkpDRHpxRnE1eXBUUlAwSFFEb240b2t2d2Q3cTRodWpLZDVMWTQ1STh4MWRYVDlSRjRGZXlid0Fma25QRkU2YlJFanBYdEtKRkNNenpQZytwb1cvY1U5SVJOU0hZM2k1ZjRZcG50aFV2RDRaTjdPVms9In0=";
            var publicKey = ConfigurationManager.AppSettings["PublicKeyXmlString"];
            Assert.IsNotNull(publicKey);
            var pass = EntryPass.FromBase64EncodedJson(gamePassString);
            Assert.IsNotNull(pass);

            // This will be false because gamePassString is expired.
            //var urlMaybeRequested = "http://localhost:50731/Home/About";
            //Assert.IsTrue(pass.IsValid(publicKey, urlMaybeRequested));
        }
    }
}
