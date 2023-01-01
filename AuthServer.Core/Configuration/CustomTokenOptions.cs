using System;
using System.Collections.Generic;

namespace AuthServer.Core.Configuration
{
    public class CustomTokenOptions
    {
        public List<String> Audince { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public string SecurityKey { get; set; }

    }
}
