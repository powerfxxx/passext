namespace PassExt.Models
{
    public class Way
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string TownTo_id { get; set; }
        public string TownTo_Name { get; set; }
        public string TownFrom_id { get; set; }
        public string TownFrom_Name { get; set; }
        public string GoBack { get; set; }
        public string dflag { get; set; }
    }
}