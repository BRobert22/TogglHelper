using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TogglHelper.Controllers
{
    internal class ConfigurationController
    {
        internal static IConfigurationRoot configuration;

        public static bool getKayakoSettingValues()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            if (Globals.KayakoSettings == null)
                Globals.KayakoSettings = new Models.Kayako.KayakoSettings();

            var SectionSettings = configuration.GetSection("kayako");

            Globals.KayakoSettings.URL = SectionSettings["Url"];
            if (string.IsNullOrEmpty(Globals.KayakoSettings.URL))
            {
                Screens.Screens.ShowErrorMessage("Kayako URL not found int the App.config");
                return false;
            }

            Globals.KayakoSettings.ApiKey = SectionSettings["ApiKey"];
            if (string.IsNullOrEmpty(Globals.KayakoSettings.ApiKey))
            {
                Screens.Screens.ShowErrorMessage("Kayako ApiKey not found int the App.config");
                return false;
            }

            Globals.KayakoSettings.SecretKey = SectionSettings["SecretKey"];
            if (string.IsNullOrEmpty(Globals.KayakoSettings.SecretKey))
            {
                Screens.Screens.ShowErrorMessage("Kayako SecretKey not found int the App.config");
                return false;
            }

            return true;
        }
    }
}