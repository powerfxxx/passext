using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassExt.Models
{
    public class Order
    {
        public Order()
        {
            
        }
        public string Fam { get; set; }
        public string Tel { get; set; }
        public string WayId { get; set; }
        public string ScheduleId { get; set; }
        public string StartTime { get; set; }
        public string Date { get; set; }
        private IEnumerable<Pnt> _points = new List<Pnt>();
        public IEnumerable<Pnt> Points
        {
            get { return _points; }
            set { _points = value;  }
        }
    }

    public class Pnt
    {
        public string Id { get; set; }
        public string Count{ get; set; }
        public string Time { get; set; }
        public string Comment { get; set; }
    }
}