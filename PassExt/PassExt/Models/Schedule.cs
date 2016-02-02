using System.Collections.Generic;
using PassExt.Models;

namespace PassExt.Models
{
    public class Schedule
    {
        public Schedule()
        {
            PGroups = new List<PGroup>();
        }
        public string ID { get; set; }
        public string WayID { get; set; }
        public string Start_time { get; set; }
        public string End_time { get; set; }
        public string GoBack { get; set; }

        private string days="0000000";
        public string Days
        {
            get { return days; }
            set { days = value; }
        }
        public string Sequence_ID { get; set; }

        public string UDate { get; set; }
        public List<PGroup> PGroups { get; set; }
        public StartSequence Sequence { get; set; }
        public string dflag { get; set; }
    }
}