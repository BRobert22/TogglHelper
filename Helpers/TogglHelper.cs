using System.Text.RegularExpressions;

namespace TogglHelper.Helpers
{
    internal class TogglHelper
    {
        internal static int? GetTicketId(string value)
        {
            var regex = new Regex(@"(\d+)");
            var resultados = regex.Matches(value);
            if (resultados.Count > 0)
            {
                int.TryParse(resultados[0].Value, out int ticketId);
                return ticketId;
            }

            return null;
        }
    }
}