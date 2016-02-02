namespace PassExt.Models
{
    public class Trip
    {
        public string ID { get; set; }
        public string ScheduleID { get; set; }
        public string WayName { get; set; }
        public string TownFromID { get; set; }
        public string TownFromMap { get; set; }
        public string TownToID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int PassCount { get; set; }
        public int RowsCount { get; set; }
        public string Comments { get; set; }
        public string TripDate { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
    }
}