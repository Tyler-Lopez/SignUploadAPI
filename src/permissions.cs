namespace Oxide.Plugins
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