namespace App.Core.Configuration
{
    public partial class BearerTokenConfig : IConfig
    {
        public string Key { set; get; } = "This is my shared key, not so secret, secret!This is my shared key, not so secret, secret!";

        public string Issuer { set; get; } = "https://localhost:7001/";

        public string Audience { set; get; } = "Any";

        public int AccessTokenExpirationMinutes { set; get; } = 5;

        public int RefreshTokenExpirationMinutes { set; get; } = 60;

        public bool AllowMultipleLoginsFromTheSameCustomer { set; get; } = true;

        public bool AllowSignoutAllCustomerActiveClients { set; get; } = true;
    }
}