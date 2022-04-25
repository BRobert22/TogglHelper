using Spectre.Console;
using TogglHelper.Models.Kayako;
using TogglHelper.Services;
using System.Linq;

namespace TogglHelper.Controllers
{
    internal class KayakoController
    {
        public static async void SendTimeEntries(Models.Toggl.TimeEntry TimeEntry)
        {
            if (TimeEntry.TicketID == 0)
                return;

            var entries = await KayakoService.GetTimeEntries(TimeEntry.TicketID);
            //result.Wait();

            if (!entries.Any(x => x.workerstaffid.Equals(Globals.KayakoUser.StaffId.ToString())
                               && x.workdate.Equals(TimeEntry.Date)
                               && x.timeworked.Equals(TimeEntry.DurationInSeconds)))
            {
                var result = KayakoService.SendTimeEntry(TimeEntry);
                //var results = KayakoHelper.TimetracksFromXml(response.Content);

                //if (!results.Any(x =>
                //    x.Workerstaffname.Equals(Program.TogglUser.Username) && x.Workdate.Equals(apontamento.Data) &&
                //    x.Timeworked.Equals(apontamento.Duracao)))
                //{
                //    var parametros = new List<dynamic>();
                //    parametros.Add(new { param = "ticketid", value = apontamento.Ticket.ID.ToString() });
                //    parametros.Add(new { param = "contents", value = apontamento.Nota });
                //    parametros.Add(new { param = "staffid", value = Program.KayakoUser.KayakoStaffId.ToString() });
                //    parametros.Add(new { param = "worktimeline", value = (int)((DateTimeOffset)apontamento.Data).ToUnixTimeSeconds() });
                //    parametros.Add(new { param = "billtimeline", value = (int)((DateTimeOffset)apontamento.Data).ToUnixTimeSeconds() });
                //    parametros.Add(new { param = "timespent", value = apontamento.Duracao.ToString() });
                //    parametros.Add(new { param = "timebillable", value = "0" });

                //    response = SendPostRequest("/Tickets/TicketTimeTrack", parametros);

                //    if (response.StatusCode.ToString().Equals("OK"))
                //        apontamento.Status = "Sincronizado";
                //    else if (response.StatusCode.ToString().Equals("521"))
                //    {
                //        Console.WriteLine("Kayako está fora do ar!");
                //        return;
                //    }
                //}
                //else
                //{
                //    apontamento.Status = "Sincronizado";
                //}
                //RazerChromaService.ApontamentosStatus();
            }
        }

        public static void LoginKayako()
        {
            var task = KayakoService.GetStaff();
            task.Wait();

            var users = task.Result;
            var options = users.Select(x => Markup.Escape(x.fullname)).ToArray();

            var op = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Selecione o usuario do Kayako")
                    .AddChoices(options));

            var user = users.FirstOrDefault(x => x.fullname.Equals(op));
            if (user != null)
            {
                Globals.KayakoUser = new User() { StaffId = user.id };
                IsolatedFileController.SetIsolatedKayakolUser();
            }
        }
    }
}