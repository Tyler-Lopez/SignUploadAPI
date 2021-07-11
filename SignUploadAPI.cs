using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("SignUploadAPI", "Bunsen", "1.0.0")]
    [Description("Upload the contents of a sign the player is looking at to Imgur. Optional DiscordCore and ServerRewards integration.")]
    public class SignUploadAPI : RustPlugin
    {
        #region Plug-in References
        [PluginReference] private Plugin ServerRewards;
        [PluginReference] private Plugin ImgurApi;
        [PluginReference] private Plugin DiscordCore;
        #endregion
        #region Global Variables and Typography
        private HashSet<ulong> signUploadCooldown = new HashSet<ulong>();
        ulong imageIcon = 76561198317970917;
        private static string h1 = "<size=15><color=#F0A92C>";
        private static string h2 = "<size=12><color=green>";
        private static string sub = "<size=11><color=#B3B3B3>";
        private static string p = "<size=12><color=#E8E8E8>";
        private static string close = "</color></size>";
        private static string colorClose = "</color>";
        private static string highlight = "<color=yellow>";
        #endregion
        #region Oxide Hooks
        // Initialize permission.
        void Init()
        {
            permission.RegisterPermission("signuploader.use", this); // Required to use SignUploadAPI
            permission.RegisterPermission("signuploader.free", this); // If given and Use Server Rewards is enabled, deducts no RP upon use of SignUploadAPI.
        }
        void OnServerInitialized()
        {
            if (ImgurApi == null) Puts("You are missing ImgurApi, get it here: https://umod.org/plugins/imgur-api");
            if (config.useServerRewards && ServerRewards == null) Puts("You are missing Server Rewards, get it here: https://umod.org/plugins/server-rewards");
            if (config.useDiscordCore && DiscordCore == null) Puts("You are missing Server Rewards, get it here: https://umod.org/plugins/discord-core");
            cmd.AddChatCommand(config.command, this, "uploadSignFromRaycast");
        }
        #endregion
        #region Handle Sign-Uploading
        void uploadSignFromRaycast(BasePlayer player, string command, string[] args)
        {
            // The following are various causes of error the player may encounter.
            if(!permission.UserHasPermission(player.UserIDString, "signuploader.use"))
            {
                SendErrorPlayer(player, $"{p}You don't have the appropriate permission to use this.{close}");
                return;
            }
            if (config.cooldown > 0 && signUploadCooldown.Contains(player.userID))
            {
                SendErrorPlayer(player, $"{p}You're trying to do that again too soon.{close}");
                return;
            }
            if ((config.useServerRewards && getPoints(player) < config.cost) && !permission.UserHasPermission(player.UserIDString, "signuploader.free"))
            {
                SendErrorPlayer(player, $"{p}You don't have enough {highlight}RP{colorClose} to upload this sign to Imgur!{close}");
                return;
            }
            // Using Raycast, find the entity in front of the player.
            BaseEntity sign = entityRaycast(player);
            // If there was no entity detected handle the error.
            if (sign == null)
            {
                SendErrorPlayer(player, $"{p}You weren't looking at any entity.{close}");
                return;
            }
            // If the player specifys a first argument, make it the title of the uploaded picture. Else, set to Untitled.
            string title = args.Length < 1 ? "Untitled" : args[0];
            
            if (sign is PhotoFrame)
            {
                var data = FileStorage.server.Get(((PhotoFrame)sign)._overlayTextureCrc, FileStorage.Type.png, sign.net.ID);
                if (data != null)
                {
                    UploadImage(data, player, title);
                    return;
                }
            }
            else if (sign is Signage)
            {
                var data = FileStorage.server.Get(((Signage)sign).textureIDs.First(), FileStorage.Type.png, sign.net.ID);
                if (data != null)
                {
                    UploadImage(data, player, title);
                    return;
                }
            }
            else
            {
                SendErrorPlayer(player, $"{p}You weren't looking at a sign or photoframe.{close}");
                return;
            }
        }
        private void UploadImage(byte[] image, BasePlayer player, string info)
        {
            Action<Hash<string, object>> action = (hashSet) =>
            {
                bool success = (bool)hashSet["Success"];
                if (!success)
                {
                    SendErrorPlayer(player, $"{p}An error occured uploading the image \n\n{JsonConvert.SerializeObject(hashSet)}{close}");
                    return;
                }
                Hash<string, object> data = hashSet["Data"] as Hash<string, object>;
                if (config.useDiscordCore)
                {
                    DiscordCore?.Call("SendMessageToUser", player.userID.ToString(), $"{data?["Link"]}");
                    if (config.channelName != null) DiscordCore?.Call("SendMessageToChannel", config.channelName, $"\n\n**NEW PHOTO UPLOADED BY {player.displayName}**\n{data?["Link"]}");
                }
                if (config.useServerRewards && !permission.UserHasPermission(player.UserIDString, "signuploader.free"))
                {
                    takePoints(player, config.cost);
                }

                SendReplyWithIcon(player, $"{h1}Successful Upload.{close}\n" +
                    $"{p}The file may be accessed at {highlight}{data?["Link"]}{close}" + (config.useServerRewards ? $"{sub}\n{config.cost} RP deducted.{close}" : ""));

                Interface.CallHook("OnSignUploaded", info, player.userID);

            };

            SendReplyWithIcon(player, $"{h2}Uploading...{close}");

            if (config.cooldown > 0)
            {

                signUploadCooldown.Add(player.userID);
                timer.Once(config.cooldown, () =>
                {

                    signUploadCooldown.Remove(player.userID);
                });
            }
            ImgurApi?.Call("UploadImage", image, action, info);
        }
        #endregion
        #region Helper Functions
        private BaseEntity entityRaycast(BasePlayer player)
        {
            RaycastHit RayHit;
            var flag1 = Physics.Raycast(player.eyes.HeadRay(), out RayHit, 20f);
            var baseEntity = flag1 ? RayHit.GetEntity() : null;
            return baseEntity;
        }
        private void SendErrorPlayer(BasePlayer player, string message)
        {
            Effect.server.Run("assets/prefabs/locks/keypad/effects/lock.code.denied.prefab",
        player.transform.position);
            SendReplyWithIcon(player, "<size=15><color=red>Error.</color></size> " + message);
        }
        private void SendReplyWithIcon(BasePlayer player, string message)
        {
            Player.Message(player, message, imageIcon);
        }
        #endregion
        #region Handle Server Rewards
        private int getPoints(BasePlayer player)
        {
            if (config.useServerRewards)
            {
                object answer = ServerRewards?.Call<int>("CheckPoints", player.userID);
                if (answer != null) return (int)answer;
            }
            return 0;
        }
        private bool takePoints(BasePlayer player, int amount)
        {
            if (config.useServerRewards)
            {
                object answer = ServerRewards?.Call<int>("TakePoints", player.userID, amount);
                if (answer == null) return false;
                else return true;
            }
            return false;
        }
        #endregion
        #region Plug-in Configuration
        private static ConfigData config;

        private class ConfigData
        {
            [JsonProperty(PropertyName = "Chat Command")]
            public string command;

            [JsonProperty(PropertyName = "Use Discord Core")]
            public bool useDiscordCore;

            [JsonProperty(PropertyName = "Discord Channel to Upload to")]
            public string channelName;

            [JsonProperty(PropertyName = "Use Server Rewards")]
            public bool useServerRewards;

            [JsonProperty(PropertyName = "Cost to Upload")]
            public int cost;

            [JsonProperty(PropertyName = "Cooldown Between Uploads (seconds)")]
            public float cooldown;



        }

        private ConfigData getDefaultConfig()
        {
            ConfigData output = new ConfigData
            {
                command = "uploadsign",
                useDiscordCore = false,
                channelName = "",
                useServerRewards = false,
                cost = 20,
                cooldown = 30
            };
            return output;
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

        #endregion
    }
}