namespace App.Core.Configuration
{
    public partial class PlaywrightConfig : IConfig
    {
        public string DownLoadPath { set; get; } = "C:\\Temp";

        public int Delay { set; get; } = 200;

        public bool EmulateBrowserEnable { set; get; } = true;

        public bool Headless { set; get; } = true;
    }
}