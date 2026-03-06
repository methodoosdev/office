namespace App.Core.Configuration
{
    /// <summary>
    /// Represents installation configuration parameters
    /// </summary>
    public partial class InstallationConfig : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether a store owner can install sample data during installation
        /// </summary>
        public bool DisableSampleData { get; private set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to download and setup the regional language pack during installation
        /// </summary>
        public bool InstallRegionalResources { get; private set; } = true;
    }
}