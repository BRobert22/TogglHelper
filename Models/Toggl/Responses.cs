using Newtonsoft.Json;
using System.Collections.Generic;

namespace TogglHelper.Models.Toggl
{
    internal class DetailsResponse
    {
        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount { get; set; }

        [JsonProperty(PropertyName = "per_page")]
        public int PerPage { get; set; }

        [JsonProperty(PropertyName = "data")]
        public List<TimeEntryData> Data { get; set; }
    }

    internal struct TimeEntryData
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "start")]
        public string StartDate { get; set; }

        [JsonProperty(PropertyName = "end")]
        public string EndDate { get; set; }

        [JsonProperty(PropertyName = "dur")]
        public int DurationInMilliseconds { get; set; }

        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }
    }
}