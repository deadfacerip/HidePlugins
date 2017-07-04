using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;

public class Config : IRocketPluginConfiguration
{
    #region Vars
    public static Config Instance;

    public string UpdateURL;
    public string DownloadURL;
    public string DisableAutoUpdates;
    
    public bool HidePlugins;
    public bool HideWorkshop;
    public bool HideConfig;
    public bool Enable_CustomEntries;

    [XmlArrayItem(ElementName = "Entry")]
    public List<string> CustomEntries;

    public bool Enable_LargeServer;
    #endregion

    #region Defaults
    public void LoadDefaults()
    {
        HidePlugins = true;
        HideWorkshop = false;
        HideConfig = false;
        Enable_CustomEntries = false;
        CustomEntries = new List<string>() { "Plugin 1", "Plugin 2", "Plugin 3" };
        Enable_LargeServer = false;
    }
    #endregion
}