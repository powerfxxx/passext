namespace PassExt.Models
{
    public class Point
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public PGroup Group { get; set; }
        public int SortIndex { get; set; }
        public int CorX { get; set; }
        public int CorY { get; set; }
        public string TakeTime { get; set; }
        public int PlacesCount { get; set; }
        public string last_car { get; set; }
        public string dflag { get; set; }
    }
}