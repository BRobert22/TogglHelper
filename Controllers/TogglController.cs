using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TogglHelper.Enums;

namespace TogglHelper.Controllers
{
    internal class TogglController
    {
        internal static async Task<bool> SetTogglData()
        {
            var data = await Services.TogglService.GetUserData();
            if (!string.IsNullOrEmpty(data.fullname.Value))
            {
                Globals.TogglUser.Username = data.fullname;

                if (Globals.TogglUser.Workspaces == null)
                    Globals.TogglUser.Workspaces = new List<Models.Toggl.Workspace>();

                foreach (var item in data.workspaces)
                    Globals.TogglUser.Workspaces.Add(new Models.Toggl.Workspace { ID = item.id.Value, Name = item.name.Value });

                IsolatedFileController.SetIsolatedTogglUser();

                return true;
            }
            return false;
        }

        internal static void SetWorkspace()
        {
            Models.Toggl.Workspace workspace;
            if (Globals.TogglUser.Workspaces.Count == 1)
                workspace = Globals.TogglUser.Workspaces[0];
            else
            {
                var workspaces = new string[Globals.TogglUser.Workspaces.Count];
                for (int i = 0; i < Globals.TogglUser.Workspaces.Count; i++)
                    workspaces[i] = Globals.TogglUser.Workspaces[i].Name;

                var workspaceName = Screens.Screens.WorkspaceSelection(workspaces);

                workspace = Globals.TogglUser.Workspaces.FirstOrDefault(x => x.Name.Equals(workspaceName));
            }
            Globals.TogglUser.CurrentWorkspace = workspace;

            IsolatedFileController.SetIsolatedTogglUser();
        }

        internal static async Task GetTimeEntriesAsync()
        {
            if (Globals.TimeEntries != null)
                Globals.TimeEntries = null;
            var result = await Services.TogglService.GetTimeEntriesAsync();

            foreach (var entry in result.Data)
            {
                var DurationInSeconds = entry.DurationInMilliseconds / 1000;

                var StartDate = DateTime.Parse(entry.StartDate);
                var End = DateTime.Parse(entry.EndDate);

                if (!entry.User.Equals(Globals.TogglUser.Username))
                    continue;

                var TicketId = Helpers.TogglHelper.GetTicketId(entry.Description);

                if (Globals.TimeEntries == null)
                    Globals.TimeEntries = new List<Models.Toggl.TimeEntry>();

                var TimeEntry = Globals.TimeEntries.Where(x => x.TicketID == TicketId).FirstOrDefault();
                if (TimeEntry != null)
                    TimeEntry.DurationInSeconds += DurationInSeconds;
                else
                {
                    TimeEntry = new Models.Toggl.TimeEntry()
                    {
                        DurationInSeconds = DurationInSeconds,
                        Description = entry.Description,
                        Date = StartDate,
                        Status = TimeEntryStatus.Loaded,
                        TicketID = TicketId ?? 0,
                    };

                    if (TicketId == 0)
                        TimeEntry.Status = TimeEntryStatus.IDNotFound;

                    Globals.TimeEntries.Add(TimeEntry);
                }
            }
        }
    }
}