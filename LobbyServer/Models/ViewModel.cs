using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LobbyServer.Models
{
    public abstract class ViewModel
    {
    }

    public class GamePlayVM : ViewModel
    {
        public Uri GameServerSignalREndpoint { get; set; }
    }
}