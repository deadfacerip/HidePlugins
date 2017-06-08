using System;
using System.Net;
using Rocket.Core;
using Rocket.Core.Plugins;
using Steamworks;
using SDG.Unturned;

namespace HidePlugins
{
    public class HidePlugins : RocketPlugin<Config>
    {
        #region Vars
        public string pluginName = "HidePlugins";
        public string pluginVersion = "1.0";
        public string pluginSite = "Plugins.4Unturned.tk";
        public string unturnedVersion = "3.18.14.0 +";
        public string rocketVersion = "4.9.3.0";
        
        public static HidePlugins Instance;

        #region Write
        public static void Write(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        #endregion

        #region WriteMenu
        public static void WriteMenu(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        #endregion

        #region Write/Log Error
        public static void WriteLogError(string message)
        {
            Rocket.Core.Logging.Logger.LogError(message);
        }
        #endregion
        #endregion

        #region Load
        protected override void Load()
        {
            Instance = this;

            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;

            WriteMenu(pluginName + ", Version: " + pluginVersion);
            WriteMenu("Made for Unturned: " + unturnedVersion);
            WriteMenu("Made for RocketMod: " + rocketVersion);
            WriteMenu("Visit " + pluginSite + " for more!" + "\n");

            #region Check4Updates
            WriteMenu("~~~~~ Checking for Updates ~~~~~" + "\n");

            try
            {
                Write(new WebClient().DownloadString("http://plugins.4unturned.tk/plugins/" + pluginName + "/update_" + pluginVersion));
                Write(" ");
            }
            catch (WebException)
            {
                WriteLogError("Failed to Establish a Connection with Plugins.4Unturned.tk!\nMore Info: goo.gl/yAIaWz\n");
            }
            #endregion
        }
        #endregion

        #region OnPluginsLoaded
        public void OnPluginsLoaded()
        {
            #region Hide Plugins / Custom Entries
            if (Instance.Configuration.Instance.HidePlugins)
            {
                SteamGameServer.SetKeyValue("rocketplugins", "");
                
                Write("Hide Plugins: Enabled", ConsoleColor.Green);

                if (Instance.Configuration.Instance.Enable_CustomEntry)
                {
                    SteamGameServer.SetKeyValue("rocketplugins", Instance.Configuration.Instance.CustomEntry);
                    Write("Custom Entry: " + Instance.Configuration.Instance.CustomEntry, ConsoleColor.Green);
                }
                else
                {
                    Write("Custom Entry: Disabled", ConsoleColor.Red);
                }
            }
            else
            {
                Write("Hide Plugins: Disabled", ConsoleColor.Red);
            }
            #endregion

            #region Hide Workshop
            if (Instance.Configuration.Instance.HideWorkshop)
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "");
                Write("Hide Workshop: Enabled", ConsoleColor.Green);
            }
            else
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "99");
                Write("Hide Workshop: Disabled", ConsoleColor.Red);
            }
            #endregion

            #region Hide Config
            if (Instance.Configuration.Instance.HideConfig)
            {
                SteamGameServer.SetKeyValue("Browser_Config_Count", "");
                Write("Hide Config: Enabled", ConsoleColor.Green);
            }
            else
            {
                SteamGameServer.SetKeyValue("Browser_Config_Count", "99");
                Write("Hide Config: Disabled", ConsoleColor.Red);
            }
            #endregion

            #region MaxPlayerCount
            if (Instance.Configuration.Instance.Enable_MaxPlayerCount)
            {
                SteamGameServer.SetMaxPlayerCount(Instance.Configuration.Instance.MaxPlayerCount);
                Write("MaxPlayerCount: " + Instance.Configuration.Instance.MaxPlayerCount, ConsoleColor.Green);
            }
            else
            {
                SteamGameServer.SetMaxPlayerCount(Provider.maxPlayers);
                Write("MaxPlayerCount: Disabled", ConsoleColor.Red);
            }
            #endregion
        }
        #endregion

        #region Unload
        protected override void Unload()
        {
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;
        }
        #endregion
    }
}