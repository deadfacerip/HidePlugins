using Rocket.API;

public class Config : IRocketPluginConfiguration
{
    #region Vars
    public static Config Instance;

    public bool HidePlugins;
    public bool HideWorkshop;
    public bool HideConfig;
    public bool Enable_CustomEntry;
    public string CustomEntry;
    public bool Enable_LargeServer;
    #endregion

    #region Defaults
    public void LoadDefaults()
    {
        HidePlugins = true;
        HideWorkshop = false;
        HideConfig = false;
        Enable_CustomEntry = false;
        CustomEntry = @"乁(ツ)ㄏ,乁(ಥ.ಥ)ㄏ,乁(ಠ-ಠ)ㄏ";
        Enable_LargeServer = false;
    }
    #endregion
}