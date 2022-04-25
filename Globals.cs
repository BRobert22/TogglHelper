using System;
using System.Collections.Generic;
using TogglHelper.Models.Kayako;
using TogglHelper.Models.Toggl;

namespace TogglHelper
{
    internal class Globals
    {
        public static object StartDate { get; internal set; }
        internal static Models.Toggl.User TogglUser { get; set; }
        internal static Models.Kayako.User KayakoUser { get; set; }
        internal static KayakoSettings KayakoSettings { get; set; }
        internal static List<TimeEntry> TimeEntries { get; set; }
        internal static DateTime DateFilter { get; set; }
    }
}