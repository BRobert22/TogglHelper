using System.Collections.Generic;

namespace TogglHelper.Models.Kayako
{
    public class staff
    {
        public int id { get; set; }
        public string fullname { get; set; }
        public bool isenabled { get; set; }
    }

    public class timetrack
    {
        public string timeworked { get; set; }
        public string workdate { get; set; }
        public string workerstaffid { get; set; }
    }
}