using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;

using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TogglHelper.Enums;
using TogglHelper.Models.Kayako;

namespace TogglHelper.Services
{
    internal class KayakoService
    {
        #region Privates

        private static async Task<RestResponse> SendGetRequest(string EndPoint, object Parameters = null)
        {
            try
            {
                var client = new RestClient(Globals.KayakoSettings.URL) { };
                var request = new RestRequest($"{EndPoint}?{GetAutenticacao()}");

                if (Parameters != null)
                    request.AddObject(Parameters);

                return await client.GetAsync(request);
            }
            catch (Exception ex) { }

            return null;
        }

        private static async Task<RestResponse> SendPostRequest(string EndPoint, object Parameters = null)
        {
            try
            {
                var client = new RestClient(Globals.KayakoSettings.URL) { };
                var request = new RestRequest($"?{EndPoint}&{GetAutenticacao()}");

                if (Parameters != null)
                    request.AddObject(Parameters);

                return await client.GetAsync(request);
            }
            catch (Exception ex) { }

            return null;
        }

        private static object GetAutenticacao()
        {
            var salt = Guid.NewGuid().ToString();
            var signature = GerarSignature(salt);
            var autenticacao = $"apikey={Globals.KayakoSettings.ApiKey}&salt={salt}&signature={signature}";
            return autenticacao;
        }

        private static string GerarSignature(string salt)
        {
            // Initialize the keyed hash object using the secret key as the key
            HMACSHA256 hashObject = new HMACSHA256(Encoding.UTF8.GetBytes(Globals.KayakoSettings.SecretKey));

            // Computes the signature by hashing the salt with the secret key as the key
            var signature = hashObject.ComputeHash(Encoding.UTF8.GetBytes(salt));

            // Base 64 Encode
            var encodedSignature = Convert.ToBase64String(signature);

            // URLEncode
            encodedSignature = System.Web.HttpUtility.UrlEncode(encodedSignature);

            return encodedSignature;
        }

        #endregion Privates

        #region StaffID

        internal static async Task<List<staff>> GetStaff()
        {
            var response = await SendGetRequest("/Base/Staff");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<staff>), new XmlRootAttribute("staffusers"));
                StringReader stringReader = new StringReader(response.Content);
                var staff = (List<staff>)serializer.Deserialize(stringReader);
                staff.RemoveAll(x => x.isenabled == false);
                return staff;
            }
            return null;
        }

        #endregion StaffID

        #region Tickets

        internal static async Task<List<timetrack>> GetTimeEntries(int TicketID)
        {
            var response = await SendGetRequest($"/Tickets/TicketTimeTrack/ListAll/{TicketID}");

            var a = 1;
            if (response.StatusCode.ToString().Equals("OK"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<timetrack>), new XmlRootAttribute("timetracks"));
                StringReader stringReader = new StringReader(response.Content);
                var TimeTracks = (List<timetrack>)serializer.Deserialize(stringReader);
                return TimeTracks;
            }
            return null;
        }

        internal static async Task SendTimeEntry(Models.Toggl.TimeEntry timeEntry)
        {
            var param = new
            {
                ticketid = timeEntry.TicketID,
                contents = timeEntry.Note,
                staffid = Globals.KayakoUser.StaffId.ToString(),
                worktimeline = (int)((DateTimeOffset)timeEntry.Date).ToUnixTimeSeconds(),
                billtimeline = (int)((DateTimeOffset)timeEntry.Date).ToUnixTimeSeconds(),
                timespent = timeEntry.DurationInSeconds,
                timebillable = '0',
            };

            var response = await SendPostRequest($"/Tickets/TicketTimeTrack", param);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                timeEntry.Status = TimeEntryStatus.Synced;
        }

        #endregion Tickets
    }
}