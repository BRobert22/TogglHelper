using System.Collections.Generic;

namespace TogglHelper.Models.Toggl
{
    public class User
    {
        public string Username { get; set; }
        public string ApiToken { get; set; }
        public List<Workspace> Workspaces { get; set; }
        public Workspace CurrentWorkspace { get; set; }
    }
}