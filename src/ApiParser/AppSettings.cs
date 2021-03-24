using System;
using Microsoft.Extensions.Configuration;

namespace ApiParser
{
    public class AppSettings
    {
        private readonly IConfiguration _configuration;

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string UrlFormat => _configuration["UrlFormat"];
        public bool ShowNoDataWarning => _configuration.GetValue<bool>("ShowNoDataWarning");
        public bool HasHeaderRecord => _configuration.GetValue<bool>("HasHeaderRecord", false);
        public int UrlColumn => _configuration.GetValue<int>("UrlColumn", 0);
        public string AuthenticationScheme => _configuration["AuthenticationScheme"];
        public string AuthenticationValue => _configuration["AuthenticationValue"];
    }
}