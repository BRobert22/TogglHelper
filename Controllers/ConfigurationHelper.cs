using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace TogglHelper.Controllers
{
    internal class ConfigurationHelper
    {
        internal static IConfigurationRoot configuration;

        internal static void getKayakoSettingValues()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            if (Globals.KayakoSettings == null)
                Globals.KayakoSettings = new Models.Kayako.KayakoSettings();

            var SectionSettings = configuration.GetSection("kayako");
            Globals.KayakoSettings.URL = SectionSettings["Url"];
            Globals.KayakoSettings.ApiKey = SectionSettings["ApiKey"];
            Globals.KayakoSettings.SecretKey = SectionSettings["SecretKey"];
        }
    }
}