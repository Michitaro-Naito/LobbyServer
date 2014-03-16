using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Configuration;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using ApiScheme.Scheme;
using ApiScheme.Client;

namespace LobbyServer.Tests
{
    [TestClass]
    public class ApiCallTest
    {
        [TestMethod]
        public void GetFoos()
        {
            var foo = Api.Get<TestGet3FoosResponse>(new TestGet3FoosRequest());
            Assert.AreEqual(3, foo.foos.Count);
        }

        [TestMethod]
        public void GetError()
        {
            try
            {
                var error = Api.Get<TestGetErrorResponse>(new TestGetErrorRequest());
            }
            catch (WebException e)
            {
                return;
            }
            Assert.Fail("Exception not thrown.");
        }

        [TestMethod]
        public void Plus()
        {
            var str = "!\"#$%&'()=~|12-^\\,./;:+*[]{}`@_?><";
            var res = Api.Get<TestPlusResponse>(new TestPlusRequest() { a = 100, b = 125, echo = str });
            Assert.AreEqual(225, res.c);
            Assert.AreEqual(str, res.echo);
        }
    }
}
