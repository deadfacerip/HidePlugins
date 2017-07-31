using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using Logger = Rocket.Core.Logging.Logger;

namespace HidePlugins
{
    public class HidePlugins : RocketPlugin<Config>
    {
        #region Vars
        public string pluginName = Assembly.GetExecutingAssembly().GetName().Name;
        public string pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor + "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
        public string pluginSite = "Plugins.4Unturned.tk";
        public string unturnedVersion = "3.20.3.0 +";
        public string rocketVersion = "4.9.3.0";
        
        public static HidePlugins Instance;

        #region WriteToConsole
        public static void Write(string message)
        {
            Console.WriteLine(message);
        }
        public static void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void WriteDebug(string message)
        {
            if (Instance.Configuration.Instance.Mode == "Debug")
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[DEBUG] " + message);
                Console.ResetColor();
            }
        }
        #endregion

        #endregion

        #region Load
        protected override void Load()
        {
            Instance = this;
            
            Level.onLevelLoaded += OnLevelLoaded;
            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;

            Write(pluginName + ", Version: " + pluginVersion, ConsoleColor.Yellow);
            Write("Made for Unturned: " + unturnedVersion, ConsoleColor.Yellow);
            Write("Made for RocketMod: " + rocketVersion + "\n", ConsoleColor.Yellow);

            CheckForUpdates();

            #region Config
            if (Configuration.Instance.HidePlugins)
            {
                Write("Hide Plugins: Enabled", ConsoleColor.Green);
            }
            else
            {
                Write("Hide Plugins: Disabled", ConsoleColor.Red);
            }

            if (Configuration.Instance.Enable_CustomEntries == true && Configuration.Instance.CustomEntries.Count > 0)
            {
                Write("Custom Entries: Enabled", ConsoleColor.Green);

                foreach (string plugin in Configuration.Instance.CustomEntries)
                {
                    if (!Regex.IsMatch(plugin, @"^[A-Za-z0-9 _-]*[A-Za-z0-9][A-Za-z0-9 _]*$"))
                    {
                        Write(@"    - " + "[?]", ConsoleColor.Green);
                    }
                    else
                    {
                        Write(@"    - " + plugin, ConsoleColor.Green);
                    }
                }
            }
            else
            {
                Write("Custom Entries: Disabled", ConsoleColor.Red);
            }

            if (Configuration.Instance.HideWorkshop)
            {
                Write("Hide Workshop: Enabled", ConsoleColor.Green);
            }
            else
            {
                Write("Hide Workshop: Disabled", ConsoleColor.Red);
            }

            if (Configuration.Instance.HideConfig)
            {
                Write("Hide Config: Enabled", ConsoleColor.Green);
            }
            else
            {
                Write("Hide Config: Disabled", ConsoleColor.Red);
            }

            if (Configuration.Instance.Enable_LargeServer)
            {
                Write("Large Server: Enabled", ConsoleColor.Green);
            }
            else
            {
                Write("Large Server: Disabled", ConsoleColor.Red);
            }
            #endregion
        }
        #endregion

        #region CheckForUpdates
        public void CheckForUpdates()
        {
            string updateDir = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Plugins" + Path.DirectorySeparatorChar + "HidePlugins" + Path.DirectorySeparatorChar + "Updates" + Path.DirectorySeparatorChar;

            try
            {
                string updateSite = new WebClient().DownloadString("http://plugins.4unturned.tk/plugins/HidePlugins/update");

                if (updateSite.Length > 7) return;

                if (updateSite == pluginVersion) return;

                if (!System.IO.Directory.Exists(updateDir))
                {
                    System.IO.Directory.CreateDirectory(updateDir);
                }

                if (Configuration.Instance.DisableAutoUpdates == "true")
                {
                    Write("Version " + updateSite + " is now available on Rocket!\n", ConsoleColor.Green);
                }
                else
                {
                    if (File.Exists(updateDir + "Update-" + updateSite + ".zip")) return;

                    try
                    {
                        new WebClient().DownloadFileAsync(new Uri("http://plugins.4unturned.tk/releases/HidePlugins/" + updateSite + ".zip"), updateDir + "Update-" + updateSite + ".zip");

                        Write("Version " + updateSite + " is now available in the \"Updates\" folder\n", ConsoleColor.Green);
                    }
                    catch
                    {
                        Logger.LogError("An error occured when trying to download updates\nMore info: goo.gl/DckR7x");
                    }
                }
            }
            catch
            {
                Logger.LogError("An error occured when trying to search for updates\nMore info: goo.gl/DckR7x");
            }
        }
        #endregion

        #region OnLevelLoaded
        public void OnLevelLoaded(int level)
        {
            #region Hide Workshop
            if (Configuration.Instance.HideWorkshop)
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", null);

                WriteDebug("Set 'Browser_Workshop_Count' KeyValue to null");
            }
            #endregion

            #region Hide Config
            if (Configuration.Instance.HideConfig)
            {
                SteamGameServer.SetKeyValue("Browser_Config_Count", null);

                WriteDebug("Set 'Browser_Config_Count' KeyValue to null");
            }
            #endregion

            #region LargeServer
            if (Configuration.Instance.Enable_LargeServer)
            {
                SteamGameServer.SetMaxPlayerCount(24);

                WriteDebug("Set 'MaxPlayerCount' value to 24");
            }
            #endregion
        }
        #endregion

        #region OnPluginsLoaded
        private void OnPluginsLoaded()
        {
            #region Custom plugin Entries
            if (Configuration.Instance.Enable_CustomEntries)
            {
                SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", Configuration.Instance.CustomEntries.ToArray()));

                WriteDebug("Set 'rocketplugins' KeyValue to " + string.Join(",", Configuration.Instance.CustomEntries.ToArray()));
            }
            else
            {
                List<IRocketPlugin> plugins = R.Plugins.GetPlugins();
                List<string> pluginNames = new List<string>();

                foreach (var plugin in plugins)
                {
                    pluginNames.Add(plugin.Name);
                }

                SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", pluginNames.ToArray()));

                WriteDebug("Set 'rocketplugins' KeyValue to " + string.Join(",", pluginNames.ToArray()));
            }
            #endregion

            #region Hide Plugins
            if (Configuration.Instance.HidePlugins)
            {
                SteamGameServer.SetKeyValue("rocketplugins", null);

                WriteDebug("Set 'rocketplugins' KeyValue to null");
            }
            else
            {
                if (!Configuration.Instance.Enable_CustomEntries == true)
                {
                    List<IRocketPlugin> plugins = R.Plugins.GetPlugins();
                    List<string> pluginNames = new List<string>();

                    foreach (var plugin in plugins)
                    {
                        pluginNames.Add(plugin.Name);
                    }

                    SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", pluginNames.ToArray()));

                    WriteDebug("Set 'rocketplugins' KeyValue to " + string.Join(",", pluginNames.ToArray()));
                }
            }
            #endregion
        }
        #endregion

        #region Unload
        protected override void Unload()
        {
            Level.onLevelLoaded -= OnLevelLoaded;
            R.Plugins.OnPluginsLoaded -= OnPluginsLoaded;
            
            Write("Visit " + pluginSite + " for more!" + "\n", ConsoleColor.Yellow);
        }
        #endregion
    }
}