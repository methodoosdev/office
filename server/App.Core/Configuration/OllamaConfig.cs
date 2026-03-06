namespace App.Core.Configuration
{
    public partial class OllamaConfig : IConfig
    {
        public string LLama318b { set; get; } = "llama3.1:8b";

        public string BaseUrl { set; get; } = "http://192.168.101.19:11434";

        public int RequestTimeoutSeconds { set; get; } = 120;
    }
}