namespace Z_Golmohammadi_HW_W17.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public string StaffName { get; set; }
    }
}
