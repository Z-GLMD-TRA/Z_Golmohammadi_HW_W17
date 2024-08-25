namespace Z_Golmohammadi_HW_W17.Models
{
    public class OrderProductViewModel
    {
        public List<Order> OrderInfo { get; set; }
        public Order SelectedOrder { get; set; }
        public List<Product> ProductInfo { get; set; }
        public decimal TotalPayment { get; set; }
    }
}
