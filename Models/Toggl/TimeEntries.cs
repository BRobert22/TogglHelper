using Newtonsoft.Json;
using System;

namespace TogglHelper.Models.Toggl
{
    internal class TimeEntry
    {
        public string Description { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public long DurationInSeconds { get; set; }
        public DateTime Date { get; set; }
        public int TicketID { get; set; }
    }
}