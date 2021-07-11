namespace Oxide.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    partial class RustPluginTemplate
    {
        public class Helpers
        {
            public static BasePlayer findPlayer(string name, BasePlayer player)
            {
                if (string.IsNullOrEmpty(name)) return null;
                ulong id;
                ulong.TryParse(name, out id);
                List<BasePlayer> results = BasePlayer.allPlayerList.Where((p) => p.displayName.Contains(name, System.Globalization.CompareOptions.IgnoreCase) || p.userID == id).ToList();
                if (results.Count == 0)
                {
                    if (player != null) Localization.SendLocalizedMessage(player, Localization.UniversalMessages.noPlayersFound);
                }
                else if (results.Count == 1)
                {
                    return results[0];
                }
                else if (player != null)
                {
                    Localization.SendLocalizedMessage(player, Localization.UniversalMessages.multiplePlayersFound);
                    int i = 1;
                    foreach (BasePlayer p in results)
                    {
                        player.ChatMessage($"{i}. {p.displayName}[{p.userID}]");
                        i++;
                    }
                }
                return null;
            }
        }
    }
}