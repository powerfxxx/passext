namespace PassExt.Models
{
    public class Customer
    {
        public Customer() {
            Name = "";
            Otch = "";
            Fam = "";
            Addr = "";
            Tel = "";
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Otch { get; set; }
        public string Fam { get; set; }
        public string Addr { get; set; }
        public string Tel { get; set; }
        public int Poezdok { get; set; }
        public int Otkaz { get; set; }
        public int Nevyh { get; set; }
        public string Comment { get; set; }
        public string dflag { get; set; }
    }

}