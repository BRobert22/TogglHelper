using System.Threading.Tasks;
using TogglHelper.Controllers;

namespace TogglHelper
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IsolatedFileController.GetIsolatedTogglUser();

            if (string.IsNullOrEmpty(Globals.TogglUser?.ApiToken))
                await Screens.Screens.LoginToggl();

            Helpers.ConfigurationHelper.getKayakoSettingValues();

            Screens.Screens.MainMenu();
        }
    }
}