using ApiScheme.Scheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LobbyServer.Controllers.Helper
{
    public static class GameServerHelper
    {
        public static List<GameServerStatus> OrderByRecommended(IEnumerable<GameServerStatus> unorderedServers)
        {
            // Finds a recommended GameServer.
            var goodServers = unorderedServers
                .OrderByDescending(s => s.players)
                .Where(s =>
                    s.framesPerInterval / s.reportIntervalSeconds > 60      // Fast. (Has > 60 frames per second in average.)
                    && s.maxElapsedSeconds < 10);                           // No great lag. (Not stopped over 10 seconds.)

            var goodAvailableServers = goodServers
                .Where(s => s.players < 0.66 * s.maxPlayers);

            var goodFullServers = goodServers
                .Where(s => s.players >= 0.66 * s.maxPlayers);

            var badServers = unorderedServers.Where(s => !goodServers.Contains(s));

            var servers = new List<GameServerStatus>();
            servers.AddRange(goodAvailableServers);     // Good and Can Join.
            servers.AddRange(goodFullServers);          // Good but Mostly Full.
            servers.AddRange(badServers);               // Bad. Laggy or Buggy... :(

            return servers;
        }
    }
}