namespace Oxide.Plugins
{
    using UnityEngine;

    partial class RustPluginTemplate : RustPlugin
    {
        partial void initCommands()
        {
            cmd.AddChatCommand("pos", this, nameof(posCommand));
            cmd.AddChatCommand("list", this, nameof(listCommand));
        }

        private void posCommand(BasePlayer player, string command, string[] args)
        {
            if (!config.allowPosCom) return;
            if (!hasPermission(player, permissions.use))
            {
                Localization.SendLocalizedMessage(player, Localization.UniversalMessages.noPermission);
                return;
            }
            //get player Position
            Vector3 pos = player.transform.position;
            //store Position in Data
            StoredData.addPosition(pos);
        }

        private void listCommand(BasePlayer player, string command, string[] args)
        {
            UIPositionList(player);
        }
    }
}﻿namespace Oxide.Plugins
{
    using Newtonsoft.Json;

    partial class RustPluginTemplate : RustPlugin
    {
        private static ConfigData config;

        private class ConfigData
        {
            [JsonProperty(PropertyName = "Allow Pos Command")]
            public bool allowPosCom;
        }

        private ConfigData getDefaultConfig()
        {
            return new ConfigData
            {
                allowPosCom = true
            };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                config = Config.ReadObject<ConfigData>();
            }
            catch
            {
                Puts("Config data is corrupted, replacing with default");
                config = new ConfigData();
            }

            SaveConfig();
        }

        protected override void SaveConfig() => Config.WriteObject(config);

        protected override void LoadDefaultConfig() => config = getDefaultConfig();
    }
}﻿namespace Oxide.Plugins
{
    using Newtonsoft.Json;
    using Oxide.Core;
    using Oxide.Core.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    /*
     Make sure that you're not saving complex classes like BasePlayer or Item. Try to stick with primitive types.
     If you're saving your own classes, make sure they have a default constructor and that all properties you're saving are public.
     Take control of which/how properties get serialized by using the Newtonsoft.Json Attributes https://www.newtonsoft.com/json/help/html/SerializationAttributes.htm
    */

    partial class RustPluginTemplate : RustPlugin
    {
        partial void initData()
        {
            StoredData.init();
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class StoredData
        {
            private static DynamicConfigFile storedDataFile;
            private static StoredData instance;
            private static bool initialized = false;

            [JsonProperty(PropertyName = "Position list")]
            public List<Vector3> positionList = new List<Vector3>();

            public StoredData()
            {
            }

            public static void addPosition(Vector3 position)
            {
                if (!initialized) init();
                instance.positionList.Add(position);
                save();
            }

            public static List<Vector3> getList()
            {
                if (!initialized) init();
                return instance.positionList;
            }

            public static void init()
            {
                if (initialized) return;
                storedDataFile = Interface.Oxide.DataFileSystem.GetFile("RustPluginTemplate/posData");
                load();
                initialized = true;
            }

            public static void save()
            {
                if (!initialized) init();
                try
                {
                    storedDataFile.WriteObject(instance);
                }
                catch (Exception E)
                {
                    StringBuilder sb = new StringBuilder($"saving {typeof(StoredData).Name} failed. Are you trying to save complex classes like BasePlayer or Item? that won't work!\n");
                    sb.Append(E.Message);
                    PluginInstance.Puts(sb.ToString());
                }
            }

            public static void load()
            {
                try
                {
                    instance = storedDataFile.ReadObject<StoredData>();
                }
                catch (Exception E)
                {
                    StringBuilder sb = new StringBuilder($"loading {typeof(StoredData).Name} failed. Make sure that all classes you're saving have a default constructor!\n");
                    sb.Append(E.Message);
                    PluginInstance.Puts(sb.ToString());
                }
            }
        }
    }
}﻿namespace Oxide.Plugins
{
    using UnityEngine;
    using static Oxide.Plugins.GUICreator;

    partial class RustPluginTemplate : RustPlugin
    {
        partial void initGUI()
        {
            guiCreator = (GUICreator)Manager.GetPlugin("GUICreator");
        }

        private GUICreator guiCreator;

        private void UIPositionList(BasePlayer player)
        {
            GuiContainer container = new GuiContainer(this, "positionList");
            container.addButton("close", new Rectangle(800, 80, 320, 50, 1920, 1080, true), GuiContainer.Layer.hud, new GuiColor("red"), text: new GuiText("close"));
            Rectangle backgroundRect = new Rectangle(800, 130, 320, 790, 1920, 1080, true);
            container.addPlainPanel("background", backgroundRect, GuiContainer.Layer.hud, new GuiColor(0,0,0,0.3f), blur: GuiContainer.Blur.medium);
            int i = 0;
            foreach(Vector3 pos in StoredData.getList())
            {
                if (i == 12) break;
                Rectangle entryRect = new Rectangle(10, 10 + i * 60, 300, 50, 320, 790, true);
                GuiText text = new GuiText($"X:{(int)pos.x}, Y:{(int)pos.y}, Z:{(int)pos.z}", color: new GuiColor("white"));
                container.addPanel($"entry_{i}", entryRect, "background", new GuiColor(0, 0, 0, 0.5f), text: text);
                i++;
            }
            container.display(player);

        }
    }
}﻿namespace Oxide.Plugins
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
}﻿namespace Oxide.Plugins
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
}﻿namespace Oxide.Plugins
{
    partial class RustPluginTemplate : RustPlugin
    {
        partial void initData();

        partial void initCommands();

        partial void initLang();

        partial void initPermissions();

        partial void initGUI();

        private void Loaded()
        {
            initData();
            initCommands();
            initLang();
            initPermissions();
            initGUI();
        }
    }
}﻿namespace Oxide.Plugins
{
    using System;

    partial class RustPluginTemplate : RustPlugin
    {
        //permissions will be (lowercase class name).(perm)
        partial void initPermissions()
        {
            foreach (string perm in Enum.GetNames(typeof(permissions)))
            {
                permission.RegisterPermission($"{PluginInstance.Name}.{perm}", this);
            }
        }

        private enum permissions
        {
            use,
            admin
        }

        private bool hasPermission(BasePlayer player, permissions perm)
        {
            return player.IPlayer.HasPermission($"{PluginInstance.Name}.{Enum.GetName(typeof(permissions), perm)}");
        }

        private void grantPermission(BasePlayer player, permissions perm)
        {
            player.IPlayer.GrantPermission($"{PluginInstance.Name}.{Enum.GetName(typeof(permissions), perm)}");
        }

        private void revokePermission(BasePlayer player, permissions perm)
        {
            player.IPlayer.RevokePermission($"{PluginInstance.Name}.{Enum.GetName(typeof(permissions), perm)}");
        }
    }
}