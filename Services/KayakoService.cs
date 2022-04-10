namespace TogglHelper.Services
{
    internal class KayakoService
    {
        //private static async Task<RestResponse> SendRequest(string EndPoint, object Parameters = null,
        //                                              Method Method = Method.Get)
        //{
        //    try
        //    {
        //        var client = new RestClient(Globals.) { };
        //        var request = new RestRequest(EndPoint);
        //        request.AddHeader("Authorization", $"Basic {GetAuthToken()}");

        //        if (Parameters != null)
        //            request.AddObject(Parameters);

        //        return await client.GetAsync(request);
        //    }
        //    catch (Exception ex) { }

        //    return null;

        //    private static IRestResponse SendPostRequest(string endPoint, List<dynamic> parametros = null)
        //    {
        //        var client = new RestClient($"{Program.UrlKayako}?{endPoint}");

        //        var request = new RestRequest(Method.POST);
        //        request.AddHeader("accept", "*/*");
        //        request.AddHeader("content-length", "250");
        //        request.AddHeader("content-type", "application/x-www-form-urlencoded");

        //        var strParametros = "";
        //        if (parametros != null)
        //            foreach (var param in parametros)
        //                strParametros += $"&{param.param}={param.value}";

        //        request.AddParameter("application/x-www-form-urlencoded", $"{KayakoHelper.GetAutenticacao()}{strParametros}", ParameterType.RequestBody);
        //        IRestResponse response = client.Execute(request);
        //        return response;
        //    }
    }
}