using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PassExt.Models
{
    [Table("cars")]
    public class Car
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        public string Name { get; set; }
        public string No { get; set; }
        public int Places { get; set; }
        //public int Custs { get; set; }
        public string dflag { get; set; }
    }

    public class TripCar : Car
    {
        public string driver_id { get; set; }
        public string driver_fam { get; set; }
        public string driver_name { get; set; }
        public string driver_otch { get; set; }
        public string driver2_id { get; set; }
        public string driver2_fam { get; set; }
        public string driver2_name { get; set; }
        public string driver2_otch { get; set; }
    }
}