namespace Oxide.Plugins
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
}