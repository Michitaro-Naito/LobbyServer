using ApiScheme.Scheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LobbyServer.Models
{
    public abstract class ViewModel
    {
    }

    public class HomeIndexVM : ViewModel
    {
        public GetStatisticsOut Statistics { get; set; }
        public List<PlayLogInfo> PlayLogs { get; set; }
        public List<GameServerStatus> Servers { get; set; }
    }

    public class GamePlayVM : ViewModel
    {
        public Uri GameServerSignalREndpoint { get; set; }
    }
}