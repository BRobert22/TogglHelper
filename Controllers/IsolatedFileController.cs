using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using TogglHelper.Helpers;

namespace TogglHelper.Controllers
{
    internal class IsolatedFileController
    {
        internal static void GetIsolatedTogglUser()
        {
            try
            {
                var json = GetIsolatedFile("togglUser.txt");
                Globals.TogglUser = JsonConvert.DeserializeObject<Models.Toggl.User>(json);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        internal static void SetIsolatedTogglUser()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Globals.TogglUser);
                SetIsolatedFile("togglUser.txt", json);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void GetIsolatedKayakoUser()
        {
            try
            {
                var json = GetIsolatedFile("kayakoUser.txt");
                Globals.KayakoUser = JsonConvert.DeserializeObject<Models.Kayako.User>(json);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void SetIsolatedKayakolUser()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Globals.KayakoUser);
                SetIsolatedFile("kayakoUser.txt", json);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static string GetIsolatedFile(string fileName)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            if (!isoStore.FileExists(fileName)) return "";

            using IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore);
            using StreamReader reader = new StreamReader(isoStream);
            var str = reader.ReadToEnd();
            var json = CryptographyHelper.Base64Decode(str);
            return json;
        }

        private static void SetIsolatedFile(string fileName, string json)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            using IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Create, isoStore);
            using StreamWriter writer = new StreamWriter(isoStream);
            var str = CryptographyHelper.Base64Encode(json);
            writer.WriteLine(str);
        }
    }
}