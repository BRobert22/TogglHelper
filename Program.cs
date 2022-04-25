using System;
using System.Threading.Tasks;
using TogglHelper.Controllers;

namespace TogglHelper
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IsolatedFileController.GetIsolatedTogglUser();
            IsolatedFileController.GetIsolatedKayakoUser();

            if (!ConfigurationController.getKayakoSettingValues())
            {
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrEmpty(Globals.TogglUser?.ApiToken))
                await Screens.Screens.LoginToggl();

            if (Globals.KayakoUser == null)
                KayakoController.LoginKayako();

            Screens.Screens.MainMenu();
        }
    }
}