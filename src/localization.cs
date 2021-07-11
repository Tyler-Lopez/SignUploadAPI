namespace Oxide.Plugins
{
    using System.Collections.Generic;

    partial class RustPluginTemplate : RustPlugin
    {
        partial void initLang()
        {
            lang.RegisterMessages(Localization.messages, this);
        }

        public class Localization
        {
            public enum UniversalMessages { noPermission, invalidArguments, noPlayersFound, multiplePlayersFound };

            public static Dictionary<string, string> messages = new Dictionary<string, string>()
            {
                {UniversalMessages.noPermission.ToString(), "You don't have permission to use this command!"},
                {UniversalMessages.invalidArguments.ToString(), "Too few/many arguments given. Remember to use quotes(\"\") for names containing whitespaces!" },
                {UniversalMessages.noPlayersFound.ToString(), "No players found!" },
                {UniversalMessages.multiplePlayersFound.ToString(), "Multiple players found:" }
            };

            public static void SendLocalizedMessage(BasePlayer player, UniversalMessages message, string[] args = null)
            {
                SendLocalizedMessage(player, message.ToString(), args);
            }

            public static void SendLocalizedMessage(BasePlayer player, string msgKey, string[] args = null)
            {
                string message;
                string langMsg = PluginInstance.lang.GetMessage(msgKey, PluginInstance, player.UserIDString);
                if (args != null) message = string.Format(langMsg, args);
                else message = langMsg;
                PluginInstance.PrintToChat(player, message);
            }
        }
    }
}