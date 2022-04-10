using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;
using TogglHelper.Models.Toggl;

namespace TogglHelper.Services
{
    internal class TogglService
    {
        private static async Task<RestResponse> SendRequest(string EndPoint, object Parameters = null,
                                                      Method Method = Method.Get)
        {
            try
            {
                var client = new RestClient("https://api.track.toggl.com") { };
                var request = new RestRequest(EndPoint);
                request.AddHeader("Authorization", $"Basic {GetAuthToken()}");

                if (Parameters != null)
                    request.AddObject(Parameters);

                return await client.GetAsync(request);
            }
            catch (Exception ex) { }

            return null;
        }

        internal static string GetAuthToken()
        {
            return Helpers.CryptographyHelper.Base64Encode($"{Globals.TogglUser.ApiToken}:api_token");
        }

        internal static async Task<dynamic> GetUserData()
        {
            try
            {
                var response = await SendRequest("/api/v8/me");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    dynamic objContent = JsonConvert.DeserializeObject(response.Content);
                    if (objContent != null && objContent.data != null && objContent.data.fullname != null)
                        return objContent.data;
                }
            }
            catch (Exception ex) { }

            return null;
        }

        internal static async Task<DetailsResponse> GetTimeEntriesAsync()
        {
            try
            {
                var parameters = new
                {
                    workspace_id = Globals.TogglUser.CurrentWorkspace.ID,
                    since = Globals.DateFilter.ToString("yyyy-MM-dd"),
                    until = Globals.DateFilter.ToString("yyyy-MM-dd"),
                    user_agent = "api_test"
                };

                var response = await SendRequest("/reports/api/v2/details", parameters);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<DetailsResponse>(response.Content);
                    //if (objContent != null && objContent.data != null)
                    //    return objContent.data;
                }
            }
            catch (Exception ex) { }

            return null;
        }
    }
}