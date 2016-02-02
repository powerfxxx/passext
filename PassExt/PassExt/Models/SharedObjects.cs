using System.Collections.Generic;


namespace PassExt.Models
{
    public static class SharedObjects
    {
        static SharedObjects()
        {
            PGroups = new List<PGroup>();
            Ways = new List<Way>();
            Points = new List<Point>();
            Schedules = new List<Schedule>();
            Sequences = new List<StartSequence>();
            Towns = new List<Town>();


            Mapper mpr = new Mapper();
            mpr.GetPGroups();
            mpr.GetWays();
            mpr.GetPoints();
            mpr.GetSchedules();
            mpr.GetTowns();
        }

        public static bool ForInit = true;

        public static List<Way> Ways { get; set; }
        public static List<Point> Points { get; set; }
        public static List<Schedule> Schedules { get; set; }
        public static List<PGroup> PGroups { get; set; }
        public static List<StartSequence> Sequences { get; set; }
        public static List<Town> Towns { get; set; }
    }
}