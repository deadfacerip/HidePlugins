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
using UnityEngine;

namespace HidePlugins
{
    public class HidePlugins : RocketPlugin<Config>
    {
        #region Vars
        public string pluginName = Assembly.GetExecutingAssembly().GetName().Name;
        public string pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor + "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
        public string currentRocket = Assembly.LoadFrom(System.IO.Directory.GetParent(Application.dataPath).ToString() + Path.DirectorySeparatorChar + "Modules" + Path.DirectorySeparatorChar + "Rocket.Unturned" + Path.DirectorySeparatorChar + "Rocket.Unturned.dll").GetName().Version.ToString();
        public string pluginSite = "Plugins.4Unturned.tk";
        public string unturnedVersion = "3.19.2.0 +";
        public string supportedRocket = "4.9.3.0";

        public string updateInfo;
        public string currentVersion;
        public string downloadUrl;

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
            #region 4.9.3.0 Only...
            if (currentRocket != supportedRocket)
            {
                WriteLogError("This version of Rocket (" + currentRocket + ") is not supported!\nUnloading now...");
                UnloadPlugin(PluginState.Loaded);
                Instance = null;
                return;
            }
            #endregion

            Instance = this;

            R.Plugins.OnPluginsLoaded += OnPluginsLoaded;

            WriteMenu(pluginName + ", Version: " + pluginVersion);
            WriteMenu("Made for Unturned: " + unturnedVersion);
            WriteMenu("Made for RocketMod: " + supportedRocket);
            WriteMenu("Visit " + pluginSite + " for more!" + "\n");
            WriteMenu("~~~~~ Checking for Updates ~~~~~" + "\n");

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
            WebClient webClient = new WebClient();
            string path = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Plugins" + Path.DirectorySeparatorChar + "HidePlugins" + Path.DirectorySeparatorChar + "Updates" + Path.DirectorySeparatorChar;

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            if (string.IsNullOrEmpty(Configuration.Instance.UpdateURL))
            {
                try
                {
                    currentVersion = webClient.DownloadString("http://plugins.4unturned.tk/plugins/" + pluginName + "/update");
                    updateInfo = webClient.DownloadString("http://plugins.4unturned.tk/plugins/" + pluginName + "/update_" + pluginVersion);
                }
                catch (WebException)
                {
                    WriteLogError("Failed to Search for Updates!\nMore Info: goo.gl/yAIaWz\n");
                    return;
                }
            }
            else
            {
                try
                {
                    currentVersion = webClient.DownloadString(Configuration.Instance.UpdateURL + pluginName + "/update");
                    updateInfo = webClient.DownloadString(Configuration.Instance.UpdateURL + pluginName + "/update_" + pluginVersion);
                }
                catch (WebException)
                {
                    WriteLogError("Failed to Search for Updates!\nMore Info: goo.gl/yAIaWz\n");
                    return;
                }
            }

            if (string.IsNullOrEmpty(Configuration.Instance.DownloadURL))
            {
                downloadUrl = "http://plugins.4unturned.tk/releases/HidePlugins/" + currentVersion + ".zip";
            }
            else
            {
                downloadUrl = Configuration.Instance.DownloadURL + currentVersion + ".zip";
            }

            if (updateInfo.Length < 200)
            {
                if (currentVersion != pluginVersion)
                {
                    Write(updateInfo);

                    if (string.IsNullOrEmpty(Configuration.Instance.DisableAutoUpdates))
                    {
                        try
                        {
                            if (!File.Exists(path + "Update-" + currentVersion + ".zip"))
                            {
                                Write("Downloading Update...\n");
                                webClient.DownloadFileAsync(new Uri(downloadUrl), path + "Update-" + currentVersion + ".zip");
                            }
                        }
                        catch (WebException)
                        {
                            WriteLogError("Failed to Download Updates!\nMore Info: goo.gl/yAIaWz\n");
                            return;
                        }
                    }
                    else
                    {
                        Write("To download the update, go to\n" + downloadUrl + "\n");
                    }
                }
                else
                {
                    Write(pluginName + " is up to date!\n");
                }
            }
            else
            {
                Write("No Updates Available\n");
            }
        }
        #endregion

        #region OnPluginsLoaded
        public void OnPluginsLoaded()
        {
            #region Custom Entries
            if (Configuration.Instance.Enable_CustomEntries)
            {
                SteamGameServer.SetKeyValue("rocketplugins", string.Join(",", Configuration.Instance.CustomEntries.ToArray()));
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
            }
            #endregion

            #region Hide Plugins
            if (Configuration.Instance.HidePlugins)
            {
                SteamGameServer.SetKeyValue("rocketplugins", "");
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
                }
            }
            #endregion

            #region Hide Workshop
            if (Configuration.Instance.HideWorkshop)
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "0");
            }
            else
            {
                SteamGameServer.SetKeyValue("Browser_Workshop_Count", "99");
            }
            #endregion

            #region Hide Config
            if (Configuration.Instance.HideConfig)
            {
                SteamGameServer.SetKeyValue("Browser_Config_Count", "0");
            }
            else
            {
                SteamGameServer.SetKeyValue("Browser_Config_Count", "99");
            }
            #endregion

            #region MaxPlayerCount
            if (Configuration.Instance.Enable_LargeServer)
            {
                SteamGameServer.SetMaxPlayerCount(24);
            }
            else
            {
                SteamGameServer.SetMaxPlayerCount(Provider.maxPlayers);
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