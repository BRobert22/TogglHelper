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

        internal string PrintDuration()
        {
            var milliseconds = DurationInSeconds * 1000;
            var t = TimeSpan.FromMilliseconds(milliseconds);
            return $"{t.Hours:D2}h:{t.Minutes:D2}m:{t.Seconds:D2}s";
        }

        internal string hasNote()
        {
            return (!string.IsNullOrEmpty(Note)).ToString();
        }
    }
}